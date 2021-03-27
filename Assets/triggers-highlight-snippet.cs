//https://carbon.now.sh/?bg=rgba%280%2C0%2C0%2C0%29&t=vscode&wt=none&l=text%2Fx-csharp&ds=false&dsyoff=20px&dsblur=68px&wc=false&wa=true&pv=0px&ph=0px&ln=false&fl=1&fm=Hack&fs=14px&lh=133%25&si=false&es=1x&wm=false&code=var%2520binding%2520%253D%2520new%2520EBinding%250A%257B%250A%2520%2520%2520%2520%28%29%2520%253D%253E%2520view.Date%2520%253D%253D%2520Convert%28vm.Year%252C%2520vm.Month%252C%2520vm.Day%29%252C%250A%2520%2520%2520%2520%28%29%2520%253D%253E%2520Dispatchers.Dispatcher.RunOnUiThread%28%28%29%2520%253D%253E%2520view.Image.Show%28vm.ImageUri%29%29%252C%250A%250A%2520%2520%2520%2520BindFlag.TwoWay%252C%250A%2520%2520%2520%2520%28%29%2520%253D%253E%2520view.Subview.Text%2520%253D%253D%2520vm.Decription.Title.Text%252C%250A%250A%2520%2520%2520%2520%28view%252C%2520nameof%28view.Click%29%252C%2520%28%29%2520%253D%253E%2520vm.ViewClickCommand.TryExecute%28view.Text%29%29%252C%250A%257D%253B

var binding = new EBinding
{
  () => view.Date == Convert(vm.Year, vm.Month, vm.Day),
  () => Dispatchers.Dispatcher.RunOnUiThread(() => view.Image.Show(vm.ImageUri)),

  BindFlag.TwoWay,
  () => view.Subview.Text == vm.Decription.Title.Text,

  (view, nameof(view.Click), () => vm.ViewClickCommand.TryExecute(view.Text)),
};
