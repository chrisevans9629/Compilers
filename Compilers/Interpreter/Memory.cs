using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class Memory
    {
        public string Name { get; }
        public Memory Parent { get; }
        private Dictionary<string, object> dictionary  = new Dictionary<string, object>();

        public Memory(string name, Memory parent = null)
        {
            Name = name;
            Parent = parent;
        }
        public bool ContainsKey(string key)
        {

            var result = dictionary.ContainsKey(key.ToUpper());
            if (!result && Parent != null)
            {
                return Parent.ContainsKey(key);
            }

            return result;
        }
        public void Add(string key, object obj)
        {
            dictionary.Add(key.ToUpper(), obj);
        }

        public void SetValue(string key, object value)
        {
            var uKey = key.ToUpper();
            if (dictionary.ContainsKey(uKey))
            {
                dictionary[uKey] = value;
            }
            else if (Parent != null)
            {
                Parent.SetValue(uKey, value);
            }
            else
            {
                throw new RuntimeException(ErrorCode.IdNotFound,null, $"Could not set '{uKey}' to '{value}' since it has not been defined");
            }
        }

        public T GetValue<T>(string key, bool checkParent = false)
        {
            var uKey = key.ToUpper();
            if (dictionary.ContainsKey(uKey) && dictionary[key.ToUpper()] is T val)
            {
                return val;
            }
            else if (checkParent && Parent != null)
            {
                return Parent.GetValue<T>(key, true);
            }
            throw new RuntimeException(ErrorCode.IdNotFound, null, $"Could not find '{key}'");

        }
        public object GetValue(string key, bool checkParent = false)
        {
            var uKey = key.ToUpper();
            if (dictionary.ContainsKey(uKey))
            {
                return dictionary[key.ToUpper()];
            }
            else if(checkParent && Parent != null)
            {
                return Parent.GetValue(key, true);
            }
            throw new RuntimeException(ErrorCode.IdNotFound, null, $"Could not find '{key}'");
        }
        // public object this[string key] { get => dictionary[key.ToUpper()]; set=> dictionary[key.ToUpper()] = value; }
        public override string ToString()
        {
            return $"{Name}(\n{dictionary.Aggregate("", (s, pair) => s + $", ({pair.Key},{pair.Value})")})\n {Parent}";
        }
    }
}