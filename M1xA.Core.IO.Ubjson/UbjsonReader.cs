using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using M1xA.Core.IO.Ubjson.Extensions;

namespace M1xA.Core.IO.Ubjson
{
    /// <summary>
    /// Reads data from the Ubjson-formated stream.
    /// </summary>
    public class UbjsonReader : Ubjson
    {
        public UbjsonReader(Stream stream)
            : base(stream)
        {

        }

        /// <summary>
        /// Parses the only one item from Ubjson-enabled stream.
        /// </summary>
        /// <returns>The result value is the value of primitive or container type. Container can be an object or array.</returns>
        public object Parse()
        {
            DataMarker header;
            byte bits = 0;

            while (Stream.Read(ref bits))
            {
                header = DataMarker.Unknown;

                if (bits.IsHeader(ref header))
                {
                    switch (header)
                    {
                        case DataMarker.Null: return null;
                        case DataMarker.True: return true;
                        case DataMarker.False: return false;
                        case DataMarker.Byte: return GetRawByte();
                        case DataMarker.Int32: return GetRawInt32();
                        case DataMarker.Int64: return GetRawInt64();
                        case DataMarker.Double: return GetRawDouble();
                        case DataMarker.ShortHuge: return GetRawBigInteger(true);
                        case DataMarker.Huge: return GetRawBigInteger();
                        case DataMarker.ShortString: return GetRawString(true);
                        case DataMarker.String: return GetRawString();
                        case DataMarker.ShortArray: return GetRawArray(true);
                        case DataMarker.Array: return GetRawArray();
                        case DataMarker.ShortObject: return GetRawObject(true);
                        case DataMarker.Object: return GetRawObject();
                        case DataMarker.NoOp: continue;
                        case DataMarker.End: throw new EndOfContainerException();
                        default: throw new NotImplementedException(string.Format("Reading of {0} data type is not implemented.", header.ToString()));
                    }
                }

                throw new InvalidDataException(string.Format("Invalid marker is readed. Value: 0x{0:x}.", bits));
            }

            throw new EndOfStreamException();
        }

        protected byte GetRawByte()
        {
            byte result = 0;

            Stream.Read(ref result);

            return result;
        }

        protected int GetRawInt32()
        {
            byte[] data = new byte[sizeof(int)];

            Stream.Read(data, 0, data.Length);

            return BitConverter.ToInt32(data.ReverseIf(InvalidEndiannes).ToArray(), 0);
        }

        protected long GetRawInt64()
        {
            byte[] data = new byte[sizeof(long)];

            Stream.Read(data, 0, data.Length);

            return BitConverter.ToInt64(data.ReverseIf(InvalidEndiannes).ToArray(), 0);
        }

        protected double GetRawDouble()
        {
            byte[] data = new byte[sizeof(double)];

            Stream.Read(data, 0, data.Length);

            return BitConverter.ToDouble(data.ReverseIf(InvalidEndiannes).ToArray(), 0);
        }

        protected BigInteger GetRawBigInteger(bool zipped = false)
        {
            return BigInteger.Parse(GetRawString(zipped));
        }

        protected string GetRawString(bool zipped = false)
        {
            int byteCount = zipped ? GetRawByte() : GetRawInt32();

            byte[] stringBytes = new byte[byteCount];

            Stream.Read(stringBytes, 0, byteCount);

            return Encoding.GetString(stringBytes, 0, byteCount);
        }

        protected object[] GetRawArray(bool zipped = false)
        {
            int itemCount = zipped ? GetRawByte() : GetRawInt32();

            List<object> result = new List<object>(itemCount);

            if (itemCount != UnknownLength)
            {
                for (int i = 0; i < itemCount; i++)
                    result[i] = Parse();
            }
            else
            {
                try
                {
                    while (true)
                        result.Add(Parse());
                }
                catch (EndOfContainerException ex) { }
            }

            return result.ToArray();
        }

        protected Dictionary<string, object> GetRawObject(bool zipped = false)
        {
            int itemCount = zipped ? GetRawByte() : GetRawInt32();

            Dictionary<string, object> result = new Dictionary<string, object>(itemCount);

            if (itemCount != UnknownLength)
            {
                for (int i = 0; i < itemCount; i++)
                    result.Add(Parse().ToString(), Parse());
            }
            else
            {
                try
                {
                    while (true)
                        result.Add(Parse().ToString(), Parse());
                }
                catch (EndOfContainerException ex) { }
            }

            return result;
        }
    }
}
