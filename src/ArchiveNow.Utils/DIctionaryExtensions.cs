using System.Collections.Generic;

namespace ArchiveNow.Utils
{
    public static class DictionaryExtensions
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, IList<TValue>> map,
            TKey key, TValue value)
        {
            IList<TValue> list;
            if (map.ContainsKey(key))
            {
                list = map[key];
            }
            else
            {
                list = new List<TValue>();
                map.Add(key, list);
            }

            list.Add(value);
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, IList<TValue>> map,
            TKey key, IList<TValue> values)
        {
            if (map.ContainsKey(key))
            {
                var list = map[key];
                foreach (var value in values)
                {
                    list.Add(value);
                }
            }
            else
            {
                map.Add(key, values);
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.GetOrDefault(key, default(TValue));
        }
    }
}
