using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms.Internals
{
    internal class DummyPlatformServices : IPlatformServices
    {
        public bool IsInvokeRequired => false;
        public OSAppTheme RequestedTheme => throw new System.NotImplementedException();
        public string RuntimePlatform => throw new System.NotImplementedException();
        public void BeginInvokeOnMainThread(System.Action action) => throw new System.NotImplementedException();
        public Ticker CreateTicker() => throw new System.NotImplementedException();
        public Assembly[] GetAssemblies() => System.AppDomain.CurrentDomain.GetAssemblies();
        public string GetHash(string input) => throw new System.NotImplementedException();
        public string GetMD5Hash(string input) => throw new System.NotImplementedException();
        public Color GetNamedColor(string name) => throw new System.NotImplementedException();
        public double GetNamedSize(NamedSize size, System.Type targetElementType, bool useOldSizes) => throw new System.NotImplementedException();
        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint) => throw new System.NotImplementedException();
        public Task<Stream> GetStreamAsync(System.Uri uri, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        public IIsolatedStorageFile GetUserStoreForApplication() => throw new System.NotImplementedException();
        public void OpenUriAction(System.Uri uri) => throw new System.NotImplementedException();
        public void QuitApplication() => throw new System.NotImplementedException();
        public void StartTimer(System.TimeSpan interval, System.Func<bool> callback) => throw new System.NotImplementedException();
    }
}
