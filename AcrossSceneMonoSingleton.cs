using UnityEngine;

namespace GenericMonoSingleton
{
    /// <summary>
    /// This is the base class for implementing your own singleton class whose script can be attached to any Unity GameObject. Besides, as the class name implies, the Unity GameObject attached with this singleton script will be kept on switching to another scene.<br /><br />
    /// NOTE: <br />
    /// 1. If you want to inherit from this abstract class, then DO NOT write your own Awake() method in your child class.<br />
    /// 2. Because of 1, if you want to do some initialization, DO NOT write code in Awake() method, OVERRIDE Init() method AND DO it in Init() method.<br />
    /// 3. If you really want to write your own Awake() method in your child class, then you should call Awake() method of the parent class by writing code "base.Awake()" at the first line of your own Awake() method.
    /// </summary>
    /// <typeparam name="T">The type of the singleton instance</typeparam>
    public abstract class AcrossSceneMonoSingleton<T> : MonoSingleton<T> where T : AcrossSceneMonoSingleton<T>
    {
        protected new void Awake()
        {
            base.Awake();  // After "Awake()" method of the base class is called, singleton instance has been created and initialized, so singleton instance is available now.  

            // Set across scene instance and remove unnecessary instances.
            // 如果我们在一个场景A中创建了一个空物体obj，并且将继承了这个脚本的子脚本挂载在这个物体上，那么这个游戏物体在场景切换的时候就不会被删除。
            // 不过，如果我们从别的场景中再次回到场景A中去，场景A在创建的时候还是会创建一个obj的游戏物体，这个时候我们在创建A中就会存在两个obj游戏物体，所以我们需要删除那个被场景A创建的重复的obj游戏物体。
            // 这个if判断的条件就是创建一个不会在场景切换的时候被删除的游戏物体，而else条件就是删除上面提到的重复的obj游戏物体。
            if (Instance == this)   // 一旦我们调用了父类的Awake()方法（第一行写的base.Awake()），Instance这个单例就已经被成功的创建并且初始化了，所以我们在之后的代码中就可以随便使用Instance这个单例了，Instance这个单例在之后的代码中不会是null，一定是一个有效的对象。这里将Instance和this做判断为的是避免上面提到的重复创建的obj游戏物体上的脚本也会调用DontDestroyOnLoad()从而将这个重复的obj游戏物体也变成在场景切换的时候不会被删除的游戏物体。
            {
                DontDestroyOnLoad(gameObject);  // 创建一个不会在场景切换的时候被删除的游戏物体。
            }
            else
            {
                Destroy(gameObject);   // 删除上面提到的被重复创建的obj游戏物体。
            }
        }
    }
}
