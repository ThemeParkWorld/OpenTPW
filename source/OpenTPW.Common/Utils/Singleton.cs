namespace OpenTPW;

public interface Singleton<T> where T : new()
{
	public static T Instance { get; set; }

	static Singleton()
	{
		Instance = new T();
	}
}
