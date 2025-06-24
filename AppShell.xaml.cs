using Microsoft.Maui.Controls;
using Ứng_dụng_bán_vé_xem_phim.Views; // Đảm bảo namespace này đúng với Views của bạn
using Ứng_dụng_bán_vé_xem_phim.Models;
using Ứng_dụng_bán_vé_xem_phim.Data;
using Microsoft.Maui.Dispatching; // Để dùng MainThread

namespace Ứng_dụng_bán_vé_xem_phim
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("MovieDetailPage", typeof(MovieDetailPage));
            Routing.RegisterRoute("MoviePage", typeof(MoviePage));
            Routing.RegisterRoute("TicketPage", typeof(TicketPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));

            // Gọi các hàm hiển thị tab
            Task.Run(async () =>
            {
                await TicketTabVisibility();
                await LoginTabVisibility();
                await RegisterTabVisibility();
                await UserTabVisibility();
                await SetAdminTabVisibility();
            });

        }

        private async Task LoginTabVisibility()
        {
            var username = Preferences.Get("CurrentUsername", null);

            if (!string.IsNullOrEmpty(username))
            {
                var user = await UserData.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        LoginTab.IsVisible = false;
                    });
                }
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LoginTab.IsVisible = true;
                });
            }
        }

        private async Task RegisterTabVisibility()
        {
            var username = Preferences.Get("CurrentUsername", null);

            if (!string.IsNullOrEmpty(username))
            {
                var user = await UserData.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        RegisterTab.IsVisible = false;
                    });
                }
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    RegisterTab.IsVisible = true;
                });
            }
        }

        private async Task TicketTabVisibility()
        {
            var username = Preferences.Get("CurrentUsername", null);

            if (!string.IsNullOrEmpty(username))
            {
                var user = await UserData.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TicketTab.IsVisible = true;
                    });
                }
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TicketTab.IsVisible = false;
                });
            }
        }

        private async Task UserTabVisibility()
        {
            var username = Preferences.Get("CurrentUsername", null);

            if (!string.IsNullOrEmpty(username))
            {
                var user = await UserData.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UserTab.IsVisible = true;
                    });
                }
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UserTab.IsVisible = false;
                });
            }
        }



        private async Task SetAdminTabVisibility()
        {
            var username = Preferences.Get("CurrentUsername", null);

            if (!string.IsNullOrEmpty(username))
            {
                var user = await UserData.GetUserByUsernameAsync(username);
                if (user != null && user.Role == "Admin")
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        AdminTab.IsVisible = true;
                    });
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        AdminTab.IsVisible = false;
                    });
                }
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AdminTab.IsVisible = false;
                });
            }
        }

    }
}
