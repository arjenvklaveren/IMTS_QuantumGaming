using System;

namespace Game.Data
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ComponentContextAttribute : Attribute
    {
        public string attributeName;
        public string changeValueMethodName;

        public ComponentContextAttribute(string attributeName, string changeValueMethod)
        {
            this.attributeName = attributeName;
            this.changeValueMethodName = changeValueMethod;
        }
    }
}
