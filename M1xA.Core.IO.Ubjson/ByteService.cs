// 
// ByteService.cs
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
    public abstract class ByteService
    {
        public abstract byte[] GetBytes(short value);
        public abstract byte[] GetBytes(int value);
        public abstract byte[] GetBytes(long value);
        public abstract byte[] GetBytes(float value);
        public abstract byte[] GetBytes(double value);
        public abstract short GetInt16(byte[] bytes);
        public abstract int GetInt32(byte[] bytes);
        public abstract long GetInt64(byte[] bytes);
        public abstract float GetFloat(byte[] bytes);
        public abstract double GetDouble(byte[] bytes);
    }
}
