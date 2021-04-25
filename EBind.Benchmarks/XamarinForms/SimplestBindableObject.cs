using Xamarin.Forms;

namespace EBind.Benchmarks.XamarinForms
{
    internal class SimplestBindableObject : BindableObject
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            nameof(Value), typeof(int), typeof(SimplestBindableObject));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
