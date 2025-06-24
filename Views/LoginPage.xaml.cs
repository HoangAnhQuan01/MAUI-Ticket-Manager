using Microsoft.Maui.Controls;
using System;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Ứng_dụng_bán_vé_xem_phim.Data;
using Microsoft.Maui.Storage;


namespace Ứng_dụng_bán_vé_xem_phim.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
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

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập tài khoản và mật khẩu", "OK");
                return;
            }

            await UserData.InitAsync();
            var user = await UserData.GetUserByUsernameAsync(username);

            if (user != null && user.Password == password)
            {
                Preferences.Set("CurrentUsername", user.Username);
                Preferences.Set("UserRole", user.Role);

                await DisplayAlert("Chào mừng", $"Xin chào {user.Username}!", "OK");
                Application.Current.MainPage = new AppShell(); // chuyển vào app chính
            }
            else
            {
                await DisplayAlert("Lỗi", "Sai tên đăng nhập hoặc mật khẩu", "OK");
            }
        }
        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            string email = await DisplayPromptAsync("Quên mật khẩu", "Nhập địa chỉ email đã đăng ký:", "Xác nhận", "Hủy");

            if (string.IsNullOrWhiteSpace(email))
                return;

            await UserData.InitAsync();
            var user = await UserData.GetUserByEmailAsync(email); // ⚠️ Bạn cần hàm này trong UserData.cs

            if (user != null)
            {
                await DisplayAlert("Thông báo", $"Tài khoản của bạn là: {user.Username}\nMật khẩu: {user.Password}\nHãy đăng nhập lại bằng tên tài khoản và mật khẩu.", "OK");
            }
            else
            {
                await DisplayAlert("Không tìm thấy", "Không tìm thấy tài khoản nào với email đã nhập", "OK");
            }
        }
    }
}
