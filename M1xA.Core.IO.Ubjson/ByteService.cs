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

namespace M1xA.Core.IO.Ubjson;

public abstract class ByteService{
	public abstract void GetBytes(short value, Span<byte> bytes);
	public abstract void GetBytes(int value, Span<byte> bytes);
	public abstract void GetBytes(long value, Span<byte> bytes);
	public abstract void GetBytes(float value, Span<byte> bytes);
	public abstract void GetBytes(double value, Span<byte> bytes);
	public abstract short GetInt16(ReadOnlySpan<byte> bytes);
	public abstract int GetInt32(ReadOnlySpan<byte> bytes);
	public abstract long GetInt64(ReadOnlySpan<byte> bytes);
	public abstract float GetFloat(ReadOnlySpan<byte> bytes);
	public abstract double GetDouble(ReadOnlySpan<byte> bytes);
	[Obsolete("Please use the Span<> overloads")] public abstract byte[] GetBytes(short value);
	[Obsolete("Please use the Span<> overloads")] public abstract byte[] GetBytes(int value);
	[Obsolete("Please use the Span<> overloads")] public abstract byte[] GetBytes(long value);
	[Obsolete("Please use the Span<> overloads")] public abstract byte[] GetBytes(float value);
	[Obsolete("Please use the Span<> overloads")] public abstract byte[] GetBytes(double value);
}
