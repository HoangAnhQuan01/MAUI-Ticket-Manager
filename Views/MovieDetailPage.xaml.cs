
using System;
using Microsoft.Maui.Controls;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Ứng_dụng_bán_vé_xem_phim.Data;
using System.Linq;

namespace Ứng_dụng_bán_vé_xem_phim.Views
{
    [QueryProperty(nameof(MovieId), "movieId")]
    public partial class MovieDetailPage : ContentPage
    {
        private int _movieId;
        private Movie _movie;
        private int _availableTickets = 0;
        private Ticket _ticket; // Giả sử mỗi phim có 1 loại vé duy nhất
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
        public int MovieId
        {
            get => _movieId;
            set
            {
                _movieId = value;
                LoadMovieDetail(_movieId);
            }
        }

        public MovieDetailPage()
        {
            InitializeComponent();
        }

        private async void LoadMovieDetail(int id)
        {
            _movie = await MovieDatabase.GetMovieByIdAsync(id);
            if (_movie != null)
            {
                MovieTitleLabel.Text = _movie.Title;
                PosterImage.Source = _movie.PosterUrl;
                GenreLabel.Text = $"Thể loại: {_movie.Genre}";
                DescriptionLabel.Text = _movie.Description;

                var tickets = await TicketData.GetTicketsByMovieIdAsync(id);
                _ticket = tickets?.FirstOrDefault();

                if (_ticket != null)
                {
                    _availableTickets = _ticket.Quantity;
                    AvailableTicketsLabel.Text = $"Còn lại: {_availableTickets} vé";
                    TicketPriceLabel.Text = $"Giá vé: {_ticket.Price:N0}đ";
                    TicketStepper.Maximum = _availableTickets > 0 ? _availableTickets : 1;
                }
                else
                {
                    AvailableTicketsLabel.Text = "Không có vé khả dụng";
                    TicketPriceLabel.Text = "Chưa có giá vé";
                }
            }
            string username = Preferences.Get("CurrentUsername", null);
            bool isLoggedIn = !string.IsNullOrEmpty(username);

            PurchaseButton.IsVisible = isLoggedIn;
            LoginWarningLabel.IsVisible = !isLoggedIn;
        }

        private async void OnPurchaseClicked(object sender, EventArgs e)
        {
            string username = Preferences.Get("CurrentUsername", null);
            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Lỗi", "Bạn cần đăng nhập", "OK");
                return;
            }

            var user = await UserData.GetUserByUsernameAsync(username);
            if (user == null)
            {
                await DisplayAlert("Lỗi", "Tài khoản không tồn tại", "OK");
                return;
            }

            int quantity = (int)TicketStepper.Value;
            string payment = PaymentPicker.SelectedItem?.ToString();

            if (_ticket == null)
            {
                await DisplayAlert("Lỗi", "Không tìm thấy thông tin vé", "OK");
                return;
            }

            if (string.IsNullOrEmpty(payment))
            {
                await DisplayAlert("Lỗi", "Vui lòng chọn phương thức thanh toán", "OK");
                return;
            }

            if (quantity > _ticket.Quantity)
            {
                await DisplayAlert("Lỗi", "Số lượng vé vượt quá số còn lại", "OK");
                return;
            }

            var booking = new Booking
            {
                MovieId = _movieId,
                UserId = user.Id,
                TicketId = _ticket.Id,
                QuantityPurchased = quantity,
                PaymentMethod = payment,
                PurchaseDate = DateTime.Now
            };

            await BookingData.AddBookingAsync(booking);

            // ✅ Trừ số lượng vé còn lại
            _ticket.Quantity -= quantity;
            await TicketData.UpdateTicketAsync(_ticket); // Cập nhật vào database

            await DisplayAlert("Thành công", "Bạn đã đặt vé", "OK");

            // 👇 Quay lại stack trước (MainPage hoặc MoviePage)
            await Shell.Current.GoToAsync("..");

            // ✅ Sau đó chuyển sang trang TicketPage (absolute route)
            await Shell.Current.GoToAsync("//TicketPage");
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
