using System;
using System.ComponentModel;

namespace EBind.Test.Models
{
    public class NotifyPropertyChangedEventObject : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler? _propertyChanged;

        private int _intValue;
        private float _floatValue;
        private string? _stringValue = "";
        private bool _boolValue;
        private DayOfWeek _enumValue;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
                PropertyChangedCount++;
            }
            remove
            {
                _propertyChanged -= value;
                PropertyChangedCount--;
            }
        }

        public int PropertyChangedCount { get; private set; }

        public int IntValue
        {
            get => _intValue;
            set
            {
                if (_intValue == value)
                    return;

                _intValue = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IntValue)));
            }
        }

        public float FloatValue
        {
            get => _floatValue;
            set
            {
                if (_floatValue == value)
                    return;

                _floatValue = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FloatValue)));
            }
        }

        public string? StringValue
        {
            get => _stringValue;
            set
            {
                if (_stringValue == value)
                    return;

                _stringValue = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringValue)));
            }
        }

        public bool BoolValue
        {
            get => _boolValue;
            set
            {
                if (_boolValue == value)
                    return;

                _boolValue = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BoolValue)));
            }
        }

        public DayOfWeek EnumValue
        {
            get => _enumValue;
            set
            {
                if (_enumValue == value)
                    return;

                _enumValue = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnumValue)));
            }
        }
    }
}
