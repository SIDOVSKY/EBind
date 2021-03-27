namespace MvvmCross.Binding.BindingContext
{
    internal class MvxBindingContextOwner : IMvxBindingContextOwner
    {
        public IMvxBindingContext BindingContext { get; set; } = new MvxBindingContext();
    }
}
