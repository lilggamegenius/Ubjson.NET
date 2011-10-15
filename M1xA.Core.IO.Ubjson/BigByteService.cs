// 
// BigByteService.cs
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
    public class BigByteService : ByteService
    {
        public override byte[] GetBytes(short value)
        {
            byte[] result = new byte[sizeof(short)];
            ShortBytes primitive = new ShortBytes() { Short = value };

            result[0] = primitive.Byte0;
            result[1] = primitive.Byte1;

            return result;
        }

        public override byte[] GetBytes(int value)
        {
            byte[] result = new byte[sizeof(int)];
            IntBytes primitive = new IntBytes() { Int = value };

            result[0] = primitive.Byte0;
            result[1] = primitive.Byte1;
            result[2] = primitive.Byte2;
            result[3] = primitive.Byte3;

            return result;
        }

        public override byte[] GetBytes(long value)
        {
            byte[] result = new byte[sizeof(long)];
            LongBytes primitive = new LongBytes() { Long = value };

            result[0] = primitive.Byte0;
            result[1] = primitive.Byte1;
            result[2] = primitive.Byte2;
            result[3] = primitive.Byte3;
            result[4] = primitive.Byte4;
            result[5] = primitive.Byte5;
            result[6] = primitive.Byte6;
            result[7] = primitive.Byte7;

            return result;
        }

        public override byte[] GetBytes(float value)
        {
            byte[] result = new byte[sizeof(float)];
            FloatBytes primitive = new FloatBytes() { Float = value };

            result[0] = primitive.Byte0;
            result[1] = primitive.Byte1;
            result[2] = primitive.Byte2;
            result[3] = primitive.Byte3;

            return result;
        }

        public override byte[] GetBytes(double value)
        {
            byte[] result = new byte[sizeof(double)];
            DoubleBytes primitive = new DoubleBytes() { Double = value };

            result[0] = primitive.Byte0;
            result[1] = primitive.Byte1;
            result[2] = primitive.Byte2;
            result[3] = primitive.Byte3;
            result[4] = primitive.Byte4;
            result[5] = primitive.Byte5;
            result[6] = primitive.Byte6;
            result[7] = primitive.Byte7;

            return result;
        }


        public override short GetInt16(byte[] bytes)
        {
            ShortBytes primitive = new ShortBytes();

            primitive.Byte0 = bytes[0];
            primitive.Byte1 = bytes[1];

            return primitive.Short;
        }

        public override int GetInt32(byte[] bytes)
        {
            IntBytes primitive = new IntBytes();

            primitive.Byte0 = bytes[0];
            primitive.Byte1 = bytes[1];
            primitive.Byte2 = bytes[2];
            primitive.Byte3 = bytes[3];

            return primitive.Int;
        }

        public override long GetInt64(byte[] bytes)
        {
            LongBytes primitive = new LongBytes();

            primitive.Byte0 = bytes[0];
            primitive.Byte1 = bytes[1];
            primitive.Byte2 = bytes[2];
            primitive.Byte3 = bytes[3];
            primitive.Byte4 = bytes[4];
            primitive.Byte5 = bytes[5];
            primitive.Byte6 = bytes[6];
            primitive.Byte7 = bytes[7];

            return primitive.Long;
        }

        public override float GetFloat(byte[] bytes)
        {
            FloatBytes primitive = new FloatBytes();

            primitive.Byte0 = bytes[0];
            primitive.Byte1 = bytes[1];
            primitive.Byte2 = bytes[2];
            primitive.Byte3 = bytes[3];

            return primitive.Float;
        }

        public override double GetDouble(byte[] bytes)
        {
            DoubleBytes primitive = new DoubleBytes();

            primitive.Byte0 = bytes[0];
            primitive.Byte1 = bytes[1];
            primitive.Byte2 = bytes[2];
            primitive.Byte3 = bytes[3];
            primitive.Byte4 = bytes[4];
            primitive.Byte5 = bytes[5];
            primitive.Byte6 = bytes[6];
            primitive.Byte7 = bytes[7];

            return primitive.Double;
        }
    }
}
