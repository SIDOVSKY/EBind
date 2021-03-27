using EBind.Test.Models;
using UIKit;
using Xunit;

namespace EBind.Tests.iOS
{
    public class UITabBarControllerTriggerTest : ViewControllerFixture<UITabBarController>
    {
        [UIFact]
        public void UITabBarController_SelectedIndex_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                IntValue = 1
            };

            ViewController.ViewControllers = new[]
            {
                new UIViewController(), new UIViewController(), new UIViewController(),
            };

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => ViewController.SelectedIndex == vm.IntValue,
            };

            Assert.Equal(1, ViewController.SelectedIndex);

            ViewController.SelectedIndex = 2;
            ViewController.Delegate.ViewControllerSelected(ViewController, ViewController.ViewControllers[2]);

            Assert.Equal(2, vm.IntValue);
        }
    }
}