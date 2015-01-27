using System;
using System.Collections.Generic;

namespace InfoSol.MvvmCross.Plugins.JavascriptBridge
{
    public interface IMvxJavascriptBridge
    {
        void ShowWebPage(string url, Action onLoadCompleted);
        void InjectScript(string script);
        object InvokeScript(string name, params string[] args);
        void AttachListener(Action<string, object> target);
        void LoadLocalPage(string htmlPage, Action onLoadCompleted);
        string AddErrorHandling(string htmldoc);
        List<string> GetErrors();
    }
}