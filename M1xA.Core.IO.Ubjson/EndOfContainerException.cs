using System;

namespace M1xA.Core.IO.Ubjson
{
    internal class EndOfContainerException : ApplicationException
    {
        public EndOfContainerException(string message = null, Exception inner = null)
            : base(message, inner)
        {

        }
    }
}
