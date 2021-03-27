<p align="center">
  <br>
  <img src="Assets/logo.svg" alt="EBind" height="150"><br>
  <br>
  <a href="https://github.com/SIDOVSKY/EBind/actions/workflows/ci.yml">
    <img src="https://github.com/SIDOVSKY/EBind/actions/workflows/ci.yml/badge.svg?branch=main"alt="CI">
  </a>
  <a href="https://www.nuget.org/packages/EBind.NET/">
    <img src="https://img.shields.io/nuget/v/EBind.NET?logo=nuget" alt="nuget: EBind">
  </a>
  <a href="https://www.nuget.org/packages/EBind.LinkerIncludeGenerator/">
    <img src="https://img.shields.io/nuget/v/EBind.LinkerIncludeGenerator?label=nuget%20%7C%20EBind.LinkerIncludeGenerator&logo=nuget" alt="nuget: EBind.LinkerIncludeGenerator">
  </a>
  <a href="https://codecov.io/gh/SIDOVSKY/EBind">
    <img src="https://img.shields.io/codecov/c/gh/sidovsky/ebind?label=coverage%20%28strict%29&logo=codecov" alt="Codecov">
  </a>
</p>

<!-- snippet: Bind -->
<a id='snippet-bind'></a>
```cs
var binding = new EBinding
{
    () => view.Text == vm.Text,
    () => view.Text == vm.Description.Title.Text,
    () => view.Text == (vm.Text ?? vm.FallbackText),
    () => view.Visible == !vm.TextVisible,
    () => view.Visible == (vm.TextVisible == vm.ImageVisible),
    () => view.Visible == (vm.TextVisible && vm.ImageVisible),
    () => view.Visible == (vm.TextVisible || vm.ImageVisible),
    () => view.FullName == $"{vm.FirstName} {vm.LastName}",
    () => view.FullName == vm.FirstName + " " + vm.LastName,
    () => view.Timestamp == Converter.DateTimeToEpoch(vm.DateTime),

    BindFlag.TwoWay,
    () => view.Text == vm.Text,
    () => view.SliderValueFloat == vm.AgeInt,

    BindFlag.OneTime | BindFlag.NoInitialTrigger,
    () => view.ShowImage(vm.ImageUri),
    () => Dispatcher.RunOnUiThread(() => view.ShowImage(vm.ImageUri)),

    BindFlag.OneTime,
    (view, nameof(view.Click), vm.OnViewClicked),
    (view, nameof(view.TextEditedEventHandler), () => vm.OnViewTextEdited(view.Text)),
    (view, "CustomEventForGesture", vm.OnViewClicked),

    (view, nameof(view.Click), vm.ViewClickCommand),
    (view, nameof(view.Click), () => vm.ViewClickCommand.TryExecute(view.Text)),

    new UserExtensions.CustomEBinding(),
};

binding.Dispose();
```
<sup><a href='/EBind.Tests/Snippets/Sample.cs#L64-L98' title='Snippet source file'>snippet source</a> | <a href='#snippet-bind' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Three types of bindings here

* <a id='equality'>**EQUALITY**</a>
  ```cs
  () => view.Text == vm.Text,
  ```
* <a id='action'>**ACTION**</a>
  ```cs
  () => view.ShowImage(vm.ImageUri),
  ```
* <a id='event'>**EVENT**</a>
  ```cs
  (view, nameof(View.Click), vm.OnViewClicked),
  ```

### Key points

