using System;

namespace EBind.Tests
{
    internal class FuncEventObject
    {
        private string _stringValue = "";

        private Func<string, bool>? _stringValueChanged;

        public event Func<string, bool> StringValueChanged
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

                _stringValue = value;
                _stringValueChanged?.Invoke(_stringValue);
            }
        }

        public int StringValueChangedCount { get; private set; }
    }
}
