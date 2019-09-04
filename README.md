# Singleton in Unity
#### How To Use:

If you want to implement your own singleton class whose script can be attached to any Unity GameObject, then your own singleton class need to inherit from either "MonoSingleton&lt;T&gt;" class or "AcrossSceneMonoSingleton&lt;T&gt;" class.
In order to use the classes mentioned above, you need to use the namespace called "GenericMonoSingleton".

If you want the script of your own singleton class to be destroyed on switching to another scene, then you need to make your own class inherit from the base class called "MonoSingleton&lt;T&gt;" like so:

	using GenericMonoSingleton;

	public class YOURCLASS : MonoSingleton<YOURCLASS>
	{
		/// NOTE:
    	/// 1. DO NOT write your own Awake() method in YOURCLASS.
    	/// 2. Because of 1, if you want to do some initialization, DO NOT write code in Awake() method, OVERRIDE Init() method AND DO it in Init() method like BELOW.
    	/// 3. If you really want to write your own Awake() method in YOURCLASS, then you should call Awake() method of the base class by writing code "base.Awake()" at the first line of your own Awake() method.
		
		protected override void Init()
		{
			// Write your own initialization code here
		}
	}

NOW, you can access your own singleton instance like so:
	
	var singleton = YOURCLASS.Instance;   // This accessing code can be written wherever you want in your script, even in Awake() method.

But in order to access your own singleton instance like above, you can:
1. Create an empty Unity GameObject manually (actually any Unity GameObject is ok for this purpose) in the scene and attach the script of your own singleton class to that GameObject.
or
2. Write the accesssing code above directly in your script. In this case, you DO NOT need to create any Unity GameObjects manually, because the accessing code above can create one for you, and the name of that GameObject is "Singleton of YOURCLASS".


If you want the script of your own singleton class to be kept on switching to another scene, then you need to make your own class inherit from the base class called "AcrossSceneMonoSingleton<T>" like so:

	using GenericMonoSingleton;

	public class YOURCLASS : AcrossSceneMonoSingleton<YOURCLASS>
	{
		/// NOTE:
    	/// 1. DO NOT write your own Awake() method in YOURCLASS.
    	/// 2. Because of 1, if you want to do some initialization, DO NOT write code in Awake() method, OVERRIDE Init() method AND DO it in Init() method like BELOW.
    	/// 3. If you really want to write your own Awake() method in YOURCLASS, then you should call Awake() method of the base class by writing code "base.Awake()" at the first line of your own Awake() method.
		
		protected override void Init()
		{
			// Write your own initialization code here
		}
	}

NOW, the way of accessing your own singleton instance is the same as that mentioned above.
