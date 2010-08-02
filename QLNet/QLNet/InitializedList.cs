using System.Collections.Generic;

namespace QLNet
{
	/// <summary>
	/// this is a redefined collection class to emulate array-type behaviour at initialisation
	/// if T is a class then the list is initilized with default constructors instead of null
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class InitializedList<T> : List<T> where T : new()
	{
		public InitializedList() : base() { }
		public InitializedList(int size)
			: base(size)
		{
			for (int i = 0; i < this.Capacity; i++)
				this.Add(default(T) == null ? new T() : default(T));
		}
		public InitializedList(int size, T value)
			: base(size)
		{
			for (int i = 0; i < this.Capacity; i++)
				this.Add(value);
		}

		// erases the contents without changing the size
		public void Erase()
		{
			for (int i = 0; i < this.Count; i++)
				this[i] = default(T);       // do we need to use "new T()" instead of default(T) when T is class?
		}
	}
}