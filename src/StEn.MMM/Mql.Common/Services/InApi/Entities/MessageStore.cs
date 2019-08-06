using System.Collections.Generic;

namespace StEn.MMM.Mql.Common.Services.InApi.Entities
{
	public class MessageStore<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> dictionary;
		private readonly Queue<TKey> keys;
		private readonly int capacity;

		public MessageStore(int capacity)
		{
			this.keys = new Queue<TKey>(capacity);
			this.capacity = capacity;
			this.dictionary = new Dictionary<TKey, TValue>(capacity);
		}

		public TValue this[TKey key] => this.dictionary[key];

		public void Add(TKey key, TValue value)
		{
			if (this.dictionary.Count == this.capacity)
			{
				var oldestKey = this.keys.Dequeue();
				this.dictionary.Remove(oldestKey);
			}

			this.dictionary.Add(key, value);
			this.keys.Enqueue(key);
		}
	}
}
