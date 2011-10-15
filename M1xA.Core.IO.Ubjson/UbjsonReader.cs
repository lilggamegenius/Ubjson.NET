// 
// UbjsonReader.cs
//  
// Author:
//       M1xA <dev@m1xa.com>
// 
// Copyright (c) 2011 M1xA LLC. All Rights Reserved.
// 
// THE SOFTWARE IS PROVIDED "AS IS" UNDER THE MICROSOFT PUBLIC LICENCE.
// FOR DETAILS, SEE "Ms-PL.txt".
// 
using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// Opens the read-only stream with data from specified array and initializes the UBJSON Reader.
        /// </summary>
        /// <param name="data">Well formatted UBJSON objects.</param>
        public UbjsonReader(byte[] data)
            : this(new MemoryStream(data))
        {

        }

        public UbjsonReader(Stream stream)
            : base(stream)
        {

        }

        public bool EndOfStream { get { return Stream.Position == Stream.Length; } }

        public object ReadNull()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && marker.Equals(DataMarker.Null))
                return null;

            throw new InvalidMarkerException(bits);
        }

        public bool ReadBool()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && (marker.Equals(DataMarker.True) || marker.Equals(DataMarker.False)))
                return marker.Equals(DataMarker.True);

            throw new InvalidMarkerException(bits);
        }

        public byte ReadByte()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && marker.Equals(DataMarker.Byte))
                return GetRawByte();

            throw new InvalidMarkerException(bits);
        }

        public short ReadInt16()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && marker.Equals(DataMarker.Int16))
                return GetRawInt16();

            throw new InvalidMarkerException(bits);
        }

        public int ReadInt32()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && marker.Equals(DataMarker.Int32))
                return GetRawInt32();

            throw new InvalidMarkerException(bits);
        }

        public long ReadInt64()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && marker.Equals(DataMarker.Int64))
                return GetRawInt64();

            throw new InvalidMarkerException(bits);
        }

        public float ReadFloat()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && marker.Equals(DataMarker.Float))
                return GetRawFloat();

            throw new InvalidMarkerException(bits);
        }

        public double ReadDouble()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && marker.Equals(DataMarker.Double))
                return GetRawDouble();

            throw new InvalidMarkerException(bits);
        }

        public BigInteger ReadHuge()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && (marker.Equals(DataMarker.Huge) || marker.Equals(DataMarker.ShortHuge)))
                return GetRawBigInteger(marker.Equals(DataMarker.ShortHuge));

            throw new InvalidMarkerException(bits);
        }

        public string ReadString()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && (marker.Equals(DataMarker.String) || marker.Equals(DataMarker.ShortString)))
                return GetRawString(marker.Equals(DataMarker.ShortString));

            throw new InvalidMarkerException(bits);
        }

        public object[] ReadArray()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && (marker.Equals(DataMarker.Array) || marker.Equals(DataMarker.ShortArray)))
                return GetRawArray(marker.Equals(DataMarker.ShortArray));

            throw new InvalidMarkerException(bits);
        }

        public object ReadObject()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && (marker.Equals(DataMarker.Object) || marker.Equals(DataMarker.ShortObject)))
                return GetRawObject(marker.Equals(DataMarker.ShortObject));

            throw new InvalidMarkerException(bits);
        }

        /// <summary>
        /// Reads the array header.
        /// </summary>
        /// <returns>Array length.</returns>
        public int ReadArrayHeader()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && (marker.Equals(DataMarker.Array) || marker.Equals(DataMarker.ShortArray)))
                return marker == DataMarker.ShortArray ? GetRawByte() : GetRawInt32();

            throw new InvalidMarkerException(bits);
        }

        /// <summary>
        /// Reads the object header.
        /// </summary>
        /// <returns>Item count.</returns>
        public int ReadObjectHeader()
        {
            DataMarker marker;

            byte bits = GetRawByte();

            if (bits.GetMarker(out marker) && (marker.Equals(DataMarker.Object) || marker.Equals(DataMarker.ShortObject)))
                return marker == DataMarker.ShortObject ? GetRawByte() : GetRawInt32();

            throw new InvalidMarkerException(bits);
        }

        public bool TryReadNull(ref object result)
        {
            try
            {
                result = ReadNull();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadByte(ref byte result)
        {
            try
            {
                result = ReadByte();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadInt16(ref short result)
        {
            try
            {
                result = ReadInt16();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadInt32(ref int result)
        {
            try
            {
                result = ReadInt32();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadInt64(ref long result)
        {
            try
            {
                result = ReadInt64();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadFloat(ref float result)
        {
            try
            {
                result = ReadFloat();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadDouble(ref double result)
        {
            try
            {
                result = ReadDouble();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadHuge(ref BigInteger result)
        {
            try
            {
                result = ReadHuge();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadString(ref string result)
        {
            try
            {
                result = ReadString();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadArray(ref object[] result)
        {
            try
            {
                result = ReadArray();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadObject(ref object result)
        {
            try
            {
                result = ReadObject();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadArrayHeader(ref int length)
        {
            try
            {
                length = ReadArrayHeader();
                return true;
            }
            catch { return false; }
        }

        public bool TryReadObjectHeader(ref int itemCount)
        {
            try
            {
                itemCount = ReadObjectHeader();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Utility method that automatically detects type 
        /// and parses the only one its item from Ubjson-enabled stream.
        /// All tree elements of object will be parsed.
        /// </summary>
        /// <returns>The result value is the value of primitive or container type. Container can be an object or array.</returns>
        public object Parse()
        {
            DataMarker header;
            byte bits;

            while (Stream.Read(out bits))
            {
                if (bits.GetMarker(out header))
                {
                    switch (header)
                    {
                        case DataMarker.Null: return null;
                        case DataMarker.True: return true;
                        case DataMarker.False: return false;
                        case DataMarker.Byte: return GetRawByte();
                        case DataMarker.Int16: return GetRawInt16();
                        case DataMarker.Int32: return GetRawInt32();
                        case DataMarker.Int64: return GetRawInt64();
                        case DataMarker.Float: return GetRawFloat();
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
                        default: throw new NotImplementedException(string.Format("Reading of the {0} data type is not implemented.", header.ToString()));
                    }
                }

                throw new InvalidMarkerException(bits);
            }

            throw new IrregularEndOfStreamException();
        }        

        protected byte GetRawByte()
        {
            byte result;

            if (Stream.Read(out result))
                return result;

            throw new IrregularEndOfStreamException();
        }

        protected short GetRawInt16()
        {
            byte[] data = new byte[sizeof(short)];

            if (Stream.Read(data, data.Length))
                return ByteService.GetInt16(data);

            throw new IrregularEndOfStreamException();
        }

        protected int GetRawInt32()
        {
            byte[] data = new byte[sizeof(int)];

            if (Stream.Read(data, data.Length))
                return ByteService.GetInt32(data);

            throw new IrregularEndOfStreamException();
        }

        protected long GetRawInt64()
        {
            byte[] data = new byte[sizeof(long)];

            if (Stream.Read(data, data.Length))
                return ByteService.GetInt64(data);

            throw new IrregularEndOfStreamException();
        }

        protected float GetRawFloat()
        {
            byte[] data = new byte[sizeof(float)];

            if (Stream.Read(data, data.Length))
                return ByteService.GetFloat(data);

            throw new IrregularEndOfStreamException();
        }

        protected double GetRawDouble()
        {
            byte[] data = new byte[sizeof(double)];

            if (Stream.Read(data, data.Length))
                return ByteService.GetDouble(data);

            throw new IrregularEndOfStreamException();
        }

        protected BigInteger GetRawBigInteger(bool zipped = false)
        {
            return BigInteger.Parse(GetRawString(zipped));
        }

        protected string GetRawString(bool zipped = false)
        {
            int byteCount = zipped ? GetRawByte() : GetRawInt32();

            byte[] data = new byte[byteCount];

            if (Stream.Read(data, byteCount))
                return Encoding.GetString(data);

            throw new IrregularEndOfStreamException();
        }

        protected object[] GetRawArray(bool zipped = false)
        {
            int itemCount = zipped ? GetRawByte() : GetRawInt32();

            List<object> result = new List<object>(itemCount);

            if (itemCount != UnknownLength)
            {
                for (int i = 0; i < itemCount; i++)
                    result.Add(Parse());
            }
            else
            {
                try
                {
                    while (true)
                        result.Add(Parse());
                }
                catch (EndOfContainerException) { }
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
                catch (EndOfContainerException) { }
            }

            return result;
        }
    }
}
