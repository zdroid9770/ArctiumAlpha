using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace Common.Cryptography
{
    public sealed class BigInteger : IEquatable<BigInteger>, IComparable<BigInteger>, IComparable
    {
        /// <summary>
        /// Maximum length of the BigInteger. The actual in-memory size is 4 * MaxLength.
        /// </summary>
        public const int MaxLength = 512;

        private readonly uint[] _data;

        private int _dataLength;

        /// <summary>
        /// Gets the number of bytes that are actually used in the BigInteger.
        /// </summary>
        public int ByteLength
        {
            get
            {
                var numBits = BitLength;
                var numBytes = numBits >> 3;

                if ((numBits & 0x7) != 0)
                    numBytes++;

                return numBytes;
            }
        }

        public BigInteger()
        {
            _data = new uint[MaxLength];
            _dataLength = 1;
        }

        public BigInteger(long value)
            : this()
        {
            var tempVal = value;
            _dataLength = 0;

            while (value != 0 && _dataLength < MaxLength)
            {
                _data[_dataLength] = (uint)(value & 0xffffffff);
                value >>= 32;
                _dataLength++;
            }

            if (tempVal > 0 && (value != 0 || (_data[MaxLength - 1] & 0x80000000) != 0))
                throw new ArithmeticException("Positive overflow.");

            if (tempVal < 0 && (value != -1 || (_data[_dataLength - 1] & 0x80000000) == 0))
                throw new ArithmeticException("Negative underflow.");

            if (_dataLength == 0)
                _dataLength = 1;
        }

        public BigInteger(ulong value)
            : this()
        {
            _dataLength = 0;

            while (value != 0 && _dataLength < MaxLength)
            {
                _data[_dataLength] = (uint)(value & 0xffffffff);
                value >>= 32;
                _dataLength++;
            }

            if (value != 0 || (_data[MaxLength - 1] & 0x80000000) != 0)
                throw new ArithmeticException("Positive overflow.");

            if (_dataLength == 0)
                _dataLength = 1;
        }

        public BigInteger(BigInteger bi)
            : this()
        {
            Contract.Requires(bi != null);

            _dataLength = bi._dataLength;

            for (var i = 0; i < _dataLength; i++)
                _data[i] = bi._data[i];
        }

        public BigInteger(string value, int radix = 16)
        {
            Contract.Requires(!string.IsNullOrEmpty(value));
            Contract.Requires(value.Length > 0);

            var multiplier = new BigInteger(1);
            var result = new BigInteger();
            value = value.ToUpper(CultureInfo.InvariantCulture).Trim();
            var limit = 0;

            if (value.Length == 0)
                throw new ArgumentException("value is empty.", "value");

            if (value[0] == '-')
                limit = 1;

            for (var i = value.Length - 1; i >= limit; i--)
            {
                int posVal = value[i];

                if (posVal >= '0' && posVal <= '9')
                    posVal -= '0';
                else if (posVal >= 'A' && posVal <= 'Z')
                    posVal = (posVal - 'A') + 10;
                else
                    posVal = 9999999;

                if (posVal >= radix)
                    throw new ArgumentException("Invalid string.");

                if (value[0] == '-')
                    posVal = -posVal;

                result = result + (multiplier * posVal);

                if ((i - 1) >= limit)
                    multiplier = multiplier * radix;
            }

            if (value[0] == '-' && (result._data[MaxLength - 1] & 0x80000000) == 0)
                throw new ArithmeticException("Negative underflow.");

            if ((result._data[MaxLength - 1] & 0x80000000) != 0)
                throw new ArithmeticException("Positive overflow.");

            _data = new uint[MaxLength];

            for (var i = 0; i < result._dataLength; i++)
                _data[i] = result._data[i];

            _dataLength = result._dataLength;
        }

        public BigInteger(byte[] inData)
        {
            Contract.Requires(inData != null);
            Contract.Ensures(ByteLength == inData.Length);

            inData = (byte[])inData.Clone();
            inData = inData.Reverse().ToArray();

            var dataLength = inData.Length >> 2;

            var leftOver = inData.Length & 0x3;
            if (leftOver != 0)
                dataLength++;

            if (dataLength > MaxLength)
                throw new ArithmeticException("Byte overflow.");

            _dataLength = dataLength;
            _data = new uint[MaxLength];

            for (int i = inData.Length - 1, j = 0; i >= 3; i -= 4, j++)
                _data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                    (inData[i - 1] << 8) + inData[i]);

            switch (leftOver)
            {
                case 1:
                    _data[_dataLength - 1] = inData[0];
                    break;
                case 2:
                    _data[_dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
                    break;
                case 3:
                    _data[_dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
                    break;
            }

            while (_dataLength > 1 && _data[_dataLength - 1] == 0)
                _dataLength--;

            Contract.Assume(ByteLength == inData.Length);
        }

        public BigInteger(uint[] inData)
        {
            Contract.Requires(inData != null);

            inData = (uint[])inData.Clone();
            _dataLength = inData.Length;

            _data = new uint[MaxLength];

            for (int i = _dataLength - 1, j = 0; i >= 0; i--, j++)
                _data[j] = inData[i];

            while (_dataLength > 1 && _data[_dataLength - 1] == 0)
                _dataLength--;
        }

        public BigInteger(Random rand, int bitLength)
        {
            Contract.Requires(bitLength > 0);

            if (rand == null)
                rand = new Random(Environment.TickCount);

            _data = new uint[MaxLength];
            _dataLength = 1;

            GenerateRandomBits(bitLength, rand);
        }

        public static explicit operator BigInteger(long value)
        {
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return new BigInteger(value);
        }

        public static explicit operator BigInteger(ulong value)
        {
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return new BigInteger(value);
        }

        public static explicit operator BigInteger(int value)
        {
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return new BigInteger(value);
        }

        public static explicit operator BigInteger(uint value)
        {
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return new BigInteger((ulong)value);
        }

        public static implicit operator BigInteger(byte[] value)
        {
            Contract.Requires(value != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);
            Contract.Ensures(Contract.Result<BigInteger>().ByteLength == value.Length);

            return new BigInteger(value);
        }

        public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger();
            result._dataLength = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;
            long carry = 0;

            for (var i = 0; i < result._dataLength; i++)
            {
                var sum = (long)bi1._data[i] + bi2._data[i] + carry;
                carry = sum >> 32;
                result._data[i] = (uint)(sum & 0xffffffff);
            }

            if (carry != 0 && result._dataLength < MaxLength)
            {
                result._data[result._dataLength] = (uint)(carry);
                result._dataLength++;
            }

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            const int lastPos = MaxLength - 1;
            if ((bi1._data[lastPos] & 0x80000000) == (bi2._data[lastPos] & 0x80000000) &&
                (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
                throw new ArithmeticException("Overflow in operator +.");

            return result;
        }

        public static BigInteger operator +(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 + (BigInteger)bi2;
        }

        public static BigInteger operator +(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 + (BigInteger)bi2;
        }

        public static BigInteger operator +(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 + (BigInteger)bi2;
        }

        public static BigInteger operator +(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 + (BigInteger)bi2;
        }

        public static BigInteger operator ++(BigInteger bi1)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger(bi1);
            long carry = 1;
            var index = 0;

            while (carry != 0 && index < MaxLength)
            {
                long val = result._data[index];
                val++;
                result._data[index] = (uint)(val & 0xffffffff);
                carry = val >> 32;
                index++;
            }

            if (index <= result._dataLength)
            {
                while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                    result._dataLength--;
            }
            else
                result._dataLength = index;

            const int lastPos = MaxLength - 1;

            if ((bi1._data[lastPos] & 0x80000000) == 0 && (result._data[lastPos] & 0x80000000) !=
                (bi1._data[lastPos] & 0x80000000))
                throw new ArithmeticException("Overflow in operator ++.");

            return result;
        }

        public static BigInteger operator -(BigInteger bi1)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            if (bi1._dataLength == 1 && bi1._data[0] == 0)
                return new BigInteger();

            var result = new BigInteger(bi1);

            for (var i = 0; i < MaxLength; i++)
                result._data[i] = ~bi1._data[i];

            long carry = 1;
            var index = 0;

            while (carry != 0 && index < MaxLength)
            {
                long val = result._data[index];
                val++;
                result._data[index] = (uint)(val & 0xffffffff);
                carry = val >> 32;
                index++;
            }

            if ((bi1._data[MaxLength - 1] & 0x80000000) == (result._data[MaxLength - 1] & 0x80000000))
                throw new ArithmeticException("Overflow in operator -.");

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }

        public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger();
            result._dataLength = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;
            long carryIn = 0;

            for (var i = 0; i < result._dataLength; i++)
            {
                var diff = bi1._data[i] - (long)bi2._data[i] - carryIn;
                result._data[i] = (uint)(diff & 0xffffffff);
                carryIn = diff < 0 ? 1 : 0;
            }

            if (carryIn != 0)
            {
                for (var i = result._dataLength; i < MaxLength; i++)
                    result._data[i] = 0xffffffff;

                result._dataLength = MaxLength;
            }

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            const int lastPos = MaxLength - 1;

            if ((bi1._data[lastPos] & 0x80000000) != (bi2._data[lastPos] & 0x80000000) &&
                (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
                throw new ArithmeticException("Underflow in operator -.");

            return result;
        }

        public static BigInteger operator -(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 - (BigInteger)bi2;
        }

        public static BigInteger operator -(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 - (BigInteger)bi2;
        }

        public static BigInteger operator -(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 - (BigInteger)bi2;
        }

        public static BigInteger operator -(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 - (BigInteger)bi2;
        }

        public static BigInteger operator --(BigInteger bi1)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger(bi1);
            var carryIn = true;
            var index = 0;

            while (carryIn && index < MaxLength)
            {
                long val = result._data[index];
                val--;
                result._data[index] = (uint)(val & 0xffffffff);

                if (val >= 0)
                    carryIn = false;

                index++;
            }

            if (index > result._dataLength)
                result._dataLength = index;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            const int lastPos = MaxLength - 1;

            if ((bi1._data[lastPos] & 0x80000000) != 0 && (result._data[lastPos] & 0x80000000) !=
                (bi1._data[lastPos] & 0x80000000))
                throw new ArithmeticException("Underflow in operator --.");

            return result;
        }

        public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            const int lastPos = MaxLength - 1;
            var bi1Neg = false;
            var bi2Neg = false;

            try
            {
                if ((bi1._data[lastPos] & 0x80000000) != 0)
                {
                    bi1Neg = true;
                    bi1 = -bi1;
                }

                if ((bi2._data[lastPos] & 0x80000000) != 0)
                {
                    bi2Neg = true;
                    bi2 = -bi2;
                }
            }
            catch
            {
            }

            var result = new BigInteger();

            try
            {
                for (var i = 0; i < bi1._dataLength; i++)
                {
                    if (bi1._data[i] == 0)
                        continue;

                    ulong mcarry = 0;

                    for (int j = 0, k = i; j < bi2._dataLength; j++, k++)
                    {
                        var val = ((ulong)bi1._data[i] * bi2._data[j]) + result._data[k] + mcarry;

                        result._data[k] = (uint)(val & 0xffffffff);
                        mcarry = (val >> 32);
                    }

                    if (mcarry != 0)
                        result._data[i + bi2._dataLength] = (uint)mcarry;
                }
            }
            catch (Exception)
            {
                throw new ArithmeticException("Multiplication overflow.");
            }

            result._dataLength = bi1._dataLength + bi2._dataLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            if ((result._data[lastPos] & 0x80000000) != 0)
            {
                if (bi1Neg != bi2Neg && result._data[lastPos] == 0x80000000)
                {
                    if (result._dataLength == 1)
                        return result;

                    var isMaxNeg = true;

                    for (var i = 0; i < result._dataLength - 1 && isMaxNeg; i++)
                        if (result._data[i] != 0)
                            isMaxNeg = false;

                    if (isMaxNeg)
                        return result;
                }

                throw new ArithmeticException("Multiplication overflow.");
            }

            if (bi1Neg != bi2Neg)
                return -result;

            return result;
        }

        public static BigInteger operator *(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 * (BigInteger)bi2;
        }

        public static BigInteger operator *(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 * (BigInteger)bi2;
        }

        public static BigInteger operator *(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 * (BigInteger)bi2;
        }

        public static BigInteger operator *(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 * (BigInteger)bi2;
        }

        private static void MultiByteDivide(BigInteger bi1, BigInteger bi2, ref BigInteger outQuotient,
            ref BigInteger outRemainder)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Requires(outQuotient != null);
            Contract.Requires(outRemainder != null);
            Contract.Ensures(outQuotient != null);
            Contract.Ensures(outRemainder != null);

            var result = new uint[MaxLength];
            var remainderLen = bi1._dataLength + 1;
            var remainder = new uint[remainderLen];

            var mask = 0x80000000;
            var val = bi2._data[bi2._dataLength - 1];
            var shift = 0;
            var resultPos = 0;

            while (mask != 0 && (val & mask) == 0)
            {
                shift++;
                mask >>= 1;
            }

            for (var i = 0; i < bi1._dataLength; i++)
                remainder[i] = bi1._data[i];

            ShiftLeft(remainder, shift);
            bi2 = bi2 << shift;

            var j = remainderLen - bi2._dataLength;
            var pos = remainderLen - 1;

            ulong firstDivisorByte = bi2._data[bi2._dataLength - 1];
            ulong secondDivisorByte = bi2._data[bi2._dataLength - 2];

            var divisorLen = bi2._dataLength + 1;
            var dividendPart = new uint[divisorLen];

            while (j > 0)
            {
                var dividend = ((ulong)remainder[pos] << 32) + remainder[pos - 1];
                Contract.Assume(firstDivisorByte > 0);
                var q_hat = dividend / firstDivisorByte;
                var r_hat = dividend % firstDivisorByte;
                var done = false;

                while (!done)
                {
                    done = true;

                    if (q_hat != 0x100000000 && (q_hat * secondDivisorByte) <=
                        ((r_hat << 32) + remainder[pos - 2]))
                        continue;

                    q_hat--;
                    r_hat += firstDivisorByte;

                    if (r_hat < 0x100000000)
                        done = false;
                }

                for (var h = 0; h < divisorLen; h++)
                    dividendPart[h] = remainder[pos - h];

                var kk = new BigInteger(dividendPart);
                var ss = bi2 * (long)q_hat;

                while (ss > kk)
                {
                    q_hat--;
                    ss -= bi2;
                }

                var yy = kk - ss;

                for (var h = 0; h < divisorLen; h++)
                    remainder[pos - h] = yy._data[bi2._dataLength - h];

                result[resultPos++] = (uint)q_hat;

                pos--;
                j--;
            }

            outQuotient._dataLength = resultPos;
            var y = 0;

            for (var x = outQuotient._dataLength - 1; x >= 0; x--, y++)
                outQuotient._data[y] = result[x];

            for (; y < MaxLength; y++)
                outQuotient._data[y] = 0;

            while (outQuotient._dataLength > 1 && outQuotient._data[outQuotient._dataLength - 1] == 0)
                outQuotient._dataLength--;

            if (outQuotient._dataLength == 0)
                outQuotient._dataLength = 1;

            outRemainder._dataLength = ShiftRight(remainder, shift);

            for (y = 0; y < outRemainder._dataLength; y++)
                outRemainder._data[y] = remainder[y];

            for (; y < MaxLength; y++)
                outRemainder._data[y] = 0;
        }

        private static void SingleByteDivide(BigInteger bi1, BigInteger bi2, ref BigInteger outQuotient,
            ref BigInteger outRemainder)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Requires(outQuotient != null);
            Contract.Requires(outRemainder != null);
            Contract.Ensures(outQuotient != null);
            Contract.Ensures(outRemainder != null);

            var result = new uint[MaxLength];
            var resultPos = 0;

            for (var i = 0; i < MaxLength; i++)
                outRemainder._data[i] = bi1._data[i];

            outRemainder._dataLength = bi1._dataLength;

            while (outRemainder._dataLength > 1 && outRemainder._data[outRemainder._dataLength - 1] == 0)
                outRemainder._dataLength--;

            ulong divisor = bi2._data[0];
            var pos = outRemainder._dataLength - 1;
            ulong dividend = outRemainder._data[pos];

            if (dividend >= divisor)
            {
                Contract.Assume(divisor > 0);
                var quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder._data[pos] = (uint)(dividend % divisor);
            }

            pos--;

            while (pos >= 0)
            {
                dividend = ((ulong)outRemainder._data[pos + 1] << 32) + outRemainder._data[pos];
                var quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder._data[pos + 1] = 0;
                outRemainder._data[pos--] = (uint)(dividend % divisor);
            }

            outQuotient._dataLength = resultPos;
            var j = 0;

            for (var i = outQuotient._dataLength - 1; i >= 0; i--, j++)
                outQuotient._data[j] = result[i];

            for (; j < MaxLength; j++)
                outQuotient._data[j] = 0;

            while (outQuotient._dataLength > 1 && outQuotient._data[outQuotient._dataLength - 1] == 0)
                outQuotient._dataLength--;

            if (outQuotient._dataLength == 0)
                outQuotient._dataLength = 1;

            while (outRemainder._dataLength > 1 && outRemainder._data[outRemainder._dataLength - 1] == 0)
                outRemainder._dataLength--;
        }

        public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            if (bi1 == null)
                throw new ArgumentNullException("bi1");

            if (bi2 == null)
                throw new ArgumentNullException("bi2");

            var quotient = new BigInteger();
            var remainder = new BigInteger();

            const int lastPos = MaxLength - 1;
            bool divisorNeg = false, dividendNeg = false;

            if ((bi1._data[lastPos] & 0x80000000) != 0)
            {
                bi1 = -bi1;
                dividendNeg = true;
            }

            if ((bi2._data[lastPos] & 0x80000000) != 0)
            {
                bi2 = -bi2;
                divisorNeg = true;
            }

            if (bi1 < bi2)
                return quotient;

            if (bi2._dataLength == 1)
                SingleByteDivide(bi1, bi2, ref quotient, ref remainder);
            else
                MultiByteDivide(bi1, bi2, ref quotient, ref remainder);

            if (dividendNeg != divisorNeg)
                return -quotient;

            return quotient;
        }

        public static BigInteger operator /(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 / (BigInteger)bi2;
        }

        public static BigInteger operator /(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 / (BigInteger)bi2;
        }

        public static BigInteger operator /(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 / (BigInteger)bi2;
        }

        public static BigInteger operator /(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 / (BigInteger)bi2;
        }

        public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var quotient = new BigInteger();
            var remainder = new BigInteger(bi1);
            const int lastPos = MaxLength - 1;
            var dividendNeg = false;

            if ((bi1._data[lastPos] & 0x80000000) != 0)
            {
                bi1 = -bi1;
                dividendNeg = true;
            }

            if ((bi2._data[lastPos] & 0x80000000) != 0)
                bi2 = -bi2;

            if (bi1 < bi2)
                return remainder;

            if (bi2._dataLength == 1)
                SingleByteDivide(bi1, bi2, ref quotient, ref remainder);
            else
                MultiByteDivide(bi1, bi2, ref quotient, ref remainder);

            if (dividendNeg)
                return -remainder;

            return remainder;
        }

        public static BigInteger operator %(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 % (BigInteger)bi2;
        }

        public static BigInteger operator %(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 % (BigInteger)bi2;
        }

        public static BigInteger operator %(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 % (BigInteger)bi2;
        }

        public static BigInteger operator %(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return bi1 % (BigInteger)bi2;
        }

        public static BigInteger operator %(long bi1, BigInteger bi2)
        {
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return (BigInteger)bi1 % bi2;
        }

        public static BigInteger operator %(ulong bi1, BigInteger bi2)
        {
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return (BigInteger)bi1 % bi2;
        }

        public static BigInteger operator %(int bi1, BigInteger bi2)
        {
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return (BigInteger)bi1 % bi2;
        }

        public static BigInteger operator %(uint bi1, BigInteger bi2)
        {
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return (BigInteger)bi1 % bi2;
        }

        public static BigInteger operator <<(BigInteger bi1, int shiftVal)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger(bi1);
            Contract.Assume(result._data != null);
            result._dataLength = ShiftLeft(result._data, shiftVal);
            return result;
        }

        private static int ShiftLeft(IList<uint> buffer, int shiftVal)
        {
            Contract.Requires(buffer != null);
            Contract.Ensures(Contract.Result<int>() >= 0);

            var shiftAmount = 32;
            var bufLen = buffer.Count;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            for (var count = shiftVal; count > 0; )
            {
                if (count < shiftAmount)
                    shiftAmount = count;

                ulong carry = 0;

                for (var i = 0; i < bufLen; i++)
                {
                    var val = ((ulong)buffer[i]) << shiftAmount;
                    val |= carry;
                    buffer[i] = (uint)(val & 0xffffffff);
                    carry = val >> 32;
                }

                if (carry != 0)
                {
                    if (bufLen + 1 <= buffer.Count)
                    {
                        buffer[bufLen] = (uint)carry;
                        bufLen++;
                    }
                }

                count -= shiftAmount;
            }

            return bufLen;
        }

        public static BigInteger operator >>(BigInteger bi1, int shiftVal)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger(bi1);
            Contract.Assume(result._data != null);
            result._dataLength = ShiftRight(result._data, shiftVal);

            if ((bi1._data[MaxLength - 1] & 0x80000000) != 0)
            {
                for (var i = MaxLength - 1; i >= result._dataLength; i--)
                    result._data[i] = 0xffffffff;

                var mask = 0x80000000;
                for (var i = 0; i < 32; i++)
                {
                    if ((result._data[result._dataLength - 1] & mask) != 0)
                        break;

                    result._data[result._dataLength - 1] |= mask;
                    mask >>= 1;
                }

                result._dataLength = MaxLength;
            }

            return result;
        }

        private static int ShiftRight(IList<uint> buffer, int shiftVal)
        {
            Contract.Requires(buffer != null);
            Contract.Ensures(Contract.Result<int>() >= 0);

            var shiftAmount = 32;
            var invShift = 0;
            var bufLen = buffer.Count;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            for (var count = shiftVal; count > 0; )
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                ulong carry = 0;

                for (var i = bufLen - 1; i >= 0; i--)
                {
                    var val = ((ulong)buffer[i]) >> shiftAmount;
                    val |= carry;
                    carry = ((ulong)buffer[i]) << invShift;
                    buffer[i] = (uint)(val);
                }

                count -= shiftAmount;
            }

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            return bufLen;
        }

        public static BigInteger operator ~(BigInteger bi1)
        {
            Contract.Requires(bi1 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger(bi1);
            for (var i = 0; i < MaxLength; i++)
                result._data[i] = ~(bi1._data[i]);

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }

        public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger();
            var len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

            for (var i = 0; i < len; i++)
            {
                var sum = bi1._data[i] & bi2._data[i];
                result._data[i] = sum;
            }

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }

        public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger();
            var len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

            for (var i = 0; i < len; i++)
            {
                var sum = bi1._data[i] | bi2._data[i];
                result._data[i] = sum;
            }

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }

        public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var result = new BigInteger();
            var len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

            for (var i = 0; i < len; i++)
            {
                var sum = bi1._data[i] ^ bi2._data[i];
                result._data[i] = sum;
            }

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }

        public static bool operator ==(BigInteger bi1, BigInteger bi2)
        {
            var obi1 = bi1 as object;
            var obi2 = bi2 as object;

            if (obi1 == null && obi2 == null)
                return true;

            if (obi1 == null || obi2 == null)
                return false;

            return bi1.Equals(bi2);
        }

        public static bool operator ==(BigInteger bi1, uint bi2)
        {
            return bi1 == (BigInteger)bi2;
        }

        public static bool operator ==(BigInteger bi1, int bi2)
        {
            return bi1 == (BigInteger)bi2;
        }

        public static bool operator ==(BigInteger bi1, long bi2)
        {
            return bi1 == (BigInteger)bi2;
        }

        public static bool operator ==(BigInteger bi1, ulong bi2)
        {
            return bi1 == (BigInteger)bi2;
        }

        public static bool operator !=(BigInteger bi1, BigInteger bi2)
        {
            return !(bi1 == bi2);
        }

        public static bool operator !=(BigInteger bi1, uint bi2)
        {
            return bi1 != (BigInteger)bi2;
        }

        public static bool operator !=(BigInteger bi1, int bi2)
        {
            return bi1 != (BigInteger)bi2;
        }

        public static bool operator !=(BigInteger bi1, long bi2)
        {
            return bi1 != (BigInteger)bi2;
        }

        public static bool operator !=(BigInteger bi1, ulong bi2)
        {
            return bi1 != (BigInteger)bi2;
        }

        public static bool operator >(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);

            var pos = MaxLength - 1;

            if ((bi1._data[pos] & 0x80000000) != 0 && (bi2._data[pos] & 0x80000000) == 0)
                return false;

            if ((bi1._data[pos] & 0x80000000) == 0 && (bi2._data[pos] & 0x80000000) != 0)
                return true;

            var len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;
            for (pos = len - 1; pos >= 0 && bi1._data[pos] == bi2._data[pos]; pos--)
            {
            }

            if (pos >= 0)
                return bi1._data[pos] > bi2._data[pos];

            return false;
        }

        public static bool operator >(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 > (BigInteger)bi2;
        }

        public static bool operator >(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 > (BigInteger)bi2;
        }

        public static bool operator >(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 > (BigInteger)bi2;
        }

        public static bool operator >(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 > (BigInteger)bi2;
        }

        public static bool operator <(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);

            var pos = MaxLength - 1;

            if ((bi1._data[pos] & 0x80000000) != 0 && (bi2._data[pos] & 0x80000000) == 0)
                return true;

            if ((bi1._data[pos] & 0x80000000) == 0 && (bi2._data[pos] & 0x80000000) != 0)
                return false;

            var len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;
            for (pos = len - 1; pos >= 0 && bi1._data[pos] == bi2._data[pos]; pos--)
            {
            }

            if (pos >= 0)
                return bi1._data[pos] < bi2._data[pos];

            return false;
        }

        public static bool operator <(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 < (BigInteger)bi2;
        }

        public static bool operator <(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 < (BigInteger)bi2;
        }

        public static bool operator <(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 < (BigInteger)bi2;
        }

        public static bool operator <(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 < (BigInteger)bi2;
        }

        public static bool operator >=(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);

            return (bi1 == bi2 || bi1 > bi2);
        }

        public static bool operator >=(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 >= (BigInteger)bi2;
        }

        public static bool operator >=(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 >= (BigInteger)bi2;
        }

        public static bool operator >=(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 >= (BigInteger)bi2;
        }

        public static bool operator >=(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 >= (BigInteger)bi2;
        }

        public static bool operator <=(BigInteger bi1, BigInteger bi2)
        {
            Contract.Requires(bi1 != null);
            Contract.Requires(bi2 != null);

            return (bi1 == bi2 || bi1 < bi2);
        }

        public static bool operator <=(BigInteger bi1, long bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 <= (BigInteger)bi2;
        }

        public static bool operator <=(BigInteger bi1, ulong bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 <= (BigInteger)bi2;
        }

        public static bool operator <=(BigInteger bi1, int bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 <= (BigInteger)bi2;
        }

        public static bool operator <=(BigInteger bi1, uint bi2)
        {
            Contract.Requires(bi1 != null);

            return bi1 <= (BigInteger)bi2;
        }

        public BigInteger Max(BigInteger bi)
        {
            Contract.Requires(bi != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return this > bi ? (new BigInteger(this)) : (new BigInteger(bi));
        }

        public BigInteger Min(BigInteger bi)
        {
            Contract.Requires(bi != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return this < bi ? new BigInteger(this) : new BigInteger(bi);
        }

        public BigInteger Abs()
        {
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            return (_data[MaxLength - 1] & 0x80000000) != 0 ? new BigInteger(-this) :
                new BigInteger(this);
        }

        public BigInteger ModPow(BigInteger exp, BigInteger n)
        {
            Contract.Requires(exp != null);
            Contract.Requires(n != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            if ((exp._data[MaxLength - 1] & 0x80000000) != 0)
                throw new ArgumentException("ModPow overflow.");

            var resultNum = (BigInteger)1;
            BigInteger tempNum;
            var thisNegative = false;

            if ((_data[MaxLength - 1] & 0x80000000) != 0)
            {
                tempNum = -this % n;
                thisNegative = true;
            }
            else
                tempNum = this % n;

            if ((n._data[MaxLength - 1] & 0x80000000) != 0)
                n = -n;

            var constant = new BigInteger();
            var i = n._dataLength << 1;
            constant._data[i] = 0x00000001;
            constant._dataLength = i + 1;
            constant = constant / n;
            var totalBits = exp.BitLength;
            var count = 0;

            for (var pos = 0; pos < exp._dataLength; pos++)
            {
                uint mask = 0x01;

                for (var index = 0; index < 32; index++)
                {
                    if ((exp._data[pos] & mask) != 0)
                    {
                        var resultNumTemp = resultNum * tempNum;
                        resultNum = BarrettReduction(resultNumTemp, n, constant);
                    }

                    mask <<= 1;

                    var tempNum2 = tempNum * tempNum;
                    tempNum = BarrettReduction(tempNum2, n, constant);

                    if (tempNum._dataLength == 1 && tempNum._data[0] == 1)
                    {
                        if (thisNegative && (exp._data[0] & 0x1) != 0)
                            return -resultNum;

                        return resultNum;
                    }

                    count++;

                    if (count == totalBits)
                        break;
                }
            }

            if (thisNegative && (exp._data[0] & 0x1) != 0)
                return -resultNum;

            return resultNum;
        }

        private static BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
        {
            Contract.Requires(x != null);
            Contract.Requires(n != null);
            Contract.Requires(constant != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var k = n._dataLength;
            var kPlusOne = k + 1;
            var kMinusOne = k - 1;
            var q1 = new BigInteger();

            for (int i = kMinusOne, j = 0; i < x._dataLength; i++, j++)
                q1._data[j] = x._data[i];

            var q1Len = x._dataLength - kMinusOne;
            if (q1Len <= 0)
                q1Len = 1;

            q1._dataLength = q1Len;

            var q2 = q1 * constant;
            var q3 = new BigInteger();

            for (int i = kPlusOne, j = 0; i < q2._dataLength; i++, j++)
                q3._data[j] = q2._data[i];

            var q3Len = q2._dataLength - kPlusOne;
            if (q3Len <= 0)
                q3Len = 1;

            q3._dataLength = q3Len;

            var r1 = new BigInteger();
            var lengthToCopy = (x._dataLength > kPlusOne) ? kPlusOne : x._dataLength;

            for (var i = 0; i < lengthToCopy; i++)
                r1._data[i] = x._data[i];

            r1._dataLength = lengthToCopy;
            var r2 = new BigInteger();

            for (var i = 0; i < q3._dataLength; i++)
            {
                if (q3._data[i] == 0)
                    continue;

                ulong mcarry = 0;
                var t = i;

                for (var j = 0; j < n._dataLength && t < kPlusOne; j++, t++)
                {
                    var val = (q3._data[i] * (ulong)n._data[j]) + r2._data[t] + mcarry;

                    r2._data[t] = (uint)(val & 0xffffffff);
                    mcarry = (val >> 32);
                }

                if (t < kPlusOne)
                    r2._data[t] = (uint)mcarry;
            }

            r2._dataLength = kPlusOne;

            while (r2._dataLength > 1 && r2._data[r2._dataLength - 1] == 0)
                r2._dataLength--;

            r1 -= r2;

            if ((r1._data[MaxLength - 1] & 0x80000000) != 0)
            {
                var val = new BigInteger();

                val._data[kPlusOne] = 0x00000001;
                val._dataLength = kPlusOne + 1;
                r1 += val;
            }

            while (r1 >= n)
                r1 -= n;

            return r1;
        }

        public BigInteger GreatestCommonDivisor(BigInteger bi)
        {
            Contract.Requires(bi != null);
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            BigInteger x;
            BigInteger y;

            if ((_data[MaxLength - 1] & 0x80000000) != 0)
                x = -this;
            else
                x = this;

            if ((bi._data[MaxLength - 1] & 0x80000000) != 0)
                y = -bi;
            else
                y = bi;

            var g = y;

            while (x._dataLength > 1 || (x._dataLength == 1 && x._data[0] != 0))
            {
                g = x;
                x = y % x;
                y = g;
            }

            return g;
        }

        public void GenerateRandomBits(int bits, Random rand)
        {
            if (rand == null)
                rand = new Random(Environment.TickCount);

            var dwords = bits >> 5;
            var remBits = bits & 0x1f;

            if (remBits != 0)
                dwords++;

            if (dwords > MaxLength)
                throw new ArgumentException("Number of required bits higher than MaxLength.");

            for (var i = 0; i < dwords; i++)
                _data[i] = (uint)(rand.NextDouble() * 0x100000000);

            for (var i = dwords; i < MaxLength; i++)
                _data[i] = 0;

            if (remBits != 0)
            {
                var mask = (uint)(0x01 << (remBits - 1));
                _data[dwords - 1] |= mask;

                mask = 0xffffffff >> (32 - remBits);
                _data[dwords - 1] &= mask;
            }
            else
                _data[dwords - 1] |= 0x80000000;

            _dataLength = dwords;

            if (_dataLength == 0)
                _dataLength = 1;
        }

        public int BitLength
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                var leadingZeros = false;
                if (_dataLength > 1 && _data[_dataLength - 1] == 0)
                {
                    leadingZeros = true;

                    while (_data[_dataLength - 1] == 0)
                        _dataLength--;
                }

                var value = _data[_dataLength - 1];
                var mask = 0x80000000;
                var bits = 32;

                while (bits > 0 && (value & mask) == 0)
                {
                    bits--;
                    mask >>= 1;
                }

                bits += ((_dataLength - 1) << 5);

                if (leadingZeros && (value & mask) != 0)
                    bits++;

                return bits;
            }
        }

        public byte ByteValue
        {
            get { return (byte)_data[0]; }
        }

        public sbyte SByteValue
        {
            get { return (sbyte)_data[0]; }
        }

        public short Int16Value
        {
            get { return (short)_data[0]; }
        }

        public ushort UInt16Value
        {
            get { return (ushort)_data[0]; }
        }

        public int Int32Value
        {
            get { return (int)_data[0]; }
        }

        public uint UInt32Value
        {
            get { return _data[0]; }
        }

        public long Int64Value
        {
            get
            {
                long val = _data[0];

                try
                {
                    val |= (long)_data[1] << 32;
                }
                catch (Exception)
                {
                    if ((_data[0] & 0x80000000) != 0)
                        val = (int)_data[0];
                }

                return val;
            }
        }

        public ulong UInt64Value
        {
            get { return unchecked((ulong)Int64Value); }
        }

        public BigInteger ModInverse(BigInteger modulus)
        {
            Contract.Requires(modulus != null);

            var p = new[] { (BigInteger)0, (BigInteger)1 };
            var r = new[] { (BigInteger)0, (BigInteger)0 };
            var q = new BigInteger[2];

            var step = 0;
            var a = modulus;
            var b = this;

            while (b._dataLength > 1 || (b._dataLength == 1 && b._data[0] != 0))
            {
                var quotient = new BigInteger();
                var remainder = new BigInteger();

                if (step > 1)
                {
                    var q0 = q[0];
                    Contract.Assume(q0 != null);

                    var pval = (p[0] - (p[1] * q0)) % modulus;

                    p[0] = p[1];
                    p[1] = pval;
                }

                if (b._dataLength == 1)
                    SingleByteDivide(a, b, ref quotient, ref remainder);
                else
                    MultiByteDivide(a, b, ref quotient, ref remainder);

                q[0] = q[1];
                r[0] = r[1];
                q[1] = quotient;
                r[1] = remainder;

                a = b;
                b = remainder;

                step++;
            }

            if (r[0]._dataLength > 1 || (r[0]._dataLength == 1 && r[0]._data[0] != 1))
                return null;

            var q02 = q[0];
            Contract.Assume(q02 != null);
            var result = ((p[0] - (p[1] * q02)) % modulus);

            if ((result._data[MaxLength - 1] & 0x80000000) != 0)
                result += modulus;

            return result;
        }

        public byte[] GetBytes()
        {
            Contract.Ensures(Contract.Result<byte[]>() != null);
            Contract.Ensures(Contract.Result<byte[]>().Length == ByteLength);

            var bytes = GetBytes(ByteLength);
            Contract.Assume(bytes.Length == ByteLength);
            return bytes;
        }

        public byte[] GetBytes(int numBytes)
        {
            Contract.Requires(numBytes >= 0);
            Contract.Ensures(Contract.Result<byte[]>() != null);
            Contract.Ensures(Contract.Result<byte[]>().Length == numBytes);

            var result = new byte[numBytes];
            var numBits = BitLength;
            var realNumBytes = numBits >> 3;

            if ((numBits & 0x7) != 0)
                realNumBytes++;

            for (var i = 0; i < realNumBytes; i++)
            {
                for (var b = 0; b < 4; b++)
                {
                    if (i * 4 + b >= realNumBytes)
                        return result;

                    result[i * 4 + b] = (byte)(_data[i] >> (b * 8) & 0xff);
                }
            }

            return result;
        }

        public void SetBit(int bitNum)
        {
            Contract.Requires(bitNum >= 0);

            var bytePos = bitNum >> 5;
            var bitPos = (byte)(bitNum & 0x1f);

            var mask = (uint)1 << bitPos;
            _data[bytePos] |= mask;

            if (bytePos >= _dataLength)
                _dataLength = bytePos + 1;
        }

        public void UnsetBit(int bitNum)
        {
            Contract.Requires(bitNum >= 0);

            var bytePos = bitNum >> 5;

            if (bytePos >= _dataLength)
                return;

            var bitPos = (byte)(bitNum & 0x1f);
            var mask = (uint)1 << bitPos;
            var mask2 = 0xffffffff ^ mask;

            _data[bytePos] &= mask2;

            if (_dataLength > 1 && _data[_dataLength - 1] == 0)
                _dataLength--;
        }

        public BigInteger Sqrt()
        {
            Contract.Ensures(Contract.Result<BigInteger>() != null);

            var numBits = (uint)BitLength;

            if ((numBits & 0x1) != 0)
                numBits = (numBits >> 1) + 1;
            else
                numBits = (numBits >> 1);

            var bytePos = numBits >> 5;
            var bitPos = (byte)(numBits & 0x1f);
            uint mask;
            var result = new BigInteger();

            if (bitPos != 0)
            {
                mask = (uint)1 << bitPos;
                bytePos++;
            }
            else
                mask = 0x80000000;

            result._dataLength = (int)bytePos;

            for (var i = (int)bytePos - 1; i >= 0; i--)
            {
                while (mask != 0)
                {
                    result._data[i] ^= mask;

                    if ((result * result) > this)
                        result._data[i] ^= mask;

                    mask >>= 1;
                }

                mask = 0x80000000;
            }

            return result;
        }

        public uint SignBit
        {
            get { return (_data[MaxLength - 1] & 0x80000000); }
        }

        public int Sign
        {
            get
            {
                var sign = SignBit;

                // If the sign bit is set, the value is negative.
                if (sign != 0)
                    return -1;

                const int pos = MaxLength - 1;

                // Check for zero, since the sign bit being 0 can also indicate the value of zero.
                for (var i = 0; i < _dataLength; i++)
                {
                    if (_data[i] != 0)
                        break;

                    // If all bits and the sign bit are 0, the value is zero.
                    if (i == pos)
                        return 0;
                }

                // The value must be positive.
                return 1;
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BigInteger);
        }

        public bool Equals(BigInteger other)
        {
            if (other == null)
                return false;

            // If the length doesn't equal, we can gain some speed.
            if (_dataLength != other._dataLength)
                return false;

            for (var i = 0; i < _dataLength; i++)
                if (_data[i] != other._data[i])
                    return false;

            return true;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as BigInteger);
        }

        public int CompareTo(BigInteger other)
        {
            if (other == null || this > other)
                return 1;

            if (this < other)
                return -1;

            return 0;
        }

        public override string ToString()
        {
            return "0x" + ToString(16);
        }

        public string ToString(int radix)
        {
            Contract.Requires(radix > 0);
            Contract.Ensures(Contract.Result<string>() != null);

            const string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = string.Empty;
            var a = this;
            var negative = false;

            if ((a._data[MaxLength - 1] & 0x80000000) != 0)
            {
                negative = true;

                try
                {
                    a = -a;
                }
                catch
                {
                }
            }

            var quotient = new BigInteger();
            var remainder = new BigInteger();
            var biRadix = new BigInteger(radix);

            if (a._dataLength == 1 && a._data[0] == 0)
                result = "0";
            else
            {
                while (a._dataLength > 1 || (a._dataLength == 1 && a._data[0] != 0))
                {
                    SingleByteDivide(a, biRadix, ref quotient, ref remainder);

                    if (remainder._data[0] >= 10)
                    {
                        var chr = (int)remainder._data[0] - 10;
                        Contract.Assume(chr < charSet.Length);
                        result = charSet[chr] + result;
                    }
                    else
                        result = remainder._data[0] + result;

                    a = quotient;
                }

                if (negative)
                    result = "-" + result;
            }

            return result;
        }
    }
}
