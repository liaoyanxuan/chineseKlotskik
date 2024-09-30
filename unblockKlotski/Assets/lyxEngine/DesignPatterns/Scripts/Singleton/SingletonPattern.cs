using UnityEngine;
using System;
using System.Collections;
using liaoyanxuan.common.injector;
using liaoyanxuan.common.interfaces;
using sw.game.evt;

namespace VoxelBusters.DesignPatterns
{
	public class SingletonPattern <T> : MonoBehaviour, ISingleton where T : MonoBehaviour
	{
        #region Static Fields

        protected   static 		T 			instance 					= null;
		protected 	static 		object 		instanceLock			 	= new object();
		protected 	static 		bool 		destroyedOnApplicationQuit 	= false;

		#endregion

		#region Fields

		private 				Transform	m_transform					= null;
		private 				GameObject	m_gameObject				= null;

		private					bool		m_isInitialized				= false;
		private 				bool		m_isForcefullyDestroyed		= false;

        #endregion

        #region Static Properties

        public static Transform parentTransorm;

        [Inject(InjectType.SINGLETON)]
        protected static ILoggerHelper iLoggerHelper { get; set; }    //日志系统;    负责写日志

        [Inject(InjectType.SINGLETON)]
        protected static IAssetLoader iAssetLoader { get; set; }        //asset资源加载


        /// <summary>
        /// Gets the singleton instance which will be persistent until Application quits.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
		{
			get 
			{
				System.Type _singletonType	= typeof(T);
				
				// We are requesting an instance after application is quit
				if (destroyedOnApplicationQuit) 
				{
					Debug.LogWarning("[SingletonPattern] " + _singletonType + " instance is already destroyed.");
					return null;
				}
				
				lock (instanceLock)
				{
					if (instance == null)
					{
                        InjectorFactory.Instance.InjectStatic(_singletonType);
                        // Get all the instances that exist in the screen
                        T[] _monoComponents = FindObjectsOfType(_singletonType) as T[];
						
						if (_monoComponents.Length > 0)
						{
							instance		= _monoComponents[0];

							for (int iter = 1; iter < _monoComponents.Length; iter++)
								Destroy(_monoComponents[iter].gameObject);
						}
						
						// We need to create new instance
						if (instance == null)
						{
							// First search in resource if prefab exists for this class
							string 		_singletonName		= _singletonType.Name;
                            //GameObject 	_singletonPrefab 	=  Resources.Load("Singleton/" + _singletonName, typeof(GameObject)) as GameObject;
                            UnityEngine.Object _singletonPrefab = iAssetLoader.LoadAssetSync("prefabs/singleton/" + _singletonName.ToLower(), _singletonName, typeof(GameObject));
                          //  _singletonPrefab = (GameObject)GameObject.Instantiate(arrowPrefab, drawStepPanel.transform, false);

                            if (_singletonPrefab != null)
							{
								
                                if(null!= parentTransorm)
                                {
                                    instance = (GameObject.Instantiate(_singletonPrefab, parentTransorm, false) as GameObject).GetComponent<T>();
                                }else
                                {
                                    instance = (Instantiate(_singletonPrefab) as GameObject).GetComponent<T>();
                                }
							}
							else
							{
								instance 		= new GameObject().AddComponent<T>();
							}
							
							// Update name 
							instance.name		= _singletonName;
						}
					}
				}

				// Check if component is initialized or not
				SingletonPattern<T> 	_singletonInstance	= (SingletonPattern<T>)(object)instance;

				if (!_singletonInstance.m_isInitialized)
					_singletonInstance.Init();

				return instance;
			}
			
			private set
			{
				instance	= value;
			}
		}

		#endregion

		#region Properties
		
		public Transform CachedTransform
		{
			get
			{
				if (m_transform == null)
					m_transform	= transform;
				
				return m_transform;
			}
		}
		
		public GameObject CachedGameObject
		{
			get
			{
				if (m_gameObject == null)
					m_gameObject	= gameObject;
				
				return m_gameObject;
			}
		}
		
		#endregion

		#region Methods

		private void Awake ()
		{
			if (!m_isInitialized)			
				Init();
		}

		protected virtual void Start ()
		{
            EventDispatcher.AddEventListener(UIEventType.COLOR_SETTING_CHANGE_STAGE2, setColor);
            OnStart();
            setColor(null);
        }

        virtual protected void OnStart()
        {

        }

        virtual protected void setColor(object[] param)
        {
            // Debug.Log("setColor");
        }

        protected virtual void Reset ()
		{
			// Reset properties
			m_gameObject			= null;
			m_transform				= null;
			m_isInitialized			= false;
			m_isForcefullyDestroyed	= false;
		}

		protected virtual void OnEnable ()
		{}

		protected virtual void OnDisable ()
		{}

		protected virtual void OnDestroy ()
		{
			// Singleton instance means same instance will run throughout the gameplay session
			// If its destroyed that means application is quit
			if (instance == this && !m_isForcefullyDestroyed)
				destroyedOnApplicationQuit = true;
		}

		#endregion
	
		#region Methods

		protected virtual void Init ()
		{
			// Set as initialized
			m_isInitialized	= true;

			// Just in case, handling so that only one instance is alive
			if (instance == null)
			{
				instance 	= this as T;
			}
			// Destroying the reduntant copy of this class type
			else if (instance != this)
			{
				Destroy(CachedGameObject);
				return;
			}
			
			// Set it as persistent object
			DontDestroyOnLoad(CachedGameObject);
		}

		public void ForceDestroy ()
		{			
			// Mark that object was forcefully destroyed
			m_isForcefullyDestroyed = true;

			// Destory
			Destroy(CachedGameObject);
		}

		#endregion
	}
}