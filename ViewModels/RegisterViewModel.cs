using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Ứng_dụng_bán_vé_xem_phim.Data;
using Ứng_dụng_bán_vé_xem_phim.Models;

//Phần model cho RegisterPage

namespace Ứng_dụng_bán_vé_xem_phim.ViewModels
{
    // Triển khai INotifyPropertyChanged để thông báo thay đổi cho View
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _email;
        private string _password;
        private string _confirmPassword;

        public event PropertyChangedEventHandler PropertyChanged;

        // Helper method để gọi PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Vui lòng không để trống tên đăng nhập")]
        [MaxLength(20, ErrorMessage = "Tên đăng nhập không được vượt quá 20 ký tự")]
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(); // Thông báo khi Username thay đổi
                }
            }
        }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng không để trống email")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập địa chỉ email hợp lệ")]
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(); // Thông báo khi Email thay đổi
                }
            }
        }

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng không để trống mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(); // Thông báo khi Password thay đổi
                    // Cũng cần thông báo khi ConfirmPassword có thể bị ảnh hưởng
                    OnPropertyChanged(nameof(ConfirmPassword));
                }
            }
        }

        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    OnPropertyChanged(); // Thông báo khi ConfirmPassword thay đổi
                }
            }
        }

        // Command để xử lý logic đăng ký
        public ICommand RegisterCommand { get; private set; }

        public RegisterViewModel()
        {
            // Khởi tạo Command. Bạn sẽ cần triển khai logic Register trong phương thức RegisterExecute.
            RegisterCommand = new Command(RegisterExecute);
        }

        private async void RegisterExecute()
        {
            var validationContext = new ValidationContext(this, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

            if (isValid)
            {
                await UserData.InitAsync();

                var existingUser = await UserData.GetUserByUsernameAsync(this.Username);
                if (existingUser != null)
                {
                    await Shell.Current.DisplayAlert("Lỗi", "Tên đăng nhập đã tồn tại", "OK");
                    return;
                }

                var existingEmail = await UserData.GetUserByEmailAsync(this.Email);
                if (existingEmail != null)
                {
                    await Shell.Current.DisplayAlert("Lỗi", "Email này đã được sử dụng", "OK");
                    return;
                }

                var newUser = new User
                {
                    Username = this.Username,
                    Email = this.Email,
                    Password = this.Password,
                    Role = "User"
                };

                await UserData.AddUserAsync(newUser);

                await Shell.Current.DisplayAlert("Thành công", "Đăng ký thành công!", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                string errors = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
                await Shell.Current.DisplayAlert("Lỗi đăng ký", errors, "OK");
            }
        }

    }
}