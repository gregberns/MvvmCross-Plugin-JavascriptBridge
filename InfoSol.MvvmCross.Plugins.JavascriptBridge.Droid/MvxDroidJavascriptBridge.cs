using System;
using System.Collections.Generic;
using Android;
using Android.Webkit;

namespace InfoSol.MvvmCross.Plugins.JavascriptBridge.Droid
{
    public class MvxDroidJavascriptBridge : IMvxJavascriptBridge
    {
        private readonly WebView _browser;

        public MvxDroidJavascriptBridge(WebView webView)
        {
            _browser = webView;

			WebSettings webSettings = _browser.Settings;
			webSettings.JavaScriptEnabled = true;
        }
        
        public void ShowWebPage(string url, Action onLoadCompleted)
        {
			_browser.LoadUrl(url);
        }

        public void InjectScript(string script)
        {

			//webView.addJavascriptInterface(new WebAppInterface(this), "Android");

            //throw new NotImplementedException();
        }

        public object InvokeScript(string name, params string[] args)
        {
			//_browser.AddJavascriptInterface ();

			return null;
        }

        public void AttachListener(Action<string, object> target)
        {
            //throw new NotImplementedException();
        }

        public void LoadLocalPage(string htmlPage, Action onLoadCompleted)
        {
			string summary = "<html><body>You scored <b>192</b> points.</body></html>";
			_browser.LoadData(summary, "text/html", null);
			// ... although note that there are restrictions on what this HTML can do.
			// See the JavaDocs for loadData() and loadDataWithBaseURL() for more info.		
		}

        public string AddErrorHandling(string htmldoc)
        {
            //throw new NotImplementedException();
			return "";
        }

        public List<string> GetErrors()
        {
            //throw new NotImplementedException();
			return new List<string> ();
        }
    }
}
