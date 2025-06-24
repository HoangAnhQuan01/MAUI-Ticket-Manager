using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;
using Ứng_dụng_bán_vé_xem_phim.Data;
using Ứng_dụng_bán_vé_xem_phim.Models;

namespace Ứng_dụng_bán_vé_xem_phim.Views
{
    public partial class UserPage : ContentPage
    {
        private User currentUser;
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

        public UserPage()
        {
            InitializeComponent();
            Task.Run(async () => await LoadUserInfo());
        }

        private async Task LoadUserInfo()
        {
            string username = Preferences.Get("CurrentUsername", null);
            if (string.IsNullOrEmpty(username))
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Lỗi", "Bạn cần đăng nhập để truy cập trang này", "OK");
                    Application.Current.MainPage = new LoginPage();
                });
                return;
            }

            await UserData.InitAsync();
            currentUser = await UserData.GetUserByUsernameAsync(username);

            if (currentUser != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    usernameLabel.Text = currentUser.Username;
                    emailLabel.Text = currentUser.Email;
                    roleLabel.Text = currentUser.Role;
                });
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("CurrentUsername");
            Preferences.Remove("UserRole");

            await DisplayAlert("Đăng xuất", "Bạn đã đăng xuất", "OK");

            // Reset toàn bộ Shell
            Application.Current.MainPage = new AppShell();

        }

        private async void OnDeleteAccountClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Xác nhận", "Bạn có chắc muốn xóa tài khoản?", "Xóa", "Hủy");
            if (confirm && currentUser != null)
            {
                await UserData.DeleteUserAsync(currentUser);
                Preferences.Clear();
                await DisplayAlert("Đã xóa", "Tài khoản đã được xóa", "OK");

                Application.Current.MainPage = new AppShell();
            }
        }
        private async void OnEditAccountClicked(object sender, EventArgs e)
        {
            if (currentUser == null)
            {
                await DisplayAlert("Lỗi", "Không tìm thấy thông tin người dùng", "OK");
                return;
            }

            string oldPassword = await DisplayPromptAsync("Xác thực", "Nhập mật khẩu hiện tại:", "Xác nhận", "Hủy", "Mật khẩu cũ");
            if (string.IsNullOrEmpty(oldPassword) || oldPassword != currentUser.Password)
            {
                await DisplayAlert("Sai mật khẩu", "Mật khẩu cũ không đúng", "OK");
                return;
            }

            string newUsername = await DisplayPromptAsync("Sửa tên", "Nhập tên người dùng mới:", initialValue: currentUser.Username);
            string newEmail = await DisplayPromptAsync("Sửa email", "Nhập email mới:", initialValue: currentUser.Email);
            string newPassword = await DisplayPromptAsync("Đổi mật khẩu", "Nhập mật khẩu mới:", "OK", "Hủy", "Mật khẩu mới");

            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newEmail) || string.IsNullOrEmpty(newPassword))
            {
                await DisplayAlert("Lỗi", "Không được để trống thông tin", "OK");
                return;
            }

            currentUser.Username = newUsername;
            currentUser.Email = newEmail;
            currentUser.Password = newPassword;

            await UserData.UpdateUserAsync(currentUser);
            Preferences.Set("CurrentUsername", currentUser.Username); // cập nhật tên đăng nhập mới nếu có

            await DisplayAlert("Thành công", "Thông tin tài khoản đã được cập nhật", "OK");
            await LoadUserInfo();
        }


    }
}
