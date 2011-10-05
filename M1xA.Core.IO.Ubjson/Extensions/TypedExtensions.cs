using System;
using System.IO;
using System.Linq;
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

    internal static class ByteArrayTypeExtension
    {
        public static byte[] ReverseIf(this byte[] value, Func<bool> predicate)
        {
            return predicate() ? value.Reverse().ToArray() : value;
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

        public static byte[] GetBytes(this short value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this float value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this double value)
        {
            return BitConverter.GetBytes(value);
        }
    }

    internal static class StreamExtension
    {
        /// <summary>
        /// Reads and return only one byte from stream.
        /// </summary>
        /// <param name="stream">Underlying stream.</param>
        /// <param name="value">Contains readed byte.</param>
        /// <returns>False when end of stream reached.</returns>
        public static bool Read(this Stream stream, ref byte result)
        {
            int data = stream.ReadByte();

            if (data != -1)
            {
                result = (byte)data;
                return true;
            }

            return false;
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
}
