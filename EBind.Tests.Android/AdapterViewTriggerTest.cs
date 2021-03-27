using System.Threading.Tasks;
using Android.App;
using Android.Views;
using Android.Widget;
using EBind.Test.Models;
using Xunit;

namespace EBind.Tests.Droid
{
    public class AdapterViewTriggerTest : ActivityFixture<EmptyActivity>
    {
        [UIFact]
        public async Task AdapterView_SelectedItemPosition_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                IntValue = 1,
            };

            var spinner = new Spinner(Application.Context)
            {
                Adapter = new ArrayAdapter<string>(Application.Context, Android.Resource.Layout.SimpleSpinnerItem, new[]
                {
                    "0",
                    "1",
                    "2",
                    "3",
                }),
            };

            Activity.AddContentView(spinner, new ViewGroup.LayoutParams(500, 500));

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => spinner.SelectedItemPosition == vm.IntValue,
            };

            Assert.Equal(1, spinner.SelectedItemPosition);

            spinner.SetSelection(2);
            await spinner.PostAsync();

            Assert.Equal(2, vm.IntValue);
        }
    }
}