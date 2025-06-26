using System;

namespace FoodyGo.Utils.DI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        public string Key { get; }
        public InjectAttribute() { }
        public InjectAttribute(string key)
        {
            Key = key;
        }
    }
}
