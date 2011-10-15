// 
// InvalidMarkerException.cs
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
    public class InvalidMarkerException: UbjsonException
    {
        public InvalidMarkerException(byte invalid, Exception inner = null)
            : base(string.Format("Was read an invalid marker. Value: 0x{0:x}.", invalid), inner)
        {

        }

        public InvalidMarkerException(string message = null, Exception inner = null)
            : base(message, inner)
        {

        }
    }
}
