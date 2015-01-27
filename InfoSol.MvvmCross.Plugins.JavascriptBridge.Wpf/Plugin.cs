using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace InfoSol.MvvmCross.Plugins.JavascriptBridge.Wpf
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterType<IMvxJavascriptBridge, MvxWpfJavascriptBridge>();
        }
    }
}