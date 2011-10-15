// 
// PrimitiveTypeBytes.cs
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
using System.Runtime.InteropServices;

namespace M1xA.Core.IO.Ubjson
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct ShortBytes
    {
        [FieldOffset(0)]
        public short Short;

        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct IntBytes
    {
        [FieldOffset(0)]
        public int Int;

        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;

        [FieldOffset(2)]
        public byte Byte2;

        [FieldOffset(3)]
        public byte Byte3;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct LongBytes
    {
        [FieldOffset(0)]
        public long Long;

        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;

        [FieldOffset(2)]
        public byte Byte2;

        [FieldOffset(3)]
        public byte Byte3;

        [FieldOffset(4)]
        public byte Byte4;

        [FieldOffset(5)]
        public byte Byte5;

        [FieldOffset(6)]
        public byte Byte6;

        [FieldOffset(7)]
        public byte Byte7;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct FloatBytes
    {
        [FieldOffset(0)]
        public float Float;
        
        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;

        [FieldOffset(2)]
        public byte Byte2;

        [FieldOffset(3)]
        public byte Byte3;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DoubleBytes
    {
        [FieldOffset(0)]
        public double Double;

        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;

        [FieldOffset(2)]
        public byte Byte2;

        [FieldOffset(3)]
        public byte Byte3;

        [FieldOffset(4)]
        public byte Byte4;

        [FieldOffset(5)]
        public byte Byte5;

        [FieldOffset(6)]
        public byte Byte6;

        [FieldOffset(7)]
        public byte Byte7;
    }
}
