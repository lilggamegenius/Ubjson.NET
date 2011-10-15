// 
// UbjsonWriter.cs
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
using System.Collections;
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
        private ObjectService _os;

        public UbjsonWriter(Stream stream)
            : base(stream)
        {
            _os = new SimpleObjectService();

            _os.AddIgnorable(typeof(Delegate));
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
            Stream.Write(ByteService.GetBytes(value));
        }

        public void Write(int value)
        {
            Stream.Write(DataMarker.Int32);
            Stream.Write(ByteService.GetBytes(value));
        }

        public void Write(long value)
        {
            Stream.Write(DataMarker.Int64);
            Stream.Write(ByteService.GetBytes(value));
        }

        public void Write(float value)
        {
            Stream.Write(DataMarker.Float);
            Stream.Write(ByteService.GetBytes(value));
        }

        public void Write(double value)
        {
            Stream.Write(DataMarker.Double);
            Stream.Write(ByteService.GetBytes(value));
        }

        /// <summary>
        /// Writes the passed Huge (number) value or its short version.
        /// </summary>
        /// <param name="value">BigInteger value to write.</param>
        public void Write(BigInteger value)
        {
            WriteAsString(value.ToString(), DataMarker.ShortHuge, DataMarker.Huge);
        }

        /// <summary>
        /// Writes the passed Huge (number) value or its short version.
        /// </summary>
        /// <param name="value">Decimal value to write.</param>
        public void Write(decimal value)
        {
            WriteAsString(value.ToString(), DataMarker.ShortHuge, DataMarker.Huge);
        }

        /// <summary>
        /// Writes the passed string value or its short version.
        /// </summary>
        /// <param name="value">String value to write.</param>
        public void Write(string value)
        {
            WriteAsString(value, DataMarker.ShortString, DataMarker.String);
        }

        /// <summary>
        /// Writes header for the passed array or its short version and inspects array items.
        /// </summary>
        /// <param name="value">Flat array or container.</param>
        public void Write(IList value)
        {
            WriteArrayHeader(value.Count);

            foreach (object item in value)
                Write(item);
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
                case DataMarker.Huge: WriteAsString(o.ToString(), DataMarker.ShortHuge, DataMarker.Huge); break;
                case DataMarker.String: WriteAsString(o.ToString(), DataMarker.ShortString, DataMarker.String); break;
                case DataMarker.Array: Write(o as IList); break;

                case DataMarker.Object:
                    {
                        if (o is IDictionary<string, object>) // Added for dynamic/ExpandoObject.
                        {
                            Dictionary<string, object> members = _os.GetSerializableMembers(o as IDictionary<string, object>);

                            WriteObjectHeader(members.Count);

                            foreach (KeyValuePair<string, object> kv in members)
                            {
                                Write(kv.Key);
                                Write(kv.Value);
                            }
                        }
                        else if (o is IDictionary) // All other dictionaries
                        {
                            Dictionary<object, object> members = _os.GetSerializableMembers(o as IDictionary);

                            WriteObjectHeader(members.Count);

                            foreach (KeyValuePair<object, object> kv in members)
                            {
                                Write(kv.Key);
                                Write(kv.Value);
                            }
                        }
                        else // Standard case
                        {
                            MemberInfo[] members = _os.GetSerializableMembers(o);

                            WriteObjectHeader(members.Length);

                            foreach (MemberInfo mi in members)
                            {
                                if (mi is FieldInfo)
                                {
                                    FieldInfo fi = mi as FieldInfo;

                                    Write(fi.Name);
                                    Write(fi.GetValue(o));
                                }

                                if (mi is PropertyInfo)
                                {
                                    PropertyInfo pi = mi as PropertyInfo;

                                    Write(pi.Name);
                                    Write(pi.GetValue(o, null));
                                }
                            }
                        }
                    }
                    break;

                case DataMarker.Unknown: throw new UbjsonException("Unknown data type to write.");
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
        /// Writes only the short or normal header for array depending on its length.
        /// </summary>
        /// <param name="count">Item count, its length.</param>
        public void WriteArrayHeader(int count)
        {
            if (count <= ShortLength)
            {
                Stream.Write(DataMarker.ShortArray);
                Stream.WriteByte((byte)count);
            }
            else
            {
                Stream.Write(DataMarker.Array);
                Stream.Write(ByteService.GetBytes(count));
            }
        }

        /// <summary>
        /// Writes only the short or normal header for object depending on item count.
        /// </summary>
        /// <param name="count">Item count.</param>
        public void WriteObjectHeader(int count)
        {
            if (count <= ShortLength)
            {
                Stream.Write(DataMarker.ShortObject);
                Stream.WriteByte((byte)count);
            }
            else
            {
                Stream.Write(DataMarker.Object);
                Stream.Write(ByteService.GetBytes(count));
            }
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
                Stream.Flush();
        }

        private void WriteAsString(string value, DataMarker shortObject, DataMarker normalObject)
        {
            byte[] data = Encoding.GetBytes(value);

            if (data.Length <= ShortLength)
            {
                Stream.Write(shortObject);
                Stream.WriteByte((byte)data.Length);
            }
            else
            {
                Stream.Write(normalObject);
                Stream.Write(ByteService.GetBytes(data.Length));
            }

            Stream.Write(data);
        }
    }
}
