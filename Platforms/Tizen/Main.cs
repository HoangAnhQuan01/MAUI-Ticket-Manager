using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Ứng_dụng_bán_vé_xem_phim
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
