// 
// LittleByteService.cs
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

namespace M1xA.Core.IO.Ubjson;

public class LittleByteService : ByteService{
	public override void GetBytes(short value, Span<byte> bytes){
		var primitive = new ShortBytes{ Short = value };
		bytes[0] = primitive.Byte1;
		bytes[1] = primitive.Byte0;
	}

	public override void GetBytes(int value, Span<byte> bytes){
		var primitive = new IntBytes{ Int = value };
		bytes[0] = primitive.Byte3;
		bytes[1] = primitive.Byte2;
		bytes[2] = primitive.Byte1;
		bytes[3] = primitive.Byte0;
	}

	public override void GetBytes(long value, Span<byte> bytes){
		var primitive = new LongBytes{ Long = value };
		bytes[0] = primitive.Byte7;
		bytes[1] = primitive.Byte6;
		bytes[2] = primitive.Byte5;
		bytes[3] = primitive.Byte4;
		bytes[4] = primitive.Byte3;
		bytes[5] = primitive.Byte2;
		bytes[6] = primitive.Byte1;
		bytes[7] = primitive.Byte0;
	}

	public override void GetBytes(float value, Span<byte> bytes){
		var primitive = new FloatBytes{ Float = value };
		bytes[0] = primitive.Byte3;
		bytes[1] = primitive.Byte2;
		bytes[2] = primitive.Byte1;
		bytes[3] = primitive.Byte0;
	}

	public override void GetBytes(double value, Span<byte> bytes){
		var primitive = new DoubleBytes{ Double = value };
		bytes[0] = primitive.Byte7;
		bytes[1] = primitive.Byte6;
		bytes[2] = primitive.Byte5;
		bytes[3] = primitive.Byte4;
		bytes[4] = primitive.Byte3;
		bytes[5] = primitive.Byte2;
		bytes[6] = primitive.Byte1;
		bytes[7] = primitive.Byte0;
	}

	public override short GetInt16(ReadOnlySpan<byte> bytes){
		var primitive = new ShortBytes{
			Byte0 = bytes[1],
			Byte1 = bytes[0]
		};
		return primitive.Short;
	}

	public override int GetInt32(ReadOnlySpan<byte> bytes){
		var primitive = new IntBytes{
			Byte0 = bytes[3],
			Byte1 = bytes[2],
			Byte2 = bytes[1],
			Byte3 = bytes[0]
		};
		return primitive.Int;
	}

	public override long GetInt64(ReadOnlySpan<byte> bytes){
		var primitive = new LongBytes{
			Byte0 = bytes[7],
			Byte1 = bytes[6],
			Byte2 = bytes[5],
			Byte3 = bytes[4],
			Byte4 = bytes[3],
			Byte5 = bytes[2],
			Byte6 = bytes[1],
			Byte7 = bytes[0]
		};
		return primitive.Long;
	}

	public override float GetFloat(ReadOnlySpan<byte> bytes){
		var primitive = new FloatBytes{
			Byte0 = bytes[3],
			Byte1 = bytes[2],
			Byte2 = bytes[1],
			Byte3 = bytes[0]
		};
		return primitive.Float;
	}

	public override double GetDouble(ReadOnlySpan<byte> bytes){
		var primitive = new DoubleBytes{
			Byte0 = bytes[7],
			Byte1 = bytes[6],
			Byte2 = bytes[5],
			Byte3 = bytes[4],
			Byte4 = bytes[3],
			Byte5 = bytes[2],
			Byte6 = bytes[1],
			Byte7 = bytes[0]
		};
		return primitive.Double;
	}
	[Obsolete("Please use the Span<> overloads")]
	public override byte[] GetBytes(short value){
		byte[] bytes = new byte[sizeof(short)];
		GetBytes(value, bytes);
		return bytes;
	}
	[Obsolete("Please use the Span<> overloads")]
	public override byte[] GetBytes(int value){
		byte[] bytes = new byte[sizeof(int)];
		GetBytes(value, bytes);
		return bytes;
	}
	[Obsolete("Please use the Span<> overloads")]
	public override byte[] GetBytes(long value){
		byte[] bytes = new byte[sizeof(long)];
		GetBytes(value, bytes);
		return bytes;
	}
	[Obsolete("Please use the Span<> overloads")]
	public override byte[] GetBytes(float value){
		byte[] bytes = new byte[sizeof(float)];
		GetBytes(value, bytes);
		return bytes;
	}
	[Obsolete("Please use the Span<> overloads")]
	public override byte[] GetBytes(double value){
		byte[] bytes = new byte[sizeof(double)];
		GetBytes(value, bytes);
		return bytes;
	}
}
