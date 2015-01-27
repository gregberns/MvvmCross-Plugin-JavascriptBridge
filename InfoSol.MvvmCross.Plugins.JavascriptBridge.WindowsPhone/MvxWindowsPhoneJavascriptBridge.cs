using System;
using System.Collections.Generic;
using Microsoft.Phone.Controls;

namespace InfoSol.MvvmCross.Plugins.JavascriptBridge.WindowsPhone
{
    public class MvxWindowsPhoneJavascriptBridge : IMvxJavascriptBridge
    {
        private readonly WebBrowser _browser;

        public MvxWindowsPhoneJavascriptBridge(WebBrowser browser)
        {
            _browser = browser;
            _browser.IsScriptEnabled = true;
            _browser.IsGeolocationEnabled = true;
        }
        
        public void ShowWebPage(string url, Action onLoadCompleted)
        {
            _browser.Navigate(new Uri(url));
        }

        public void InjectScript(string script)
        {
            //_browser.Source
        }

        public object InvokeScript(string name, params string[] args)
        {
            return _browser.InvokeScript(name, args);
        }

        private Action<string, object> _target;
        public void AttachListener(Action<string, object> target)
        {
            _browser.ScriptNotify += browser_ScriptNotify;
            _target = target;
        }

        private void browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            _target(null, e.Value);
        }

        public void LoadLocalPage(string htmlPage, Action onLoadCompleted)
        {
            _browser.NavigateToString(htmlPage);

            _browser.LoadCompleted += (sender, args) => { onLoadCompleted(); };
        }

        public string AddErrorHandling(string htmldoc)
        {
            return "";
        }

        public List<string> GetErrors()
        {
            return new List<string>();
        }
    }

    public class JavascriptError
    {
        public string ErrorMessage { get; set; }
        public object Document { get; set; }
        public string LineNumber { get; set; }
    }
}
