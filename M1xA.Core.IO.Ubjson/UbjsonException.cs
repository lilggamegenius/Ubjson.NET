using System;

namespace M1xA.Core.IO.Ubjson
{
    public class UbjsonException : ApplicationException
    {
        public UbjsonException(string message = null, Exception inner = null)
            : base(message, inner)
        {

        }
    }
}
