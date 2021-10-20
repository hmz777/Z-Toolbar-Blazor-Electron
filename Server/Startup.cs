using BlazorElectronToolbar.Server.Helpers;
using BlazorElectronToolbar.Shared;
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
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        public BrowserWindow MainWindow { get; set; }
        public Rectangle ScreenBounds { get; set; }
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public int ExpandAmount { get; set; }
        public string HotKeyCombination { get; set; }
        public string AppPath { get; set; }
        public string AssetsPath { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddElectron();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app)
        {
            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
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
                await ElectronBootstrap();
            }
        }

        async Task ElectronBootstrap()
        {
            AppPath = await Electron.App.GetAppPathAsync();
            AssetsPath = Path.Combine(WebHostEnvironment.ContentRootPath, "Assets");

            ScreenBounds = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

            WindowWidth = Configuration.GetValue<int>("Window:Width");
            WindowHeight = Configuration.GetValue<int>("Window:Height");
            ExpandAmount = Configuration.GetValue<int>("Window:ExpandAmount");
            HotKeyCombination = Configuration.GetValue<string>("Window:HotKeyCombination");

            MainWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = WindowWidth,
                Height = WindowHeight,
                X = ScreenBounds.Width - WindowWidth,
                Y = (ScreenBounds.Height - WindowHeight) / 2,
                Show = false,
                Frame = false,
                TitleBarStyle = TitleBarStyle.hidden,
                AutoHideMenuBar = true,
                Resizable = false,
                Maximizable = false,
                Fullscreenable = false,
                Movable = false,
                Transparent = true,
                SkipTaskbar = true,
                Icon = Path.Combine(AssetsPath, "app-icon-l.ico")
            });

            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.System);

            if (!WebHostEnvironment.IsDevelopment())
            {
                await ConfigureTrayIcon(AssetsPath);
                ConfigureStartup();
            }

            MainWindow.OnBlur += OnLostFocus;

            Electron.App.Ready += OnReady;
            Electron.App.WillQuit += WillQuit;

            await MainWindow.WebContents.Session.ClearCacheAsync();

            MainWindow.OnReadyToShow += () =>
            {
                RestoreToolbarDefaultPosition(true);
                MainWindow.SetTitle(Configuration.GetValue<string>("AppInfo:AppTitle"));
                MainWindow.SetAlwaysOnTop(true);
                MainWindow.SetVisibleOnAllWorkspaces(true);

                if (WebHostEnvironment.IsDevelopment())
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

        async void OnLostFocus()
        {
            var hasFocus = await MainWindow.IsFocusedAsync();

            if (!hasFocus)
            {
                var pos = await MainWindow.GetPositionAsync();

                Electron.IpcMain.Send(MainWindow, "CommandChannel", new ServerClientModel { Key = "AppShow", Value = false });

                await Task.Delay(400);

                MainWindow.SetPosition(ScreenBounds.Width, pos[1]);
            }
        }

        void OnHotKeyTrigger()
        {
            RestoreToolbarDefaultPosition(false);

            Electron.IpcMain.Send(MainWindow, "CommandChannel", new ServerClientModel { Key = "AppShow", Value = true });

            Electron.App.Focus();
        }

        Task WillQuit(QuitEventArgs quitEventArgs)
        {
            Electron.GlobalShortcut.UnregisterAll();

            //There is a problem with Electron.Tray.IsDestroyedAsync()

            Electron.Tray.Destroy();

            return Task.CompletedTask;
        }

        void RestoreToolbarDefaultPosition(bool firstStart)
        {
            if (firstStart || !StateHelpers.ToolbarExpanded)
            {
                MainWindow.SetBounds(new Rectangle
                {
                    Width = WindowWidth,
                    Height = WindowHeight,
                    X = ScreenBounds.Width - WindowWidth,
                    Y = (ScreenBounds.Height - WindowHeight) / 2,
                }, true);
            }
            else
            {
                MainWindow.SetBounds(new Rectangle
                {
                    Width = WindowWidth + ExpandAmount,
                    Height = WindowHeight,
                    X = ScreenBounds.Width - (WindowWidth + ExpandAmount),
                    Y = (ScreenBounds.Height - WindowHeight) / 2,
                }, true);
            }
        }

        async Task ConfigureTrayIcon(string RootPath)
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
                }};

            //Since chrome follows the system chosen theme, we could guess the system theme from the chrome theme.
            if (await Electron.NativeTheme.ShouldUseDarkColorsAsync())
            {
                Electron.Tray.Show(Path.Combine(RootPath, "app-icon-l.png"), menu);
            }
            else
            {
                Electron.Tray.Show(Path.Combine(RootPath, "app-icon-d.png"), menu);
            }
        }

        void ConfigureStartup()
        {
            var isStartupAllowed = Configuration.GetValue<bool>("Settings:Startup");

            Electron.App.SetLoginItemSettings(new LoginSettings() { OpenAtLogin = isStartupAllowed });
        }
    }
}