# MvvmCross JavaScript Bridge Plugin

MvvmCross has a simple WebBrowser plugin that is great for displaying an external website.

But, in some cases, we'll need access to the "Javascript Bridge" or at least more control control over what happens in the browser. This project is an initial attempt to provide this support.

Somethings you might want to do with the browser control:

* Load an external web page
* Load local HTML file, including locally stored page resources
* Execute Browser Javascript functions from the Application, and return 'messages' after function execution
* Send 'messages' from the browser to the Application
* Add error handling hooks within the browser, so the application can notify the user of website/browser issues

NOTE:
* This is a work in progress... Any help would be appreciated, especially workflows not accounted for here.
* The WPF code is in the best shape. 
* The Android and WindowsPhone code needs work. 
* Any help to provide support for iOS would be much appreciated.


## Code Examples

### ViewModel Code

Simple example: 

	public class MainViewModel : BaseViewModel
    {
        public IMvxJavascriptBridge _browser;

        public void NewWebBrowserContainer(IMvxJavascriptBridge bridge)
        {
            _browser = bridge;
			
			//Load external page
			browser.ShowWebPage("SomeURL", () => OnBrowserLoadCompleted());
        }

        private void OnBrowserLoadCompleted()
        {	
			//Inject script into the browser
            _browser.InjectScript("SomeScript");
			
			//Attach a listener to recieve any responses from the browser
            _browser.AttachListener((funcName, obj) => {
				//Get callbacks here
            });

			//Invoke a specific browser script, a parameter array can also be passed in
            _browser.InvokeScript("SomeJSFunction", "arg1", "arg2");

            //Get the errors
            _browser.GetErrors();
        }
    }


### Windows Phone 8

Add a WebBrowser component to View.xaml:

	<Page xmlns:phone="clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone">
	<phone:WebBrowser x:Name="MyWebBrowser" Height="Auto" Width="Auto" />
	

In the code behind, View.xaml.cs, , 

	namespace MvvmCrossApp.WindowsPhone.Views
	{
	    public partial class MainView 
	    {
	        public MainView()
	        {
	            InitializeComponent();

	            // until loaded, we can not access viewmodel
	            Loaded += (sender, args) =>
	            {
	                var viewModel = (DashboardViewModel)ViewModel;
					
					WeBrowser browser = MyWebBrowser;

	                viewModel.NewWebBrowserContainer(new MvxWpfJavascriptBridge(browser));
	            };
	        }
	    }
	}


### WPF

Add a WebBrowser component to View.xaml:

	<WebBrowser x:Name="MyWebBrowser" Height="Auto" Width="Auto" />

In the code behind, View.xaml.cs, , 

	namespace MvvmCrossApp.Wpf.Views
	{
	    public partial class MainView 
	    {
	        public MainView()
	        {
	            InitializeComponent();

	            // until loaded, we can not access viewmodel
	            Loaded += (sender, args) =>
	            {
	                var viewModel = (DashboardViewModel)ViewModel;
					
					WeBrowser browser = MyWebBrowser;

	                viewModel.NewWebBrowserContainer(new MvxWpfJavascriptBridge(browser));
	            };
	        }
	    }
	}

### Other ViewModel Examples

Locally loaded HTML Page, which can contain loacl resources(i.e. JS, CSS, img)

    public class MainViewModel : BaseViewModel
    {
        ...

        public void NewWebBrowserContainer(IMvxJavascriptBridge bridge)
        {
            _browser = bridge;
			LoadLocalPage();            
        }

        private void LoadLocalPage()
        {
        	//This file would probably be retrieved from disk
			var HtmlFile = "<html><head></head><body><h1>Hello World!</h1></body>"

        	//Adding the 'base' element will allow you to access local files in WPF
            HtmlFile = HtmlFile.Replace("<head>",
                "<head><base href='file://C:\\IBE\\Deployed\\Dashboards\\1\\'>");

			//Here you can add scripts to the page, before its loaded
			//Use: If errors occur, you can collect them, then once loaded, the application can get them
            
            _browser.LoadLocalPage(HtmlFile, () => OnBrowserLoadCompleted());
        }

        private void OnBrowserLoadCompleted()
        {
        }
    }


## Currently Available Functions:

* ShowWebPage(string url, Action onLoadCompleted)
  * Supply an external URL for browser to load. Supply a function to do something once the page has loaded

* InjectScript(string script)
  * Inject Javascript into the browser. This can only occur after the page has loaded.

* InvokeScript(string functionName, params string[] args) - returns an object
  * Executes a script, with optional parameters, and returns a response.
  * The response may need to be a string, but could be a JSON string, that could then be convered to an object

* AttachListener(Action<string, object> target)
  * The action supplied will be invoked anytime the browser executes a specific function 'window.external.Notify()'

* LoadLocalPage(string htmlPage, Action onLoadCompleted)
  * Load a locally stored HTML file, and once the page has loaded call the suppled Action
  * This currently should work in WPF, but will need to be explored in other platforms

* AddErrorHandling(string htmldoc) returns a String
  * Used only for locally opened HTML pages
  * Add some Javascript code to the browser so when its loaded, if there are errors, they can be captured. Once loaded, the errors can be retrieved.
        
* GetErrors() return a List of String
  * Retrieve any browser errors after load

## Todo:
* Much of the code is application specific, so needs to be made more generic
* The Javascript functions need to be improved: where they are located, naming, etc.
* In WPF, if there are JS errors, there is an ugly popup displayed. The code disables the pop up right now. This may need to be a toggle, and we need to see if this is needed on other platforms.