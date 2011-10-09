using System;
using System.IO;
using System.Text;

namespace M1xA.Core.IO.Ubjson
{
    public enum Endianness : byte { Unknown, Big, Little, Middle }

    public abstract class Ubjson
    {
        public const byte ShortLength = 0xFE;
        public const byte UnknownLength = 0xFF;

        public readonly Encoding DefaultEncoding = Encoding.UTF8;

        protected Func<bool> InvalidEndiannes;

        public Ubjson(Stream stream)
        {
            Encoding = DefaultEncoding;
            Endianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
            InvalidEndiannes = () => Endianness == Endianness.Little;

            Stream = stream;
        }

        /// <summary>
        /// Underlying stream.
        /// </summary>
        public Stream Stream { get; private set; }
        public Encoding Encoding { get; private set; }
        public Endianness Endianness { get; private set; }
    }
}
