using System;
using System.Collections.Generic;
using System.Linq;

namespace Student.Api.Template.Services.Utils
{
    public static class TypeHelper
    {
        public static Dictionary<TKey, TValue> GetDictionaryOfStaticFields<TKey, TValue>(Func<TValue, TKey> keySelector)
        {
            return typeof(TValue).GetFields().Where(x => x.IsStatic && x.FieldType == typeof(TValue)).Select(x => (TValue) x.GetValue(null))
                .ToDictionary(keySelector, x => x);
        }
    }
}