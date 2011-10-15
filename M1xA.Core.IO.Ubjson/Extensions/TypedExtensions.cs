// 
// TypedExtensions.cs
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
using System.Dynamic;
using System.IO;
using System.Numerics;

namespace M1xA.Core.IO.Ubjson.Extensions
{
    internal static class ObjectTypeExtension
    {
        public static DataMarker GetMarker(this object value)
        {
            if (value == null)
                return DataMarker.Null;

            if (value is ValueType)
            {
                if (value is bool)
                    return (bool)value ? DataMarker.True : DataMarker.False;

                if (value is byte)
                    return DataMarker.Byte;

                if (value is short)
                    return DataMarker.Int16;

                if (value is int)
                    return DataMarker.Int32;

                if (value is long)
                    return DataMarker.Int64;

                if (value is float)
                    return DataMarker.Float;

                if (value is double)
                    return DataMarker.Double;

                if (value is decimal)
                    return DataMarker.Huge;

                if (value is BigInteger)
                    return DataMarker.Huge;

                return DataMarker.Unknown;
            }

            if (value is string)
                return DataMarker.String;

            if (value is IList)
                return DataMarker.Array;

            return DataMarker.Object;
        }
    }

    internal static class PrimitiveTypeExtension
    {
        public static bool GetMarker(this byte value, out DataMarker header)
        {
            if (Enum.IsDefined(typeof(DataMarker), value))
            {
                header = (DataMarker)value;
                return true;
            }

            header = DataMarker.Unknown;
            return false;
        }
    }

    internal static class StreamExtension
    {
        /// <summary>
        /// Reads and return only one byte from stream.
        /// </summary>
        /// <param name="stream">Underlying stream.</param>
        /// <param name="result">Contains readed byte.</param>
        /// <returns>False when end of stream reached.</returns>
        public static bool Read(this Stream stream, out byte result)
        {
            int data = stream.ReadByte();

            if (data != -1)
            {
                result = (byte)data;
                return true;
            }
            else
            {
                result = default(byte);
                return false;
            }
        }

        public static bool Read(this Stream stream, byte[] result, int count)
        {
            int available = stream.Read(result, 0, count);

            if (available == 0 || available < count)
                return false;

            return true;
        }

        public static void Write(this Stream output, DataMarker marker)
        {
            output.WriteByte((byte)marker);
        }

        public static void Write(this Stream output, byte[] data)
        {
            output.Write(data, 0, data.Length);
        }
    }

    public static class IDictionaryExtension
    {
        /// <summary>
        /// Materializes the dynamic object/expando object from the dictionary.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <param name="check">Verifying an object inherits the IDictionary.</param>
        /// <returns>Dynamic object.</returns>
        public static dynamic AsDynamic(this object value, bool check = false)
        {
            if (value is IDictionary<string, object>)
            {
                ExpandoObject expando = new ExpandoObject();

                IDictionary<string, object> o = value as IDictionary<string, object>;
                IDictionary<string, object> e = expando as IDictionary<string, object>;

                foreach (KeyValuePair<string, object> entry in o)
                {
                    if (entry.Value is IDictionary<string, object>)
                    {
                        e.Add(entry.Key, entry.Value.AsDynamic());
                    }
                    else if (entry.Value is IList)
                    {
                        IList array = entry.Value as IList;

                        dynamic[] items = new dynamic[array.Count];

                        for (int i = 0; i < array.Count; i++)
                        {
                            items[i] = array[i].AsDynamic();
                        }

                        e.Add(entry.Key, items);
                    }
                    else
                    {
                        e.Add(entry);
                    }
                }

                return expando;
            }
            else if (!check)
            {
                return value;
            }

            throw new UbjsonException("Can't cast an object to dynamic/expando object.");
        }
    }
}
