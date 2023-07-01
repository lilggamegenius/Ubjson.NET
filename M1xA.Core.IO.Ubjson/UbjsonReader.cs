// 
// UbjsonReader.cs
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
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using M1xA.Core.IO.Ubjson.Extensions;

namespace M1xA.Core.IO.Ubjson;

/// <summary>
///     Reads data from the Ubjson-formatted stream.
/// </summary>
public class UbjsonReader : Ubjson{
    /// <summary>
    ///     Opens the read-only stream with data from specified array and initializes the UBJSON Reader.
    /// </summary>
    /// <param name="data">Well formatted UBJSON objects.</param>
    public UbjsonReader(byte[] data) : this(new MemoryStream(data)){}

	public UbjsonReader(Stream stream) : base(stream){}

	public bool EndOfStream=>Stream.Position == Stream.Length;

	public object ReadNull(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && marker.Equals(DataMarker.Null)){
			return null;
		}

		throw new InvalidMarkerException(bits);
	}

	public bool ReadBool(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && (marker.Equals(DataMarker.True) || marker.Equals(DataMarker.False))){
			return marker.Equals(DataMarker.True);
		}

		throw new InvalidMarkerException(bits);
	}

	public byte ReadByte(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && marker.Equals(DataMarker.Byte)){
			return GetRawByte();
		}

		throw new InvalidMarkerException(bits);
	}

	public short ReadInt16(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && marker.Equals(DataMarker.Int16)){
			return GetRawInt16();
		}

		throw new InvalidMarkerException(bits);
	}

	public int ReadInt32(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && marker.Equals(DataMarker.Int32)){
			return GetRawInt32();
		}

		throw new InvalidMarkerException(bits);
	}

	public long ReadInt64(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && marker.Equals(DataMarker.Int64)){
			return GetRawInt64();
		}

		throw new InvalidMarkerException(bits);
	}

	public float ReadFloat(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && marker.Equals(DataMarker.Float)){
			return GetRawFloat();
		}

		throw new InvalidMarkerException(bits);
	}

	public double ReadDouble(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && marker.Equals(DataMarker.Double)){
			return GetRawDouble();
		}

		throw new InvalidMarkerException(bits);
	}

	public BigInteger ReadHugeBigInteger(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && (marker.Equals(DataMarker.Huge) || marker.Equals(DataMarker.ShortHuge))){
			return (BigInteger)GetRawHuge(marker.Equals(DataMarker.ShortHuge));
		}

		throw new InvalidMarkerException(bits);
	}

	public decimal ReadHugeDecimal(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && (marker.Equals(DataMarker.Huge) || marker.Equals(DataMarker.ShortHuge))){
			return (decimal)GetRawHuge(marker.Equals(DataMarker.ShortHuge));
		}

		throw new InvalidMarkerException(bits);
	}

	public string ReadString(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && (marker.Equals(DataMarker.String) || marker.Equals(DataMarker.ShortString))){
			return GetRawString(marker.Equals(DataMarker.ShortString));
		}

		throw new InvalidMarkerException(bits);
	}

	public object[] ReadArray(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && (marker.Equals(DataMarker.Array) || marker.Equals(DataMarker.ShortArray))){
			return GetRawArray(marker.Equals(DataMarker.ShortArray));
		}

		throw new InvalidMarkerException(bits);
	}

	public object ReadObject(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && (marker.Equals(DataMarker.Object) || marker.Equals(DataMarker.ShortObject))){
			return GetRawObject(marker.Equals(DataMarker.ShortObject));
		}

		throw new InvalidMarkerException(bits);
	}

    /// <summary>
    ///     Reads the array header.
    /// </summary>
    /// <returns>Array length.</returns>
    public int ReadArrayHeader(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && (marker.Equals(DataMarker.Array) || marker.Equals(DataMarker.ShortArray))){
			return marker == DataMarker.ShortArray ? GetRawByte() : GetRawInt32();
		}

		throw new InvalidMarkerException(bits);
	}

    /// <summary>
    ///     Reads the object header.
    /// </summary>
    /// <returns>Item count.</returns>
    public int ReadObjectHeader(){
		byte bits = GetRawByte();
		if(bits.GetMarker(out DataMarker marker) && (marker.Equals(DataMarker.Object) || marker.Equals(DataMarker.ShortObject))){
			return marker == DataMarker.ShortObject ? GetRawByte() : GetRawInt32();
		}

		throw new InvalidMarkerException(bits);
	}

	public bool TryReadNull(ref object result){
		try{
			result = ReadNull();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadByte(ref byte result){
		try{
			result = ReadByte();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadInt16(ref short result){
		try{
			result = ReadInt16();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadInt32(ref int result){
		try{
			result = ReadInt32();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadInt64(ref long result){
		try{
			result = ReadInt64();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadFloat(ref float result){
		try{
			result = ReadFloat();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadDouble(ref double result){
		try{
			result = ReadDouble();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadHugeBigInteger(ref BigInteger result){
		try{
			result = ReadHugeBigInteger();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadHugeDecimal(ref decimal result){
		try{
			result = ReadHugeDecimal();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadString(ref string result){
		try{
			result = ReadString();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadArray(ref object[] result){
		try{
			result = ReadArray();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadObject(ref object result){
		try{
			result = ReadObject();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadArrayHeader(ref int length){
		try{
			length = ReadArrayHeader();
			return true;
		} catch{
			return false;
		}
	}

	public bool TryReadObjectHeader(ref int itemCount){
		try{
			itemCount = ReadObjectHeader();
			return true;
		} catch{
			return false;
		}
	}

    /// <summary>
    ///     Utility method that automatically detects type
    ///     and parses the only one its item from Ubjson-enabled stream.
    ///     All tree elements of object will be parsed.
    /// </summary>
    /// <returns>The result value is the value of primitive or container type. Container can be an object or array.</returns>
    public object Parse(){
		while(Stream.Read(out byte bits)){
			if(!bits.GetMarker(out DataMarker header)) throw new InvalidMarkerException(bits);
			switch(header){
				case DataMarker.Null:        return null;
				case DataMarker.True:        return true;
				case DataMarker.False:       return false;
				case DataMarker.Byte:        return GetRawByte();
				case DataMarker.Int16:       return GetRawInt16();
				case DataMarker.Int32:       return GetRawInt32();
				case DataMarker.Int64:       return GetRawInt64();
				case DataMarker.Float:       return GetRawFloat();
				case DataMarker.Double:      return GetRawDouble();
				case DataMarker.ShortHuge:   return GetRawHuge(true);
				case DataMarker.Huge:        return GetRawHuge();
				case DataMarker.ShortString: return GetRawString(true);
				case DataMarker.String:      return GetRawString();
				case DataMarker.ShortArray:  return GetRawArray(true);
				case DataMarker.Array:       return GetRawArray();
				case DataMarker.ShortObject: return GetRawObject(true);
				case DataMarker.Object:      return GetRawObject();
				case DataMarker.NoOp:        continue;
				case DataMarker.End:         throw new EndOfContainerException();
				case DataMarker.Unknown:
				default:                     throw new NotImplementedException($"Reading of the {header} data type is not implemented.");
			}
		}
		throw new IrregularEndOfStreamException();
	}

	protected byte GetRawByte(){
		if(Stream.Read(out byte result)){
			return result;
		}

		throw new IrregularEndOfStreamException();
	}

	protected short GetRawInt16(){
		Span<byte> data = stackalloc byte[sizeof(short)];
		if(StreamExtension.Read(Stream, data)){
			return ByteService.GetInt16(data);
		}

		throw new IrregularEndOfStreamException();
	}

	protected int GetRawInt32(){
		Span<byte> data = stackalloc byte[sizeof(int)];
		if(StreamExtension.Read(Stream, data)){
			return ByteService.GetInt32(data);
		}

		throw new IrregularEndOfStreamException();
	}

	protected long GetRawInt64(){
		Span<byte> data = stackalloc byte[sizeof(long)];
		if(StreamExtension.Read(Stream, data)){
			return ByteService.GetInt64(data);
		}

		throw new IrregularEndOfStreamException();
	}

	protected float GetRawFloat(){
		Span<byte> data = stackalloc byte[sizeof(float)];
		if(StreamExtension.Read(Stream, data)){
			return ByteService.GetFloat(data);
		}

		throw new IrregularEndOfStreamException();
	}

	protected double GetRawDouble(){
		Span<byte> data = stackalloc byte[sizeof(double)];
		if(StreamExtension.Read(Stream, data)){
			return ByteService.GetDouble(data);
		}

		throw new IrregularEndOfStreamException();
	}

	protected dynamic GetRawHuge(bool zipped = false){
		string data = GetRawString(zipped);
		if(BigInteger.TryParse(data, out BigInteger hugeInt)){
			return hugeInt;
		}

		if(decimal.TryParse(data, out decimal hugeDecimal)){
			return hugeDecimal;
		}

		throw new UbjsonException("Invalid Huge number");
	}

	protected string GetRawString(bool zipped = false){
		int byteCount = zipped ? GetRawByte() : GetRawInt32();
		Span<byte> data = byteCount < 1024 ? stackalloc byte[byteCount] : new byte[byteCount];
		if(StreamExtension.Read(Stream, data)){
			return Encoding.GetString(data);
		}

		throw new IrregularEndOfStreamException();
	}

	protected object[] GetRawArray(bool zipped = false){
		int itemCount = zipped ? GetRawByte() : GetRawInt32();
		List<object> result = new(itemCount);
		if(itemCount != UnknownLength){
			for(int i = 0; i < itemCount; i++){
				result.Add(Parse());
			}
		} else{
			try{
				while(true){
					result.Add(Parse());
				}
			} catch(EndOfContainerException){}
		}

		return result.ToArray();
	}

	protected Dictionary<string, object> GetRawObject(bool zipped = false){
		int itemCount = zipped ? GetRawByte() : GetRawInt32();
		Dictionary<string, object> result = new(itemCount);
		if(itemCount != UnknownLength){
			for(int i = 0; i < itemCount; i++){
				result.Add(Parse().ToString(), Parse());
			}
		} else{
			try{
				while(true){
					result.Add(Parse().ToString(), Parse());
				}
			} catch(EndOfContainerException){}
		}

		return result;
	}
}
