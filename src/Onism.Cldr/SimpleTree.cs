using System.Collections.Generic;
using Onism.Cldr.Extensions;
using ProtoBuf;

namespace Onism.Cldr
{
    /// <summary>
    /// Alternative for CldrTree. Simpler, but bigger.
    /// </summary>
    [ProtoContract]
    public class SimpleTree : Dictionary<string, SimpleTree>
    {
        [ProtoMember(1, AsReference = true)]
        public string Value { get; set; }

        public void Add(string path, string value)
        {
            var pathData = path.Split('.');
            var segments = new Queue<string>(pathData);
            Add(segments, value);
        }

        public void Add(Queue<string> segments, string value)
        {
            if (segments.IsEmpty())
            {
                this.Value = value;
                return;
            }

            var front = segments.Dequeue();

            if (this.ContainsKey(front))
            {
                var nested = this[front];
                nested.Add(segments, value);
            }
            else
            {
                var nested = new SimpleTree();
                this.Add(front, nested);
                nested.Add(segments, value);
            }            
        }
    }
}