using System;

namespace M1xA.Core.IO.Ubjson
{
    public class IrregularEndOfStreamException : UbjsonException
    {
        public IrregularEndOfStreamException(string message = null, Exception inner = null)
            : base(message, inner)
        {

        }
    }
}
