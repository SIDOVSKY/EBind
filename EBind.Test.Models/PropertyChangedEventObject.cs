using System;

namespace EBind.Test.Models
{
    public class PropertyChangedEventObject
    {
        private string _stringValue = "";

        private EventHandler? _stringValueChanged;

        public event EventHandler? StringValueChanged
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
                _stringValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int StringValueChangedCount { get; private set; }
    }
}
