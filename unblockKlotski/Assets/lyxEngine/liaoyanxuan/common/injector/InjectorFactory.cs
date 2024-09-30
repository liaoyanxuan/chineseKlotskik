using System;
using System.Collections.Generic;
using System.Reflection;

namespace liaoyanxuan.common.injector
{
    public class InjectorFactory
    {
        private static InjectorFactory instance;
        public static InjectorFactory Instance
        {
            get { return instance ?? (instance = new InjectorFactory()); }
        }
        private InjectorFactory() 
        {
            BindCache = new Dictionary<Type, Type>(); 
        }
        public Dictionary<Type, Type> BindCache { get; set; }
        public void Bind<T, V>()
        {
            if (!BindCache.ContainsKey(typeof(T)))
            {
                BindCache.Add(typeof(T), typeof(V));
            }
            else
            {
                BindCache[typeof(T)] = typeof(V);
            }
        }

        public T CreateInstance<T>() where T : new()
        {
            var a = new T();

            //注入此类内部属性
            KeyValuePair<Type, PropertyInfo>[] pairs = new KeyValuePair<Type, PropertyInfo>[0];
            object[] names = new object[0];
            /*
            MemberInfo[] privateMembers = a.GetType().FindMembers(MemberTypes.Property,
                                                    BindingFlags.FlattenHierarchy |
                                                    BindingFlags.SetProperty |
                                                    BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance,
                                                    null, null);
             */

            MemberInfo[] privateMembers = a.GetType().FindMembers(MemberTypes.Property,
                                                   BindingFlags.SetProperty |
                                                   BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance,
                                                   null, null);

            foreach (MemberInfo member in privateMembers)
            {
                object[] injections = member.GetCustomAttributes(typeof(InjectAttribute), true);
                if (injections.Length > 0)
                {
                    PropertyInfo point = member as PropertyInfo;
                    Type pointType = point.PropertyType;
                    point.SetValue(a, Activator.CreateInstance(BindCache[pointType]), null);
                }
            }
            return a;
        }


        public void Inject(object a)
        {
            //注入此类内部属性
            KeyValuePair<Type, PropertyInfo>[] pairs = new KeyValuePair<Type, PropertyInfo>[0];
            object[] names = new object[0];
          
            MemberInfo[] privateMembers = a.GetType().FindMembers(MemberTypes.Property,
                                                    BindingFlags.FlattenHierarchy |
                                                    BindingFlags.SetProperty |
                                                    BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance,
                                                    null, null);
           

         

            foreach (MemberInfo member in privateMembers)
            {
                //是否有自定定义附属属性
                object[] injections = member.GetCustomAttributes(typeof(InjectAttribute), true); 
                if (injections.Length > 0)
                {
                    PropertyInfo point = member as PropertyInfo;
                    Type pointType = point.PropertyType;
                    if (InjectType.NON_SINGLETON == (injections[0] as InjectAttribute).injectType)
                    {
                        point.SetValue(a, Activator.CreateInstance(BindCache[pointType]), null);
                    }
                    else 
                    {                 
                        System.Reflection.PropertyInfo propertyInfo = BindCache[pointType].GetProperty("Instance");

                        point.SetValue(a, propertyInfo.GetValue(null,null), null);
                    }
                   
                }
            }
           
        }


        public void InjectStatic(Type aType)
        {
            //注入此类内部属性
            KeyValuePair<Type, PropertyInfo>[] pairs = new KeyValuePair<Type, PropertyInfo>[0];
            object[] names = new object[0];

            MemberInfo[] privateMembers = aType.FindMembers(MemberTypes.Property,
                                                    BindingFlags.FlattenHierarchy |
                                                    BindingFlags.SetProperty |
                                                    BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Static,
                                                    null, null);




            foreach (MemberInfo member in privateMembers)
            {
                //是否有自定定义附属属性
                object[] injections = member.GetCustomAttributes(typeof(InjectAttribute), true);
                if (injections.Length > 0)
                {
                    PropertyInfo point = member as PropertyInfo;
                    Type pointType = point.PropertyType;
                    if (InjectType.NON_SINGLETON == (injections[0] as InjectAttribute).injectType)
                    {
                        point.SetValue(aType, Activator.CreateInstance(BindCache[pointType]), null);
                    }
                    else
                    {
                        System.Reflection.PropertyInfo propertyInfo = BindCache[pointType].GetProperty("Instance");

                        point.SetValue(aType, propertyInfo.GetValue(null, null), null);
                    }

                }
            }

        }
    }
}
