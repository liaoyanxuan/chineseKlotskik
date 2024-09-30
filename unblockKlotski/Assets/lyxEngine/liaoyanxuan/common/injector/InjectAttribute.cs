using System;
namespace liaoyanxuan.common.injector
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class InjectAttribute : Attribute
    {
        public InjectType injectType;
        public InjectAttribute(InjectType _injectType)
        {
            injectType = _injectType;
           // UnityEngine.
        }
        public InjectAttribute()
        {
            //UnityEngine.
        }
    }
}