- <a name="main-update-trigger"></a>
  **The main binding update trigger is [`INotifyPropertyChanged.PropertyChanged`](https://docs.microsoft.com/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged)**.  
  Boilerplate code may be avoided with [Fody.PropertyChanged](https://github.com/Fody/PropertyChanged).  
  There are some platform-specific [pre-configured triggers](#pre-configured-triggers). Additional ones are easy to [configure](#member-triggers).

- **Bindings invoke immediately after the construction, except for the [event binding](#event).**  
  To skip initial invocation prepend `BindFlag.NoInitialTrigger`.

- **Each property and field in a binding expression triggers the binding update**  
  ![Triggers Highlight](Assets/triggers-highlight.svg)

- **Default binding mode is One-Way, Source-to-Target (right to left).**  
  This is the most common use-case. Explicit declaration of a two-way mode will help to avoid unreasonable performance loss.  
  May be overridden in `EBinding.DefaultConfiguration.DefaultFlag`.

- **For Target-to-Source binding simply swap operands.**  
  It seems more natural than bringing in an additional `BindFlag.OneWayToSource`.

- **Expressions of any complexity are supported.**  
  Even MONSTERS like that:
  ```cs
  () => view.Text == new Func<string?>(() => new Dictionary<string, string?>((int)10.0){ { "Key", vm.Text } }["Key"])(),
  ```
  However, the more complex the expression, the longer it takes to create and invoke the binding from it. See [benchmarks](#benchmark-ebind-creation) for the reference.

- **`BindFlag` applies to all subsequent bindings till the next flag.**

## Configuration and Extensibility

### Pre-Configured Triggers

<details><summary>Xamarin.Android</summary>

  | View/Control                  | Event           | Property                                           |
  |-------------------------------|-----------------|----------------------------------------------------|
  | Android.Views.View            | Click           |                                                    |
  |                               | LongClick       |                                                    |
  |                               | FocusChange     |                                                    |
  | Android.Widget.AdapterView    | ItemSelected    | SelectedItemPosition                               |
  |                               | ItemClick       |                                                    |
  |                               | ItemLongClick   |                                                    |
  | Android.Widget.CalendarView   | DateChange      | Date                                               |
  | Android.Widget.CompoundButton | CheckedChange   | Checked                                            |
  | Android.Widget.DatePicker     | DateChanged     | DateTime                                           |
  | Android.Widget.NumberPicker   | ValueChanged    | Value                                              |
  | Android.Widget.RatingBar      | RatingBarChange | Rating                                             |
  | Android.Widget.SearchView     | QueryTextChange | Query                                              |
  | Android.Widget.SeekBar        | ProgressChanged | Progress                                           |
  | Android.Widget.TextView       | TextChanged     | Text                                               |
  | Android.Widget.TimePicker     | TimeChanged     | Hour (API 23+)<br>Minute (API 23+)<br>CurrentHour<br>CurrentMinute |
</details>

<details><summary>Xamarin.iOS</summary>

  | View/Control       | Event                  | Property        |
  |--------------------|------------------------|-----------------|
  | UIBarButtonItem    | Clicked                |                 |
  | UIControl          | TouchUpInside          |                 |
  |                    | ValueChanged           |                 |
  | UIDatePicker       | ValueChanged           | Date            |
  | UIPageControl      | ValueChanged           | CurrentPage     |
  | UISearchBar        | TextChanged            | Text            |
  | UISegmentedControl | ValueChanged           | SelectedSegment |
  | UISlider           | ValueChanged           | Value           |
  | UIStepper          | ValueChanged           | Value           |
  | UISwitch           | ValueChanged           | On              |
  | UITabBarController | ViewControllerSelected | SelectedIndex   |
  | UITextField        | EditingChanged         | Text            |
  |                    | EditingDidBegin        |                 |
  |                    | EditingDidEnd          |                 |
  | UITextView         | Changed                | Text            |
</details>

<details><summary>Xamarin.Forms</summary>

  All views implement `INotifyPropertyChanged` so the [main trigger](#main-update-trigger) is invoked for every bindable property change. 
</details>

### Member Triggers

In `Configuration` you may specify how to subscribe and unsubscribe for signals of property and field updates that are not tracked out of the box.  
There are overloads for the property/field update handler as `System.EventHandler`, `System.EventHandler<TEventArgs>` or any other class:

<!-- snippet: Configure-Member-Trigger -->
<a id='snippet-configure-member-trigger'></a>
```cs
EBinding.DefaultConfiguration.ConfigureTrigger<View, string>(
    v => v.Text,
    (v, h) => v.TextEditedEventHandler += h,
    (v, h) => v.TextEditedEventHandler -= h);

EBinding.DefaultConfiguration.ConfigureTrigger<View, View.TextEditedEventArgs, string>(
    v => v.Text,
    (v, h) => v.TextEditedGenericEventHandler += h,
    (v, h) => v.TextEditedGenericEventHandler -= h);

EBinding.DefaultConfiguration.ConfigureTrigger<View, Action<string>, string>(
    v => v.Text,
    trigger => _ => trigger(),
    (v, h) => v.TextEditedCustomEventHandler += h,
    (v, h) => v.TextEditedCustomEventHandler -= h);
```
<sup><a href='/EBind.Tests/Snippets/Sample.cs#L17-L33' title='Snippet source file'>snippet source</a> | <a href='#snippet-configure-member-trigger' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Event Triggers

You may configure your own triggers for [event bindings](#event) under custom identifiers even if they represent a Pub-Sub pattern differently from C# events (`IObservable`, `Add/RemoveListener` methods).

<!-- snippet: Configure-Custom-Event-Trigger -->
<a id='snippet-configure-custom-event-trigger'></a>
```cs
EBinding.DefaultConfiguration.ConfigureTrigger<View, View.GestureRecognizer>(
    "CustomEventForGesture",
    trigger => new View.GestureRecognizer(trigger),
    (v, h) => v.AddGestureRecognizer(h),
    (v, h) => v.RemoveGestureRecognizer(h));
```
<sup><a href='/EBind.Tests/Snippets/Sample.cs#L53-L59' title='Snippet source file'>snippet source</a> | <a href='#snippet-configure-custom-event-trigger' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The same identifier (event name) can be used with multiple classes.  
For example on Xamarin.iOS a [pre-defined](/EBind/Platform/Configuration.ios.cs#LC56:~:text=void%20ConfigureExtraEvents(),%7D) `Tap` event can be used with both `UIControl` and `UIView`:
```cs
using EBind;
using static EBind.Platform.Configuration.ExtraEventNames;

var binding = new EBinding
{
    (uiButton, Tap, OnButtonClick),
    (uiImageView, Tap, OnImageClick),
};
```

Configuration of C# events is **not required** – they can be found by the name: `nameof(obj.Event)`.  
But it's recommended to specify subscription and unsubscription delegates to improve cold-start performance and avoid [linker errors](#linking).

<!-- snippet: Configure-Event-Trigger -->
<a id='snippet-configure-event-trigger'></a>
```cs
EBinding.DefaultConfiguration.ConfigureTrigger<View>(
    nameof(View.TextEditedEventHandler),
    (v, h) => v.TextEditedEventHandler += h,
    (v, h) => v.TextEditedEventHandler -= h);

EBinding.DefaultConfiguration.ConfigureTrigger<View, View.TextEditedEventArgs>(
    nameof(View.TextEditedGenericEventHandler),
    (v, h) => v.TextEditedGenericEventHandler += h,
    (v, h) => v.TextEditedGenericEventHandler -= h);

EBinding.DefaultConfiguration.ConfigureTrigger<View, Action<string>>(
    nameof(View.TextEditedCustomEventHandler),
    trigger => _ => trigger(),
    (v, h) => v.TextEditedCustomEventHandler += h,
    (v, h) => v.TextEditedCustomEventHandler -= h);
```
<sup><a href='/EBind.Tests/Snippets/Sample.cs#L35-L51' title='Snippet source file'>snippet source</a> | <a href='#snippet-configure-event-trigger' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Event triggers configured for a class are available to all its children unless they have their own variation defined.

Triggers may be overwritten by onward setups including the pre-defined ones.

### Binding Dispatcher

Sometimes ViewModel properties are set from the background thread that leads to data-binding view update triggering in the non-ui thread.

Establishing a proper thread switching in place is not possible for some code. Also, the risk of missing a thread is not acceptable or not worth caring about for some projects, and delegation of this problem is a good deal.  
In this case, data-binding as a point of integration may be a good place to switch threads between the ui-threaded View and the thread-insensitive ViewModel layers.

Dispatchers which force the UI thread are set up in `Configuration`:
```cs
EBinding.DefaultConfiguration.AssignmentDispatchDelegate = Dispatcher.RunOnUiThread;
EBinding.DefaultConfiguration.ActionDispatchDelegate = Dispatcher.RunOnUiThread;
```

For Xamarin platform [`Xamarin.Essentials.MainThread.BeginInvokeOnMainThread`](https://docs.microsoft.com/en-us/xamarin/essentials/main-thread) will be the best option most of the time.

### Custom Bindings

EBinding collection initializer accepts any form of `IEBinding`.  
By implementing this simple interface you can create your own binding types and adapters for data-binding components of other systems (e.g. [Rx.Net](https://github.com/dotnet/reactive)), use them in the same collection initializer, and keep all bindings in one place.

Custom binding creation can be encapsulated inside an extension method `Add(this EBinding, ...)` so that the binding inputs are accepted in the collection initializer (supported since C# 6).

After a custom binding is created, it must be added to the collection via `EBinding.Add(IEBinding)` so that it can be disposed along with other bindings.

<!-- snippet: Custom-EBinding -->
<a id='snippet-custom-ebinding'></a>
```cs
public class CustomEBinding : IEBinding
{
    public void Dispose()
    {
    }
}

public static void Add(this EBinding bindingHolder, CustomEBinding binding,
    [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
{
    try
    {
        if (binding == null)
            throw new ArgumentNullException(nameof(binding));

        if (bindingHolder.CurrentFlag == BindFlag.OneTime)
            throw new NotSupportedException();

        bindingHolder.Add(binding);
    }
    catch (Exception ex)
    {
        throw new Exception(
            $"Error in entry {bindingHolder.Count} at line #{sourceLineNumber}", ex);
    }
}
```
<sup><a href='/EBind.Tests/Snippets/Sample.cs#L167-L194' title='Snippet source file'>snippet source</a> | <a href='#snippet-custom-ebinding' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

It's recommended to decorate exceptions during binding creation with additional information about the binding position (`EBinding.Count`) and its line number ([`CallerLineNumberAttribute`](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerlinenumberattribute)) as debuggers do not highlight that.

<details><summary>Exception screenshot</summary>

  ![Location info in exception](Assets/location_info_in_exception.png)
</details>

## Benchmarks

<details><summary>Environment</summary>

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.572 (2004/?/20H1)
AMD Ryzen 5 1600, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.102
  [Host]     : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  DefaultJob : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
```
</details>

<details open>
<summary>Comparison: Creation, One-Way <sup><a id='benchmark-comparison-creation-one-way' href='#benchmark-comparison-creation-one-way' title='Anchor'>🔗</a></sup></summary>

|         Method |       Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|--------------- |-----------:|----------:|----------:|------:|--------:|-------:|----------:|
|          EBind |   3.953 us | 0.0252 us | 0.0223 us |  1.00 |    0.00 | 0.5264 |   2.18 KB |
|      MvvmLight |   6.960 us | 0.0644 us | 0.0602 us |  1.76 |    0.02 | 0.5951 |   2.45 KB |
|          Mugen |   7.659 us | 0.0973 us | 0.0910 us |  1.94 |    0.03 | 0.4501 |   1.97 KB |
|      MvvmCross |   9.236 us | 0.1007 us | 0.0942 us |  2.34 |    0.02 | 0.9155 |   3.78 KB |
|     ReactiveUI |  47.973 us | 0.9584 us | 1.0652 us | 12.09 |    0.29 | 3.4790 |  14.28 KB |
| PraeclarumBind | 330.993 us | 1.8638 us | 1.7434 us | 83.70 |    0.67 | 2.4414 |  10.37 KB |

<sup>[sources](EBind.Benchmarks/Comparison_Creation.cs)</sup>
</details>

<details>
<summary>Comparison: Creation, Two-Way <sup><a id='benchmark-comparison-creation-two-way' href='#benchmark-comparison-creation-two-way' title='Anchor'>🔗</a></sup></summary>

|         Method |       Mean |     Error |    StdDev |  Ratio | RatioSD |  Gen 0 | Allocated |
|--------------- |-----------:|----------:|----------:|-------:|--------:|-------:|----------:|
|          EBind |   4.815 us | 0.0130 us | 0.0121 us |   1.00 |    0.00 | 0.7553 |   3.11 KB |
|          Mugen |   7.413 us | 0.0904 us | 0.0846 us |   1.54 |    0.02 | 0.4883 |   2.02 KB |
|      MvvmLight |   8.658 us | 0.0986 us | 0.0874 us |   1.80 |    0.02 | 0.7782 |   3.21 KB |
|      MvvmCross |  12.754 us | 0.1306 us | 0.1221 us |   2.65 |    0.03 | 1.0529 |   4.34 KB |
|     ReactiveUI |  79.358 us | 1.0734 us | 1.0041 us |  16.48 |    0.23 | 6.3477 |  26.06 KB |
| PraeclarumBind | 615.675 us | 3.5524 us | 3.3229 us | 127.86 |    0.81 | 3.9063 |  19.09 KB |

<sup>[sources](EBind.Benchmarks/Comparison_Creation_TwoWay.cs)</sup>
</details>

<details open>
<summary>Comparison: Trigger <sup><a id='benchmark-comparison-trigger' href='#benchmark-comparison-trigger' title='Anchor'>🔗</a></sup></summary>

|         Method |          Mean |      Error |     StdDev |    Ratio | RatioSD |  Gen 0 | Allocated |
|--------------- |--------------:|-----------:|-----------:|---------:|--------:|-------:|----------:|
|          EBind |      70.48 ns |   0.631 ns |   0.591 ns |     1.00 |    0.00 | 0.0305 |     128 B |
|          Mugen |     189.46 ns |   1.709 ns |   1.334 ns |     2.69 |    0.02 | 0.0172 |      72 B |
|      MvvmLight |   1,028.04 ns |  13.442 ns |  12.574 ns |    14.59 |    0.19 | 0.1259 |     528 B |
|      MvvmCross |   1,313.41 ns |  10.489 ns |   9.811 ns |    18.64 |    0.16 | 0.1678 |     704 B |
|     ReactiveUI |   2,810.14 ns |  26.981 ns |  25.238 ns |    39.88 |    0.53 | 0.1678 |     713 B |
| PraeclarumBind | 162,013.06 ns | 918.216 ns | 813.974 ns | 2,299.76 |   23.26 | 0.9766 |    4415 B |

<sup>[sources](EBind.Benchmarks/Comparison_Trigger.cs)</sup>
</details>

<details>
<summary>Comparison: Cold Start <sup><a id='benchmark-comparison-cold-start' href='#benchmark-comparison-cold-start' title='Anchor'>🔗</a></sup></summary>

`IterationCount=1  LaunchCount=100  RunStrategy=ColdStart`

|     Type |         Method |         Mean |       Error |      StdDev | Ratio | RatioSD | Allocated |
|--------- |--------------- |-------------:|------------:|------------:|------:|--------:|----------:|
| Creation |          EBind |  13,184.0 us |   479.24 us | 1,413.05 us |  1.00 |    0.00 |    3504 B |
| Creation |      MvvmLight |  13,990.8 us |    79.85 us |   235.45 us |  1.07 |    0.06 |    3216 B |
| Creation |      MvvmCross |  18,360.7 us |    71.51 us |   210.85 us |  1.40 |    0.08 |    6144 B |
| Creation | PraeclarumBind |  20,246.7 us |   108.30 us |   319.33 us |  1.54 |    0.09 |   12312 B |
| Creation |          Mugen |  70,996.2 us |   231.94 us |   683.88 us |  5.42 |    0.30 |    7472 B |
| Creation |     ReactiveUI | 136,189.6 us | 2,543.53 us | 7,499.65 us | 10.36 |    0.32 |   15976 B |
|  Trigger |          EBind |     671.4 us |     7.92 us |    23.34 us |  0.05 |    0.00 |     224 B |
|  Trigger |      MvvmCross |   1,090.3 us |    13.85 us |    40.83 us |  0.08 |    0.01 |     704 B |
|  Trigger |      MvvmLight |   1,956.3 us |    37.87 us |   111.66 us |  0.15 |    0.01 |     608 B |
|  Trigger |     ReactiveUI |   3,238.9 us |    23.93 us |    70.55 us |  0.25 |    0.01 |     728 B |
|  Trigger | PraeclarumBind |   3,974.3 us |    49.39 us |   145.61 us |  0.30 |    0.02 |    5000 B |
|  Trigger |          Mugen |   4,130.2 us |    44.99 us |   132.65 us |  0.32 |    0.02 |     152 B |
</details>

<details>
<summary>EBind Creation <sup><a id='benchmark-ebind-creation' href='#benchmark-ebind-creation' title='Anchor'>🔗</a></sup></summary>

|                                       Method |        Mean |     Error |    StdDev |  Gen 0 | Allocated |
|--------------------------------------------- |------------:|----------:|----------:|-------:|----------:|
|             `(a, "nameof(a.Event)", Method)` |    337.5 ns |   2.31 ns |   1.93 ns | 0.0858 |     360 B |
|                                 `a.Method()` |  3,256.9 ns |  20.82 ns |  18.46 ns | 0.4349 |    1824 B |
|                   `a.Prop == b.Prop` // INPC |  4,089.8 ns |  54.06 ns |  50.57 ns | 0.5264 |    2232 B |
|                  `a.Method(b.Prop.Method())` |  4,406.1 ns |  20.41 ns |  17.05 ns | 0.5493 |    2312 B |
|                          `a.Prop == !b.Prop` |  4,611.5 ns |  46.32 ns |  43.33 ns | 0.5646 |    2392 B |
|           `a.Prop == b.Prop` // EventHandler |  4,658.2 ns |  46.37 ns |  43.37 ns | 0.5112 |    2152 B |
|                           `a.Float == b.Int` |  4,785.8 ns |  45.54 ns |  38.03 ns | 0.5798 |    2448 B |
|                           `a.Enum == b.Enum` |  5,265.1 ns |  22.89 ns |  21.42 ns | 0.5493 |    2328 B |
|            `a.Prop == Static.Method(b.Prop)` |  6,325.2 ns |  67.12 ns |  62.78 ns | 0.6790 |    2864 B |
|               `a.Prop == (b.Prop && c.Prop)` |  6,423.7 ns |  21.64 ns |  18.07 ns | 0.8087 |    3408 B |
|               `a.Prop == (b.Prop == c.Prop)` |  6,535.5 ns |  49.44 ns |  46.25 ns | 0.8163 |    3432 B |
|             `a.Prop == (b.Prop \|\| c.Prop)` |  6,653.2 ns |  54.95 ns |  51.40 ns | 0.8163 |    3432 B |
|                 `a.Prop == b.Method(c.Prop)` |  6,823.8 ns |  60.00 ns |  56.13 ns | 0.7553 |    3184 B |
|               `a.Prop == (b.Prop ?? c.Prop)` |  7,271.6 ns |  53.72 ns |  50.25 ns | 0.8392 |    3536 B |
|         `a.Prop == b.Method(c.Prop, d.Prop)` |  7,357.3 ns |  50.22 ns |  46.98 ns | 0.8011 |    3360 B |
| `a.Prop == b.Method(c.Prop, d.Prop, e.Prop)` |  7,552.6 ns |  40.44 ns |  37.83 ns | 0.8392 |    3536 B |
|                  `a.Prop == b.Prop + c.Prop` |  7,743.0 ns |  77.14 ns |  64.41 ns | 0.8850 |    3736 B |
|             `a.Prop == $"{b.Prop}_{c.Prop}"` |  9,898.2 ns |  20.60 ns |  16.08 ns | 0.9918 |    4176 B |
|       `a.Prop == (b.Prop + c.Prop).Method()` | 12,735.2 ns |  71.11 ns |  63.04 ns | 0.9155 |    3880 B |
|        `Static.Method(() => Method(a.Prop))` | 16,007.6 ns | 131.27 ns | 116.37 ns | 1.2512 |    5344 B |

<sup>[sources](EBind.Benchmarks/EBind_Creation.cs)</sup>
</details>

<details>
<summary>EBind Trigger <sup><a id='benchmark-ebind-trigger' href='#benchmark-ebind-trigger' title='Anchor'>🔗</a></sup></summary>

|                                       Method |      Mean |    Error |   StdDev |  Gen 0 | Allocated |
|--------------------------------------------- |----------:|---------:|---------:|-------:|----------:|
|                          `a.Prop == !b.Prop` |  69.32 ns | 0.879 ns | 0.822 ns | 0.0305 |     128 B |
|                   `a.Prop == b.Prop` // INPC |  72.64 ns | 0.810 ns | 0.758 ns | 0.0305 |     128 B |
|                                 `a.Method()` |  79.61 ns | 0.653 ns | 0.611 ns | 0.0248 |     104 B |
|                           `a.Enum == b.Enum` |  80.15 ns | 1.008 ns | 0.943 ns | 0.0362 |     152 B |
|           `a.Prop == b.Prop` // EventHandler |  82.12 ns | 0.523 ns | 0.489 ns | 0.0076 |      32 B |
|             `a.Prop == (b.Prop \|\| c.Prop)` |  85.83 ns | 0.672 ns | 0.628 ns | 0.0362 |     152 B |
|               `a.Prop == (b.Prop && c.Prop)` |  88.38 ns | 1.761 ns | 1.884 ns | 0.0362 |     152 B |
|               `a.Prop == (b.Prop == c.Prop)` | 104.50 ns | 1.162 ns | 1.087 ns | 0.0362 |     152 B |
|               `a.Prop == (b.Prop ?? c.Prop)` | 115.92 ns | 0.883 ns | 0.826 ns | 0.0134 |      56 B |
|                  `a.Prop == b.Prop + c.Prop` | 122.91 ns | 1.117 ns | 1.045 ns | 0.0200 |      84 B |
|                           `a.Float == b.Int` | 128.55 ns | 0.769 ns | 0.719 ns | 0.0362 |     152 B |
|                  `a.Method(b.Prop.Method())` | 128.60 ns | 1.223 ns | 1.144 ns | 0.0324 |     136 B |
|            `a.Prop == Static.Method(b.Prop)` | 168.87 ns | 2.003 ns | 1.873 ns | 0.0267 |     112 B |
|                 `a.Prop == b.Method(c.Prop)` | 261.03 ns | 1.913 ns | 1.597 ns | 0.0286 |     120 B |
|         `a.Prop == b.Method(c.Prop, d.Prop)` | 265.79 ns | 3.130 ns | 2.928 ns | 0.0305 |     128 B |
| `a.Prop == b.Method(c.Prop, d.Prop, e.Prop)` | 297.89 ns | 2.282 ns | 2.135 ns | 0.0324 |     136 B |
|             `a.Prop == $"{b.Prop}_{c.Prop}"` | 351.95 ns | 2.445 ns | 2.042 ns | 0.0362 |     152 B |
|       `a.Prop == (b.Prop + c.Prop).Method()` | 417.61 ns | 3.072 ns | 2.724 ns | 0.0515 |     216 B |

<sup>[sources](EBind.Benchmarks/EBind_Trigger.cs)</sup>
</details>

## Linking

The library is linker-safe internally, but some exposed APIs rely on Linq Expression trees and therefore the reflection which have always been hard to process for the [mono linker](https://github.com/mono/linker).

Although linker can analyze expression trees and some reflection patterns [pretty well](https://github.com/mono/linker/blob/main/docs/design/reflection-flow.md), the following code units may not be mentioned in the code, appear unused and end up trimmed away:
* Property setters
* Events *(which are not configured)*  

The most common solution for hinting the linker to keep a member is to imitate its usage with a dummy call and mark it with a `[Preserve]` attribute. Your project may already have a `LinkerPleaseInclude.cs` file for that purpose.

[**EBind.LinkerIncludeGenerator**](EBind.LinkerIncludeGenerator) will generate such files for the mentioned members used in `EBinding` and there wont be any `EBind`-related linker issues in your project.  
Adding its NuGet package is enough for the installation:
<sub>[![NuGet](https://img.shields.io/nuget/v/EBind.LinkerIncludeGenerator?logo=nuget)](https://www.nuget.org/packages/EBind.LinkerIncludeGenerator/)</sub>

## AOT Compilation <sub>[![PLATFORM](https://img.shields.io/badge/platform-Xamarin.iOS-lightgrey)](#aot-compilation-)</sub>

This library uses C# 9 function pointers to create fast delegates for property accessors. It's the safest solution for AOT compilation so far.  
However, Xamarin.iOS AOT compiler used for device builds requires a direct indication of value-types as a generic type parameter for them.  
All standard structs are [pre-seeded](/EBind/Platform/AotCompilerHints.ios.cs).  

If you came across an exception like that:
```
System.ExecutionEngineException:
  Attempting to JIT compile method 'object EBind.PropertyAccessors.PropertyAccessor`2<..., ...>:Get (object)' while running in aot-only mode.
```
please add a hint for the compiler:
```cs
EBind.Platform.AotCompilerHints.Include<MyStruct>(); // custom struct as a member
// or
EBind.Platform.AotCompilerHints.Include<MyStruct, PropertyType>(); // custom struct as a target
```

## Contributions

If you've found an error, please file an issue.

Patches are encouraged and may be submitted by forking this project and submitting a pull request.  
If your change is substantial, please raise an issue or start a discussion first.

## Development

### Requirements

* C# 9 and .NET 5 workload
* Xamarin.Android and Xamarin.iOS SDKs for device testing

### Device Testing

Device tests may run manually from the test app UI or automatically with [XHarness CLI](https://github.com/dotnet/xharness).

Android:
```bash
xharness android test \
  --app="./EBind.Tests.Android/bin/Release/EBind.Tests.Android-Signed.apk" \
  --package-name="EBind.Tests.Android" \
  --instrumentation="ebind.tests.droid.xharness_instrumentation" \
  --device-arch="x86" \
  --output-directory="./EBind.Tests.Android/TestResults/xharness_android" \
  --verbosity
```

iOS:
```bash
xharness apple test \
  --app="./EBind.Tests.iOS/bin/iPhoneSimulator/Release/EBind.Tests.iOS.app" \
  --output-directory="./EBind.Tests.iOS/TestResults/xharness_apple" \
  --targets=ios-simulator-64 \
  --verbosity \
  -- -app-arg:xharness # to be passed in Application.Main(string[])
```

It's also a part of the github actions workflow. [Check it out!](.github/workflows/ci.yml)

## The Story

Once upon a time, there was a small but powerful library called `Bind` from the Praeclarum family.  

No other library could be compared with it in terms of concision and beauty. Every developer was dreaming about using it in his project. But it had several problems and missed some optimizations. The bravest of us could dare to use it in production.  

Beauty was stronger than fear and this is how another fork has begun.  
Bugs were fixed, some optimizations were made but it was never enough. That library deserved to SHINE!

So...

- Factory method `Create` replaced with a collection initializer to make bindings a separate block and utilize more syntax variations.
- Binding chaining became redundant, operator `&&` can be used in binding expressions now.
- Added flags that alter binding behavior, as a part of the collection initializer.
- Added support for binding properties/fields to functions ([Action bindings](#action)).
- Added support for binding functions and commands to events ([Event bindings](#event)).
- Binding triggers made configurable, all the basic ones for Xamarin are pre-defined.
- Manual invalidation doesn't seem necessary anymore – removed.
- Thread-safety improved by the means of dispatchers and concurrent collections.
- Performance brought to another level with expression interpretation, fast delegates (incl. C# 9 function pointers), and a different architecture.
- The library became more strict and informative in terms of exceptions and errors.
- Added a Source Generator that hints the mono linker about code usage.
- `Unbind` replaced by `IDisposable`, useful for aggregation (e.g. `CompositeDisposable`).

It ended up being completely rewritten with only some tests remained.

## Credits

* [@praeclarum](https://github.com/praeclarum) & [Praeclarum.Bind](https://github.com/praeclarum/Bind) - :heart: Code and Inspiration

## License

This project is licensed under the Apache License, Version 2.0 - see the [LICENSE](LICENSE) and [NOTICE](NOTICE) files for details.
