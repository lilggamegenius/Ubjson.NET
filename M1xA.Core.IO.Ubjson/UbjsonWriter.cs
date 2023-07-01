// 
// UbjsonWriter.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using M1xA.Core.IO.Ubjson.Extensions;

namespace M1xA.Core.IO.Ubjson;

/// <summary>
///     Writes data into stream in Ubjson format.
/// </summary>
public class UbjsonWriter : Ubjson, IDisposable{
	private readonly ObjectService _os;

	public UbjsonWriter(Stream stream) : base(stream){
		_os = new SimpleObjectService();
		_os.AddIgnorable(typeof(Delegate));
	}

	public void Dispose(){
		Dispose(true);
		GC.SuppressFinalize(this);
	}

    /// <summary>
    ///     Writes the null value.
    /// </summary>
    public void Write()=>Stream.Write(DataMarker.Null);

	public void Write(bool value)=>Stream.Write(value ? DataMarker.True : DataMarker.False);

	public void Write(byte value){
		Stream.Write(DataMarker.Byte);
		Span<byte> bytes = stackalloc byte[sizeof(byte)];
		bytes[0] = value;
		Stream.Write(bytes);
	}

	public void Write(short value){
		Stream.Write(DataMarker.Int16);
		Span<byte> bytes = stackalloc byte[sizeof(short)];
		ByteService.GetBytes(value, bytes);
		Stream.Write(bytes);
	}

	public void Write(int value){
		Stream.Write(DataMarker.Int32);
		Span<byte> bytes = stackalloc byte[sizeof(int)];
		ByteService.GetBytes(value, bytes);
		Stream.Write(bytes);
	}

	public void Write(long value){
		Stream.Write(DataMarker.Int64);
		Span<byte> bytes = stackalloc byte[sizeof(long)];
		ByteService.GetBytes(value, bytes);
		Stream.Write(bytes);
	}

	public void Write(float value){
		Stream.Write(DataMarker.Float);
		Span<byte> bytes = stackalloc byte[sizeof(float)];
		ByteService.GetBytes(value, bytes);
		Stream.Write(bytes);
	}

	public void Write(double value){
		Stream.Write(DataMarker.Double);
		Span<byte> bytes = stackalloc byte[sizeof(double)];
		ByteService.GetBytes(value, bytes);
		Stream.Write(bytes);
	}

    /// <summary>
    ///     Writes the passed Huge (number) value or its short version.
    /// </summary>
    /// <param name="value">BigInteger value to write.</param>
    public void Write(BigInteger value)=>WriteAsString(value.ToString(), DataMarker.ShortHuge, DataMarker.Huge);

	/// <summary>
    ///     Writes the passed Huge (number) value or its short version.
    /// </summary>
    /// <param name="value">Decimal value to write.</param>
    public void Write(decimal value)=>WriteAsString(value.ToString(), DataMarker.ShortHuge, DataMarker.Huge);

	/// <summary>
    ///     Writes the passed string value or its short version.
    /// </summary>
    /// <param name="value">String value to write.</param>
    public void Write(string value)=>WriteAsString(value, DataMarker.ShortString, DataMarker.String);

	/// <summary>
    ///     Writes header for the passed array or its short version and inspects array items.
    /// </summary>
    /// <param name="value">Flat array or container.</param>
    public void Write(IList value){
		WriteArrayHeader(value.Count);
		foreach(object item in value){
			Write(item);
		}
	}

