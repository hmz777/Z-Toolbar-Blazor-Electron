using BlazorElectronToolbar.Server.Helpers;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;

namespace BlazorElectronToolbar.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public BrowserWindow MainWindow { get; set; }
        public int HideOffset { get; set; }
        public Rectangle ScreenSize { get; set; }
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddElectron();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }
        }

        public async void ElectronBootstrap()
        {
            ScreenSize = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

            WindowWidth = Configuration.GetValue<int>("Window:Width");
            WindowHeight = Configuration.GetValue<int>("Window:Height");
            HideOffset = Configuration.GetValue<int>("Window:HideOffset");

            MainWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                MinWidth = WindowWidth,
                MaxWidth = ScreenSize.Width / 2,
                MinHeight = WindowHeight,
                MaxHeight = WindowHeight,
                //Width = WindowWidth,
                //Height = WindowHeight,
                //X = ScreenSize.Width - WindowWidth,
                //Y = (ScreenSize.Height - WindowHeight) / 2,
                Show = false,
                Frame = false,
                TitleBarStyle = TitleBarStyle.hidden,
                AutoHideMenuBar = true,
                DarkTheme = true,
                Resizable = false,
                Maximizable = false,
                Fullscreenable = false,
                Movable = false,
                Transparent = true
            });

            //MainWindow.OnFocus += OnFocus;
            //MainWindow.OnBlur += OnLostFocus;


            await MainWindow.WebContents.Session.ClearCacheAsync();

            MainWindow.SetBounds(new Rectangle
            {
                Width = WindowWidth,
                Height = WindowHeight,
                X = ScreenSize.Width - WindowWidth,
                Y = (ScreenSize.Height - WindowHeight) / 2,
            });

            MainWindow.OnReadyToShow += () => MainWindow.Show();
            MainWindow.SetTitle(Configuration.GetValue<string>("AppInfo:AppTitle"));

            MainWindow.WebContents.OpenDevTools();
        }

        public async void OnFocus()
        {
            var pos = await MainWindow.GetPositionAsync();
            MainWindow.SetPosition((ScreenSize.Width - WindowWidth), pos[1]);
        }

        public async void OnLostFocus()
        {
            var pos = await MainWindow.GetPositionAsync();
            MainWindow.SetPosition(ScreenSize.Width, pos[1]);
        }
    }
}