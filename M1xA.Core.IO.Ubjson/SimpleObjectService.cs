// 
// SimpleObjectService.cs
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
    internal class SimpleObjectService : ObjectService
    {
        private List<Type> _ignore;
        private Dictionary<string, object> _infoCache;

        public SimpleObjectService()
        {
            _ignore = new List<Type>();
            _infoCache = new Dictionary<string, object>();
        }

        public override void AddIgnorable(Type type)
        {
            if (!_ignore.Contains(type))
                _ignore.Add(type);
        }

        public override void ClearIgnorable()
        {
            _ignore.Clear();
        }

        public override MemberInfo[] GetSerializableMembers(object o)
        {
            Type type = o.GetType();

            string key = type.ToString();

            if (_infoCache.ContainsKey(key))
            {
                return (MemberInfo[])_infoCache[key];
            }
            else
            {
                MemberInfo[] info = type.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public, MemberFilter, null);

                _infoCache.Add(key, info);

                return info;
            }
        }

        public override Dictionary<string, object> GetSerializableMembers(IDictionary<string, object> o)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> kv in o)
            {
                if (!Ignorable(kv.Value.GetType()))
                    result.Add(kv.Key, kv.Value);
            }

            return result;
        }

        public override Dictionary<object, object> GetSerializableMembers(IDictionary o)
        {
            Dictionary<object, object> result = new Dictionary<object, object>();
            
            foreach (DictionaryEntry kv in o)
            {
                if (!Ignorable(kv.Value.GetType()))
                    result.Add(kv.Key, kv.Value);
            }

            return result;
        }

        private bool MemberFilter(MemberInfo mi, object _)
        {
            if (mi is FieldInfo)
            {
                return !Ignorable((mi as FieldInfo).FieldType);
            }

            if (mi is PropertyInfo)
            {
                return !Ignorable((mi as PropertyInfo).PropertyType);
            }

            return false;
        }

        private bool Ignorable(Type type)
        {
            foreach (Type t in _ignore)
                if (t.IsAssignableFrom(type))
                    return true;

            return false;
        }
    }
}
