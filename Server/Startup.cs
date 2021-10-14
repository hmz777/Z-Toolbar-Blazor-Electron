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
using System.Threading.Tasks;

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
        public string HotKeyCombination { get; set; }

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
                ElectronBootstrap(env);
            }
        }

        async void ElectronBootstrap(IWebHostEnvironment env)
        {
            ScreenSize = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

            WindowWidth = Configuration.GetValue<int>("Window:Width");
            WindowHeight = Configuration.GetValue<int>("Window:Height");
            HideOffset = Configuration.GetValue<int>("Window:HideOffset");
            HotKeyCombination = Configuration.GetValue<string>("Window:HotKeyCombination");

            MainWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = WindowWidth,
                Height = WindowHeight,
                Show = false,
                Frame = false,
                TitleBarStyle = TitleBarStyle.hidden,
                AutoHideMenuBar = true,
                Resizable = false,
                Maximizable = false,
                Fullscreenable = false,
                Movable = false,
                Transparent = true,
                SkipTaskbar = true
            });

            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.System);

            ConfigureTrayIcon(env);
            ConfigureStartup();

            MainWindow.OnBlur += OnLostFocus;

            Electron.App.Ready += OnReady;
            Electron.App.WillQuit += WillQuit;

            await MainWindow.WebContents.Session.ClearCacheAsync();

            MainWindow.OnReadyToShow += async () =>
            {
                await RestoreToolbarDefaultPosition(true);
                MainWindow.SetTitle(Configuration.GetValue<string>("AppInfo:AppTitle"));
                MainWindow.SetAlwaysOnTop(true);
                MainWindow.SetVisibleOnAllWorkspaces(true);

                if (env.IsDevelopment())
                {
                    MainWindow.WebContents.OpenDevTools();
                }

                MainWindow.Show();
                MainWindow.Blur();
            };
        }

        void OnReady()
        {
            Electron.GlobalShortcut.Register(HotKeyCombination, OnHotKeyTrigger);
        }

        Task WillQuit(QuitEventArgs quitEventArgs)
        {
            Electron.GlobalShortcut.UnregisterAll();
            Electron.Tray.Destroy();
            return Task.CompletedTask;
        }

        async Task RestoreToolbarDefaultPosition(bool firstStart)
        {
            if (firstStart)
            {
                MainWindow.SetBounds(new Rectangle
                {
                    Width = WindowWidth,
                    Height = WindowHeight,
                    X = ScreenSize.Width - WindowWidth,
                    Y = (ScreenSize.Height - WindowHeight) / 2,
                });
            }
            else
            {
                var size = await MainWindow.GetSizeAsync();

                MainWindow.SetBounds(new Rectangle
                {
                    Width = size[0],
                    Height = size[1],
                    X = ScreenSize.Width - size[0],
                    Y = (ScreenSize.Height - size[1]) / 2,
                });
            }
        }

        async void OnLostFocus()
        {
            //Try to do some kind of animation trick to hide the toolbar

            var pos = await MainWindow.GetPositionAsync();

            MainWindow.SetPosition(ScreenSize.Width, pos[1]);
        }

        async void OnHotKeyTrigger()
        {
            //Try to do some kind of animation trick to show the toolbar

            await RestoreToolbarDefaultPosition(false);

            Electron.App.Focus();
        }

        async void ConfigureTrayIcon(IWebHostEnvironment env)
        {
            Electron.Tray.SetTitle("Z-Toolbar");
            Electron.Tray.SetToolTip("Z-Toolbar");

            var menu = new MenuItem[] {
                new MenuItem
                {
                    Enabled = true,
                    Label = "Exit",
                    Role = MenuRole.quit,
                    Visible = true
                }
            };

            //Since chrome follows the system chosen theme, we could guess the system theme from the chrome theme.
            if (await Electron.NativeTheme.ShouldUseDarkColorsAsync())
            {
                Electron.Tray.Show(Path.Combine(env.ContentRootPath, "Assets/app-icon-l.png"), menu);
            }
            else
            {
                Electron.Tray.Show(Path.Combine(env.ContentRootPath, "Assets/app-icon-d.png"), menu);
            }
        }

        void ConfigureStartup()
        {
            var isStartupAllowed = Configuration.GetValue<bool>("Settings:Startup");

            Electron.App.SetLoginItemSettings(new LoginSettings() { OpenAtLogin = isStartupAllowed });
        }
    }
}