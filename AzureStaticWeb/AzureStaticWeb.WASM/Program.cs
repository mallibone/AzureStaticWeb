using System;
using System.IO;
using Xamarin.Forms;
using Ooui;

namespace AzureStaticWeb.WASM
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize Xamarin.Forms
            Forms.Init();

            // Create the UI
            var page = new MainPage();
            UI.Publish("/main", page.GetOouiElement());
            UI.Present("/main");

            Console.ReadLine();
        }
    }
}
