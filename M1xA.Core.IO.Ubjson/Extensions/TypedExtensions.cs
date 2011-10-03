using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace M1xA.Core.IO.Ubjson.Extensions
{
    public static class ObjectTypeExtension
    {
        public static DataMarker GetMarker(this object value)
        {
            if (value == null)
                return DataMarker.Null;

            if (value is ValueType)
            {
                if (value is bool)
                    return (bool)value == true ? DataMarker.True : DataMarker.False;

                if (value is byte)
                    return DataMarker.Byte;

                if (value is int)
                    return DataMarker.Int32;

                if (value is long)
                    return DataMarker.Int64;

                if (value is double)
                    return DataMarker.Double;

                if (value is BigInteger)
                    return DataMarker.Huge;

                return DataMarker.Unknown;
            }

            if (value is string)
                return DataMarker.String;

            if (value is Array)
                return DataMarker.Array;

            return DataMarker.Object;
        }
    }

    public static class ByteArrayTypeExtension
    {
        public static byte[] ReverseIf(this byte[] value, Func<bool> predicate)
        {
            return predicate() ? value.Reverse().ToArray() : value;
        }
    }

    public static class PrimitiveTypeExtension
    {
        public static bool IsHeader(this byte value, ref DataMarker header)
        {
            if (Enum.IsDefined(typeof(DataMarker), value))
            {
                header = (DataMarker)value;

                return true;
            }

            return false;
        }

        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this double value)
        {
            return BitConverter.GetBytes(value);
        }
    }

    public static class StreamExtension
    {
        public static bool Read(this Stream stream, ref byte value)
        {
            int data = stream.ReadByte();

            if (data == -1)
                return false;

            value = (byte)data;
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
}
