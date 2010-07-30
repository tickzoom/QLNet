namespace QLNet.Patterns
{
	public interface IVisitable<T>
	{
		/// <summary>
		/// This method is responsible for determining if a <seealso cref="IVisitor{T}"/> 
		/// passed as argument is eligible for handling the data structures kept by this class. 
		/// In the affirmative case, Accept is responsible for passing this data structures 
		/// to the <seealso cref="IVisitor{T}"/>.
		/// </summary>
		/// <param name="visitor"></param>
		void Accept(IVisitor<T> visitor);
	}

	public interface IVisitor<T>
	{
		/// <summary>
		/// This method is responsible for processing a data structure.
		/// </summary>
		/// <param name="object"></param>
		void Visit(T @object);
	}
}