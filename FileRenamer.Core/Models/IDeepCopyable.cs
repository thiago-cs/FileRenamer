namespace FileRenamer.Core.Models;

public interface IDeepCopyable<T>
{
	/// <summary>
	/// Creates a new object that is a deep copy of the current instance.
	/// </summary>
	/// <returns>A new <typeparamref name="T"/> that is a copy of this instance.</returns>
	T DeepCopy();
}