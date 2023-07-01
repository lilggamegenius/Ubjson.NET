// 
// TypedExtensions.cs
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
using System.Dynamic;
using System.IO;
using System.Numerics;

namespace M1xA.Core.IO.Ubjson.Extensions;

internal static class ObjectTypeExtension{
	public static DataMarker GetMarker(this object value){
		return value switch{
			null=>DataMarker.Null,
			bool b=>b ? DataMarker.True : DataMarker.False,
			byte=>DataMarker.Byte,
			short=>DataMarker.Int16,
			int=>DataMarker.Int32,
			long=>DataMarker.Int64,
			float=>DataMarker.Float,
			double=>DataMarker.Double,
			decimal=>DataMarker.Huge,
			BigInteger=>DataMarker.Huge,
			ValueType=>DataMarker.Unknown,
			string=>DataMarker.String,
			IList=>DataMarker.Array,
			_=>DataMarker.Object
		};
	}
}
internal static class PrimitiveTypeExtension{
	public static bool GetMarker(this byte value, out DataMarker header){
		if(Enum.IsDefined(typeof(DataMarker), value)){
			header = (DataMarker)value;
			return true;
		}

		header = DataMarker.Unknown;
		return false;
	}
}
internal static class StreamExtension{
    /// <summary>
    ///     Reads and return only one byte from stream.
    /// </summary>
    /// <param name="stream">Underlying stream.</param>
    /// <param name="result">Contains readed byte.</param>
    /// <returns>False when end of stream reached.</returns>
    public static bool Read(this Stream stream, out byte result){
		Span<byte> buffer = stackalloc byte[1];
		int available = stream.Read(buffer);
		if(available == 1){
			result = buffer[0];
			return true;
		}

		result = default;
		return false;
	}

	[Obsolete("Please use the Span<> overloads")] 
	public static bool Read(this Stream stream, byte[] result, int count){
		int available = stream.Read(result, 0, count);
		return (available != 0) && (available >= count);
	}
	public static bool Read(this Stream stream, Span<byte> result){
		int available = stream.Read(result);
		return (available != 0) && (available >= result.Length);
	}

	public static void Write(this Stream output, DataMarker marker){
		Span<byte> bytes = stackalloc byte[1];
		bytes[0] = (byte)marker;
		output.Write(bytes);
	}
	[Obsolete("Please use the Span<> overloads")] 
	public static void Write(this Stream output, byte[] data)=>output.Write(data, 0, data.Length);
	public static void Write(this Stream output, ReadOnlySpan<byte> data)=>output.Write(data);
}
public static class IDictionaryExtension{
    /// <summary>
    ///     Materializes the dynamic object/expando object from the dictionary.
    /// </summary>
    /// <param name="value">Object to convert.</param>
    /// <param name="check">Verifying an object inherits the IDictionary.</param>
    /// <returns>Dynamic object.</returns>
    public static dynamic AsDynamic(this object value, bool check = false){
		if(value is IDictionary<string, object> objects){
			var expando = new ExpandoObject();
			IDictionary<string, object> expandoDictionary = expando;
			foreach(KeyValuePair<string, object> entry in objects){
				switch(entry.Value){
					case IDictionary<string, object>: 
						expandoDictionary.Add(entry.Key, entry.Value.AsDynamic());
						break;
					case IList array:{
						dynamic[] items = new dynamic[array.Count];
						for(int i = 0; i < array.Count; i++){
							items[i] = array[i].AsDynamic();
						}

						expandoDictionary.Add(entry.Key, items);
						break;
					}
					default: expandoDictionary.Add(entry);
						break;
				}
			}
			return expando;
		}

		if(!check){
			return value;
		}

		throw new UbjsonException("Can't cast an object to dynamic/expando object.");
	}
}
