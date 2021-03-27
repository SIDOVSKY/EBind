using System;

namespace EBind.Test.Models
{
    public class HierarchyObject0
    {
        private int _property;

        public int Property
        {
            get => _property;
            set
            {
                if (_property != value)
                {
                    _property = value;
                    ChangedProperty?.Invoke(value);
                }
            }
        }

        public event Action<int>? ChangedProperty;
    }

    public class HierarchyObject1 : HierarchyObject0 { }
    public class HierarchyObject2 : HierarchyObject1 { }
    public class HierarchyObject3 : HierarchyObject2 { }
}
