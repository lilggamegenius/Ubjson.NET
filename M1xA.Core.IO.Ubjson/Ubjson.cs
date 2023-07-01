// 
// Ubjson.cs
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
using System.IO;
using System.Text;

namespace M1xA.Core.IO.Ubjson;

public abstract class Ubjson{
	public const byte ShortLength = 0xFE;
	public const byte UnknownLength = 0xFF;
	public readonly Encoding DefaultEncoding = Encoding.UTF8;

	public Ubjson(Stream stream){
		Encoding = DefaultEncoding;
		Endianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
		ByteService = BitConverter.IsLittleEndian ? new LittleByteService() : new BigByteService();
		Stream = stream;
	}

    /// <summary>
    ///     Underlying stream.
    /// </summary>
    public Stream Stream{get;private set;}
	public Encoding Encoding{get;private set;}
	public Endianness Endianness{get;private set;}
	public ByteService ByteService{get;private set;}
}
