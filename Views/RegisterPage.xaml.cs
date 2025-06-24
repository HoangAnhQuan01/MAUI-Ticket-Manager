using Microsoft.Maui.Controls;
using System;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Ứng_dụng_bán_vé_xem_phim.ViewModels;
using Ứng_dụng_bán_vé_xem_phim.Data;
using Microsoft.Maui.Storage;


namespace Ứng_dụng_bán_vé_xem_phim.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel();
        }

        private Color _defaultColor;
        private bool _isPressed = false;
        private Color LightenColor(Color color, float factor)
        {
            return new Color(
                Math.Min(color.Red + factor, 1),
                Math.Min(color.Green + factor, 1),
                Math.Min(color.Blue + factor, 1),
                color.Alpha);
        }
        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            if (sender is Button btn)
            {
                _defaultColor = btn.BackgroundColor;
                if (!_isPressed)
                    btn.BackgroundColor = LightenColor(_defaultColor, 0.2f); // sáng hơn 20%
            }
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            if (sender is Button btn)
            {
                _isPressed = false;
                btn.BackgroundColor = _defaultColor;
            }
        }

        private async void OnCreateAdminClicked(object sender, EventArgs e)
        {
            await UserData.InitAsync();
            var existing = await UserData.GetUserByUsernameAsync("admin");

            if (existing == null)
            {
                var admin = new User
                {
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@example.com",
                    Role = "Admin"
                };

                await UserData.AddUserAsync(admin);
                await DisplayAlert("Thành công", "Đã tạo tài khoản Admin", "OK");
            }
            else
            {
                await DisplayAlert("Thông báo", "Tài khoản Admin đã tồn tại", "OK");
            }
        }

    }
}
