using System;

namespace Game.Data
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ComponentContextAttribute : Attribute
    {
        public string attributeName;
        public string changeValueMethodName;
        public bool uneditable;

        public ComponentContextAttribute(string attributeName, string changeValueMethod, bool uneditable = false)
        {
            this.attributeName = attributeName;
            this.changeValueMethodName = changeValueMethod;
            this.uneditable = uneditable;
        }
    }
}
