using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;

using M1xA.Core.IO.Ubjson.Extensions;

namespace M1xA.Core.IO.Ubjson
{
    /// <summary>
    /// Writes data into stream in Ubjson format.
    /// </summary>
    public class UbjsonWriter : Ubjson, IDisposable
    {
        public UbjsonWriter(Stream stream)
            : base(stream)
        {

        }

        /// <summary>
        /// Writes the null value.
        /// </summary>
        public void Write()
        {
            Stream.Write(DataMarker.Null);
        }

        public void Write(bool value)
        {
            Stream.Write(value ? DataMarker.True : DataMarker.False);
        }

        public void Write(byte value)
        {
            Stream.Write(DataMarker.Byte);
            Stream.WriteByte(value);
        }

        public void Write(short value)
        {
            Stream.Write(DataMarker.Int16);
            Stream.Write(value.GetBytes().ReverseIf(InvalidEndiannes));
        }

        public void Write(int value)
        {
            Stream.Write(DataMarker.Int32);
            Stream.Write(value.GetBytes().ReverseIf(InvalidEndiannes));
        }

        public void Write(long value)
        {
            Stream.Write(DataMarker.Int64);
            Stream.Write(value.GetBytes().ReverseIf(InvalidEndiannes));
        }

        public void Write(float value)
        {
            Stream.Write(DataMarker.Float);
            Stream.Write(value.GetBytes().ReverseIf(InvalidEndiannes));
        }

        public void Write(double value)
        {
            Stream.Write(DataMarker.Double);
            Stream.Write(value.GetBytes().ReverseIf(InvalidEndiannes));
        }

        /// <summary>
        /// Writes the passed Huge (number) value or its short version.
        /// </summary>
        /// <param name="value">BigInteger value to write.</param>
        public void Write(BigInteger value)
        {
            DataMarker header = DataMarker.Huge;

            byte[] data = Encoding.GetBytes(value.ToString());

            bool zipped;
            
            byte[] length = GetLengthBytes(data.Length, out zipped);

            if (zipped)
            {
                header = DataMarker.ShortHuge;
            }

            Stream.Write(header);
            Stream.Write(length);
            Stream.Write(data);
        }

        /// <summary>
        /// Writes the passed string value or its short version.
        /// </summary>
        /// <param name="value">String value to write.</param>
        public void Write(string value)
        {
            DataMarker header = DataMarker.String;

            byte[] data = Encoding.GetBytes(value.ToString());

            bool zipped;
            
            byte[] length = GetLengthBytes(data.Length, out zipped);

            if (zipped)
            {
                header = DataMarker.ShortString;
            }

            Stream.Write(header);
            Stream.Write(length);
            Stream.Write(data);
        }

        /// <summary>
        /// Writes header for the passed array or its short version and inspects array items.
        /// </summary>
        /// <param name="value">Flat array or container.</param>
        public void Write(Array value)
        {
            DataMarker header = DataMarker.Array;

            Array array = value;

            bool zipped;
            
            byte[] length = GetLengthBytes(array.Length, out zipped);

            if (zipped)
            {
                header = DataMarker.ShortArray;
            }

            Stream.Write(header);
            Stream.Write(length);

            foreach (object item in array)
            {
                Write(item);
            }
        }

        /// <summary>
        /// Writes header for the passed object or its short version and inspects public members (fields and/or properties) of object.
        /// </summary>
        /// <param name="value">Object to inspect and to write.</param>
        public void Write(object o)
        {
            DataMarker marker = o.GetMarker();

            switch (marker)
            {
                case DataMarker.Null: Write(); break;
                case DataMarker.True: Write((bool)o); break;
                case DataMarker.False: Write((bool)o); break;
                case DataMarker.Byte: Write((byte)o); break;
                case DataMarker.Int16: Write((short)o); break;
                case DataMarker.Int32: Write((int)o); break;
                case DataMarker.Int64: Write((long)o); break;
                case DataMarker.Float: Write((float)o); break;
                case DataMarker.Double: Write((double)o); break;
                case DataMarker.Huge: Write((BigInteger)o); break;
                case DataMarker.String: Write(o.ToString()); break;
                case DataMarker.Array: Write(o as Array); break;

                case DataMarker.Object:
                    {
                        DataMarker header = marker;

                        if (o is IDictionary<string, object>) // Added for dynamic/ExpandoObject.
                        {
                            Dictionary<string, object> data = (o as IDictionary<string, object>).Purge();

                            bool zipped;

                            byte[] length = GetLengthBytes(data.Count, out zipped);

                            if (zipped)
                            {
                                header = DataMarker.ShortObject;
                            }

                            Stream.Write(header);
                            Stream.Write(length);

                            foreach (KeyValuePair<string, object> kv in data)
                            {
                                Write(kv.Key);
                                Write(kv.Value);
                            }
                        }
                        else // Standard case
                        {
                            Type type = o.GetType();

                            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public).Purge(o);
                            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Purge(o);

                            bool zipped;

                            byte[] length = GetLengthBytes(fields.Length + properties.Length, out zipped);

                            if (zipped)
                            {
                                header = DataMarker.ShortObject;
                            }

                            Stream.Write(header);
                            Stream.Write(length);

                            foreach (FieldInfo field in fields)
                            {
                                Write(field.Name);
                                Write(field.GetValue(o));
                            }

                            foreach (PropertyInfo property in properties)
                            {
                                Write(property.Name);
                                Write(property.GetValue(o, null));
                            }
                        }
                    }
                    break;

                default: goto case DataMarker.Object;
            }
        }

        /// <summary>
        /// Writes the header info of array that has an unknown length.
        /// </summary>
        public void BeginWriteArray()
        {
            Stream.Write(DataMarker.ShortArray);
            Stream.WriteByte(UnknownLength);
        }

        /// <summary>
        /// Writes the header info of object that has an unknown count of items.
        /// </summary>
        public void BeginWriteObject()
        {
            Stream.Write(DataMarker.ShortObject);
            Stream.WriteByte(UnknownLength);
        }

        /// <summary>
        /// Ends writing of object or array. 
        /// Must be called after BeginWriteArray or BeginWriteObject methods for consistency and data integrity.
        /// </summary>
        public void EndWrite()
        {
            Stream.Write(DataMarker.End);
        }

        /// <summary>
        ///  Writes the wait code (NoOp data marker). 
        ///  Added for streaming and network purposes.
        /// </summary>
        public void KeepAlive()
        {
            Stream.Write(DataMarker.NoOp);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stream.Flush();
            }
        }

        /// <summary>
        /// Converts int (32 bit integer) value into byte array according to endianness of host machine.
        /// </summary>
        /// <param name="length">Length or items counter of the inspected object.</param>
        /// <param name="zipped">True if result was zipped.</param>
        /// <returns>Byte array that corresponds to passed value.</returns>
        protected byte[] GetLengthBytes(int length, out bool zipped)
        {
            byte[] result;

            if (length <= ShortLength)
            {
                zipped = true;
                result = new byte[] { (byte)length };
            }
            else
            {
                zipped = false;
                result = length.GetBytes().ReverseIf(InvalidEndiannes);
            }

            return result;
        }
    }
}
