// 
// ObjectService.cs
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
using System.Reflection;

namespace M1xA.Core.IO.Ubjson
{
    internal abstract class ObjectService
    {
        public abstract void AddIgnorable(Type type);
        public abstract void ClearIgnorable();
        public abstract MemberInfo[] GetSerializableMembers(object o);
        public abstract Dictionary<object, object> GetSerializableMembers(IDictionary o);
        public abstract Dictionary<string, object> GetSerializableMembers(IDictionary<string, object> o);
    }
}