    /// <summary>
    ///     Writes header for the passed object or its short version and inspects public members (fields and/or properties) of object.
    /// </summary>
    /// <param name="value">Object to inspect and to write.</param>
    public void Write(object value){
		DataMarker marker = value.GetMarker();
		switch(marker){
			case DataMarker.Null:
				Write();
				break;
			case DataMarker.True:
				Write((bool)value);
				break;
			case DataMarker.False:
				Write((bool)value);
				break;
			case DataMarker.Byte:
				Write((byte)value);
				break;
			case DataMarker.Int16:
				Write((short)value);
				break;
			case DataMarker.Int32:
				Write((int)value);
				break;
			case DataMarker.Int64:
				Write((long)value);
				break;
			case DataMarker.Float:
				Write((float)value);
				break;
			case DataMarker.Double:
				Write((double)value);
				break;
			case DataMarker.Huge:
				WriteAsString(value.ToString(), DataMarker.ShortHuge, DataMarker.Huge);
				break;
			case DataMarker.String:
				WriteAsString(value.ToString(), DataMarker.ShortString, DataMarker.String);
				break;
			case DataMarker.Array:
				Write(value as IList);
				break;
			case DataMarker.Object:{
				switch(value){
					case IDictionary<string, object> objects:{ // Added for dynamic/ExpandoObject.
						Dictionary<string, object> members = _os.GetSerializableMembers(objects);
						WriteObjectHeader(members.Count);
						foreach(KeyValuePair<string, object> kv in members){
							Write(kv.Key);
							Write(kv.Value);
						}

						break;
					}
					case IDictionary dictionary:{ // All other dictionaries
						Dictionary<object, object> members = _os.GetSerializableMembers(dictionary);
						WriteObjectHeader(members.Count);
						foreach(KeyValuePair<object, object> kv in members){
							Write(kv.Key);
							Write(kv.Value);
						}

						break;
					}
					default:{ // Standard case
						MemberInfo[] members = _os.GetSerializableMembers(value);
						WriteObjectHeader(members.Length);
						foreach(MemberInfo mi in members){
							switch(mi){
								case FieldInfo info:{
									Write(info.Name);
									Write(info.GetValue(value));
									break;
								}
								case PropertyInfo info:{
									Write(info.Name);
									Write(info.GetValue(value, null));
									break;
								}
							}
						}

						break;
					}
				}
			}
				break;
			case DataMarker.Unknown: throw new UbjsonException("Unknown data type to write.");
		}
	}

    /// <summary>
    ///     Writes the header info of array that has an unknown length.
    /// </summary>
    public void BeginWriteArray(){
		Stream.Write(DataMarker.ShortArray);
		Stream.WriteByte(UnknownLength);
	}

    /// <summary>
    ///     Writes the header info of object that has an unknown count of items.
    /// </summary>
    public void BeginWriteObject(){
		Stream.Write(DataMarker.ShortObject);
		Stream.WriteByte(UnknownLength);
	}

    /// <summary>
    ///     Ends writing of object or array.
    ///     Must be called after BeginWriteArray or BeginWriteObject methods for consistency and data integrity.
    /// </summary>
    public void EndWrite()=>Stream.Write(DataMarker.End);

	/// <summary>
    ///     Writes only the short or normal header for array depending on its length.
    /// </summary>
    /// <param name="count">Item count, its length.</param>
    public void WriteArrayHeader(int count){
		Span<byte> bytes = stackalloc byte[sizeof(int)];
		if(count <= ShortLength){
			Stream.Write(DataMarker.ShortArray);
			bytes[0] = (byte)count;
			Stream.Write(bytes[..1]);
		} else{
			Stream.Write(DataMarker.Array);
			ByteService.GetBytes(count, bytes);
			Stream.Write(bytes);
		}
	}

    /// <summary>
    ///     Writes only the short or normal header for object depending on item count.
    /// </summary>
    /// <param name="count">Item count.</param>
    public void WriteObjectHeader(int count){
		Span<byte> bytes = stackalloc byte[sizeof(int)];
		if(count <= ShortLength){
			Stream.Write(DataMarker.ShortObject);
			bytes[0] = (byte)count;
			Stream.Write(bytes[..1]);
		} else{
			Stream.Write(DataMarker.Object);
			ByteService.GetBytes(count, bytes);
			Stream.Write(bytes);
		}
	}

    /// <summary>
    ///     Writes the wait code (NoOp data marker).
    ///     Added for streaming and network purposes.
    /// </summary>
    public void KeepAlive()=>Stream.Write(DataMarker.NoOp);

	public virtual void Dispose(bool disposing){
		if(disposing) Stream.Flush();
	}

	private void WriteAsString(string value, DataMarker shortObject, DataMarker normalObject){
		Span<byte> data = stackalloc byte[value.Length * sizeof(char)];
		Encoding.GetBytes(value, data);
		Span<byte> countBytes = stackalloc byte[sizeof(int)];
		if(data.Length <= ShortLength){
			Stream.Write(shortObject);
			countBytes[0] = (byte)data.Length;
			Stream.Write(countBytes[..1]);
		} else{
			Stream.Write(normalObject);
			ByteService.GetBytes(data.Length, countBytes);
			Stream.Write(countBytes);
		}

		Stream.Write(data);
	}
}
