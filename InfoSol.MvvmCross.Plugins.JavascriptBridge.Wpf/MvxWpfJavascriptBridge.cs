using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace InfoSol.MvvmCross.Plugins.JavascriptBridge.Wpf
{
    public class MvxWpfJavascriptBridge : IMvxJavascriptBridge
    {
        private readonly WebBrowser _browser;

        public MvxWpfJavascriptBridge(WebBrowser webBrowser)
        {
            _browser = webBrowser;
        }

        //Create object that inherits Action, then it can have a 'FunctionName' and 'Object' property
        public void AttachListener(Action<string, object> target)
        {
            _browser.ObjectForScripting = new JavascriptBridge(target);
        }

        public void ShowWebPage(string url, Action onLoadCompleted)
        {
            HideScriptErrors(_browser, true);

            _browser.Navigate(url);

            _browser.LoadCompleted += (sender, args) => { onLoadCompleted(); };
        }

        public object InvokeScript(string name, params string[] args)
        {
            try
            {
                var o = _browser.InvokeScript(name, args);
                return o;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void InjectScript(string script)
        {
            //HtmlDocument doc = _browser.Document;
            dynamic doc = _browser.Document;

            //HtmlElement head = _browser.Document.GetElementsByTagName("head")[0];
            dynamic head = doc.GetElementsByTagName("head")[0];

            //HtmlElement scriptEl = _browser.Document.CreateElement("script");
            dynamic scriptEl = doc.CreateElement("script");

            scriptEl.text = script;

            head.AppendChild(scriptEl);
        }

        public string AddErrorHandling(string htmldoc)
        {
            //Catch any Javascript errors by injecting this script:
            var reportScriptError = @"<script>
                                            var windowJSErrors = [];
                                            function getWindowJSErrors(){
                                                return JSON.stringify(windowJSErrors);                                              
                                                //window.external.callJavascriptBridge('errors', JSON.stringify(windowJSErrors));
                                            };
                                            
                                            window.onerror = function(msg, url, line, col, error) {
                                                msg = msg || 'emptyMessage';
                                                var extra = !col ? '' : '\ncolumn: ' + col;
                                                extra += !error ? '' : '\nerror: ' + error;
                                                windowJSErrors.push('Error: ' + msg + '\nurl: ' + url + '\nline: ' + line + extra);
                                            };
                                            
                                            </script>";

            var h = htmldoc.IndexOf("</head>");

            htmldoc = htmldoc.Insert(h, reportScriptError);

            return htmldoc;
        }

        public List<string> GetErrors()
        {
            var jsonarray = (string) InvokeScript("getWindowJSErrors", null);
            var err = JsonConvert.DeserializeObject<List<string>>(jsonarray);

            return err;
        }

        public void LoadLocalPage(string htmlPage, Action onLoadCompleted)
        {
            HideScriptErrors(_browser, true);

            _browser.NavigateToString(htmlPage);

            _browser.LoadCompleted += (sender, args) => { onLoadCompleted(); };
        }

        private void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof (WebBrowser).GetField("_axIWebBrowser2",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are too early
                return;
            }

            objComWebBrowser.GetType()
                .InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] {hide});
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        [System.Runtime.InteropServices.ComVisible(true)]
        public class JavascriptBridge
        {
            private readonly Action<string, object> callback;

            public JavascriptBridge(Action<string, object> callback)
            {
                this.callback = callback;
            }

            public void Notify(string function, object obj)
            {
                callback(function, obj);
            }

            public void CallJSBOnError(string errorMessage, string document, string linenumber)
            {
                callback("ERROR",
                    new JavascriptError {ErrorMessage = errorMessage, Document = document, LineNumber = linenumber});
            }
        }
    }

    public class JavascriptError
    {
        public string ErrorMessage { get; set; }
        public object Document { get; set; }
        public string LineNumber { get; set; }
    }
}