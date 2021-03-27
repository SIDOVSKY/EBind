using System;

namespace EBind.Tests
{
    internal class ActionEventObject
    {
        private string _stringValue = "";

        private Action<string, string>? _stringValueChanged;

        public event Action<string, string>? StringValueChanged
        {
            add
            {
                _stringValueChanged += value;
                StringValueChangedCount++;
            }
            remove
            {
                _stringValueChanged -= value;
                StringValueChangedCount--;
            }
        }

        public string StringValue
        {
            get => _stringValue;
            set
            {
                if (_stringValue == value)
                    return;

                var oldValue = _stringValue;
                _stringValue = value;
                _stringValueChanged?.Invoke(oldValue, _stringValue);
            }
        }

        public int StringValueChangedCount { get; private set; }
    }
}
