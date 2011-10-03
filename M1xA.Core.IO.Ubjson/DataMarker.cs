using System;

namespace M1xA.Core.IO.Ubjson
{
    /// <summary>
    /// The markers of types supported by the Universal Binary JSON specification.
    /// For details, see <a href="http://ubjson.org/">Universal Binary JSON Specification</a>.
    /// </summary>
    public enum DataMarker : byte
    {
        /// <summary>
        /// Unknown or invalid type.
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// Z - null, default value for reference type.
        /// </summary>
        Null = 0x5A,

        /// <summary>
        /// T - true, bool value.
        /// </summary>
        True = 0x54,

        /// <summary>
        /// F - false, bool value.
        /// </summary>
        False = 0x46,

        /// <summary>
        /// B - byte value.
        /// </summary>
        Byte = 0x42,

        /// <summary>
        /// I - Int32, int, 32 bit integer value.
        /// </summary>
        Int32 = 0x49,

        /// <summary>
        /// L - Int64, long, 64 bit integer value.
        /// </summary>
        Int64 = 0x4C,

        /// <summary>
        /// D - double, 64 bit double-precision float point value.
        /// </summary>
        Double = 0x44,

        /// <summary>
        /// h - short HugeNumber, that has length of string representation less than 255 bytes.
        /// </summary>
        ShortHuge = 0x68,

        /// <summary>
        /// H - HugeNumber, BigInteger value. Represented as string.
        /// </summary>
        Huge = 0x48,

        /// <summary>
        /// s - short string with length less than 255 bytes.
        /// </summary>
        ShortString = 0x73,

        /// <summary>
        /// S - string, reference type.
        /// </summary>
        String = 0x53,

        /// <summary>
        /// a - short flat array with length less than 255 items.
        /// </summary>
        ShortArray = 0x61,

        /// <summary>
        /// A - flat array or container, reference type.
        /// </summary>
        Array = 0x41,

        /// <summary>
        /// o - object that has less than 255 items.
        /// </summary>
        ShortObject = 0x6F,

        /// <summary>
        /// O - object, reference type that has public properties or fields. Stored as key/value pairs.
        /// </summary>
        Object = 0x4F,

        /// <summary>
        /// N - no operation. For streaming purposes, works like keep-alive signal from the server to the client.
        /// </summary>
        NoOp = 0x4E,

        /// <summary>
        /// E - the end of unknown-length containers, arrays and objects.
        /// </summary>
        End = 0x45
    }
}
