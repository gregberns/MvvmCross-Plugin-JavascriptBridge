using System;
using System.Windows.Controls;
using NUnit.Framework;

namespace InfoSol.MvvmCross.Plugins.JavascriptBridge.Wpf.Test
{
    [TestFixture, RequiresSTA]
    public class MvxWpfJavascriptBridgeTest
    {
        private IMvxJavascriptBridge _bridge;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Ignore]
        public void Test1()
        {
            var changed = false;

            var _browser = new WebBrowser();
            _bridge = new MvxWpfJavascriptBridge(_browser);

            Action<string, object> BrowserCallBack = (func, obj) =>
            {
                var f = func;

                changed = true;

                Assert.AreEqual(true, changed);
            };

            Action PageLoaded = () =>
            {
                Assert.Fail();

                _bridge.InjectScript(@"function browserScript() { window.external.callJavascriptBridge(); }");

                _bridge.AttachListener(BrowserCallBack);

                _bridge.InvokeScript("browserScript");
            };


            //Navigate to a location
            _bridge.ShowWebPage("http://localhost:8554/dashoards/83", PageLoaded);


            //Assert.AreEqual(true, changed);

            // - Later - Go back and forward in the browser


            //Configure bridge

            //Inject HTML and Javascript

            //Interact through the bridge


            //_bridge.

            //Need Callback
            //Need way to make call
        }
    }
}