using UnityEngine;

namespace GenericMonoSingleton
{
    /// <summary>
    /// This is the base class for implementing your own singleton class whose script can be attached to any Unity GameObject, but the Unity GameObject attached with this singleton script will be destroyed on switching to another scene.<br /><br />
    /// NOTE: <br />
    /// 1. If you want to inherit from this abstract class, then DO NOT write your own Awake() method in your child class.<br />
    /// 2. Because of 1, if you want to do some initialization, DO NOT write code in Awake() method, OVERRIDE Init() method AND DO it in Init() method.<br />
    /// 3. If you really want to write your own Awake() method in your child class, then you should call Awake() method of the parent class by writing code "base.Awake()" at the first line of your own Awake() method.
    /// </summary>
    /// <typeparam name="T">The type of the singleton instance</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static bool applicationIsQuitting;
        private static readonly object syncObject = new object();

        private static volatile T instance;
        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance \"{typeof(T)}\" already destroyed on application quit. Won't create again - returning null");
                    return null;
                }

                if (instance == null)
                {
                    lock (syncObject)
                    {
                        if (instance == null)
                        {
                            instance = FindObjectOfType<T>();  // Neither instance = this nor instance = new T() is allowed, because the first one will throw compiler error and the second one will break the rule "MonoBehaviour instance cannot be created by 'new' keyword". 
                            if (instance == null)  // No GameObject exists in the scene with the type "T" attached on it, so a new one will be created and attach the type "T" on it.
                            {
                                var obj = new GameObject($"Singleton of {typeof(T)}");

                                // 因为AddComponent()方法的作用是给游戏对象添加一个激活的游戏组件（脚本）并且返回这个组件（脚本），所以调用AddComponent()方法为游戏对象动态添加组件后，该组件（脚本）的Awake()和OnEnable()这两个表示脚本生命周期的回调函数会按照顺序依次被调用，调用完OnEnable()方法后AddComponent()方法才会返回并将新添加的组件（脚本）返回给调用者。
                                // 综上所述，在AddComponent()方法被调用之后，通过AddComponent()方法添加的新脚本的Awake()方法和OnEnable()方法已经被调用了。
                                // 注意：只有Awake()方法和OnEnable()方法在AddComponent()方法中被调用，Start()方法以及它之后的脚本生命周期方法不会在AddComponent()方法中被调用，但是它们肯定会被调用，只是调用的时机不是在AddComponent()方法中，尤其是对于Start()方法而言，我们通常会误以为它和Awake()方法都会被调用，但其实并不是，这和Start()方法的调用时机有关，它是在开始更新第一帧（调用FixedUpdate()）之前被调用，而不是在脚本被创建并激活之后就被调用，在脚本被创建的时候调用的是Awake()方法，在脚本被激活的时候调用的是OnEnable()方法，这也解释了为什么AddComponent()方法调用之后（AddComponent()方法的作用就是创建并激活一个脚本并挂载到游戏物体上），只有Awake()和OnEnable()方法被立刻调用了。
                                obj.AddComponent<T>();  // 这里不使用instance = obj.AddComponent<T>(); 的原因是：AddComponent()方法内部会调用脚本的Awake()方法，而我们在Awake()方法内部已经为instance赋值成了this，参见下面写的Awake()方法，并且调用了Init()这个初始化方法，所以AddComponent()方法被调用之后我们既不用为instance赋值也不用再调用Init()方法了，否则instance会被赋值两次（虽然两次赋的值都是一样的），Init()初始化方法也会被调用两次，虽然没有什么大碍，但是我们应该尽量保证instance只被赋值一次，Init()方法只被调用一次。
                            }
                            else
                            {
                                // 这个方法放到else中的原因是：上面的obj.AddComponent<T>()方法被调用后，脚本的Awake()方法会立刻被调用，我们在Awake()方法中已经为instance赋值了，并且调用了Init()这个初始化方法。所以就不用再调用Init()这个初始化方法了。这就是为什么Init()方法放在else中，而不是直接跟在obj.AddComponent<T>()之后。
                                instance.Init();   // instance.Init() can be called only if "where T : MonoSingleton<T>". Because Init() is a instance method which cannot be called in a static method, Init() can be called only by its instance.
                            }
                        }
                    }
                }

                return instance;
            }
        }

        protected virtual void Init() { }   // 这个方法必须声明成virtual或者abstract方法，因为上面Instance属性的get方法中使用了instance.Init()来调用Init()方法，如果Init()方法不声明成virtual或者abstract方法，instance.Init()就不会调用到子类重写的Init()方法，因为instance.Init()是在父类调用的，如果我们想要一个方法实现“调父执行子”，那么这个方法必须是virtual或者abstract方法。

        // Awake()方法会被子类继承，所以即使子类不写Awake()方法，当子类执行脚本生命周期中的Awake()方法的时候，如果子类没有自己的Awake()方法，那么这个被继承的Awake()方法就会被执行。
        // 事实上，子类不要写自己的Awake()方法，因为Awake()方法里面做了初始化和给instance赋值的操作，如果子类写了自己的Awake()方法，可能不能正确地初始化instance对象。
        // 所以我们子类如果要初始化，就不要再写一个Awake()方法，并在Awake()方法中进行，正确的做法是覆写Init()方法，在Init()方法中进行初始化操作。
        // 如果非要在子类中写子类的Awake()方法，那么需要在Awake()方法的第一行调一下父类的Awake()方法，做法为：base.Awake()。
        protected void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                Init();
            }
        }

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        protected void OnApplicationQuit()
        {
            if (this == instance)   // 这个判断条件的作用：如果场景中出现了多个脚本的实例，那么当程序退出，我们在删除这些多余的脚本实例的时候不会影响到“applicationIsQuitting”这个标记位，只有当删除到真正使用的单例实例的时候，“applicationIsQuitting”这个标记位才会被标记。
            {
                applicationIsQuitting = true;
            }
        }
    }
}

