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
            /// TODO Read Write short, 16 bit integer.
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
            byte[] data = Encoding.GetBytes(value.ToString());

            bool zipped;

            DataMarker header = DataMarker.Huge;
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
            byte[] data = Encoding.GetBytes(value.ToString());

            bool zipped;

            DataMarker header = DataMarker.String;
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
            Array array = value as Array;

            bool zipped;

            DataMarker header = DataMarker.Array;
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
                case DataMarker.Int32: Write((int)o); break;
                case DataMarker.Int64: Write((long)o); break;
                case DataMarker.Double: Write((double)o); break;
                case DataMarker.Huge: Write((BigInteger)o); break;
                case DataMarker.String: Write(o.ToString()); break;
                case DataMarker.Array: Write(o as Array); break;

                case DataMarker.Object:
                    {
                        if (o is IDictionary<string, object>) // Added for dynamic/ExpandoObject.
                        {
                            Dictionary<string, object> dyno = new Dictionary<string, object>();
                            IDictionary<string, object> source = o as IDictionary<string, object>;

                            foreach (KeyValuePair<string, object> kv in source)
                            {
                                if (kv.Value is Delegate) continue; // Skipping all function pointers.

                                dyno.Add(kv.Key, kv.Value);
                            }

                            bool zipped;

                            DataMarker header = marker;
                            byte[] length = GetLengthBytes(dyno.Count, out zipped);

                            if (zipped)
                            {
                                header = DataMarker.ShortObject;
                            }

                            Stream.Write(header);
                            Stream.Write(length);

                            foreach (KeyValuePair<string, object> kv in dyno)
                            {
                                Write(kv.Key);
                                Write(kv.Value);
                            }
                        }
                        else // Standard case
                        {
                            Type type = o.GetType();

                            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                            bool zipped;

                            DataMarker header = marker;
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
    }
}
