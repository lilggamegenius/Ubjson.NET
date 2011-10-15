// 
// EndOfContainerException.cs
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
