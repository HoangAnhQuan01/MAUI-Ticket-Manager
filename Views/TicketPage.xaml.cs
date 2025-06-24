
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Ứng_dụng_bán_vé_xem_phim.Data;

namespace Ứng_dụng_bán_vé_xem_phim.Views
{
    public partial class TicketPage : ContentPage
    {
        public TicketPage()
        {
            InitializeComponent();
            LoadBookings();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadBookings(); // ✅ Gọi lại khi trang hiển thị
        }

        private async void LoadBookings()
        {
            string username = Preferences.Get("CurrentUsername", null);
            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Lỗi", "Bạn cần đăng nhập để xem lịch sử đặt vé", "OK");
                return;
            }

            var user = await UserData.GetUserByUsernameAsync(username);
            if (user == null)
            {
                await DisplayAlert("Lỗi", "Không tìm thấy người dùng", "OK");
                return;
            }

            var bookings = await BookingData.GetBookingsByUserIdAsync(user.Id);
            var bookingViews = new List<BookingView>();

            foreach (var booking in bookings)
            {
                var movie = await MovieDatabase.GetMovieByIdAsync(booking.MovieId);
                var ticket = await TicketData.GetTicketByIdAsync(booking.TicketId); 

                decimal total = ticket != null ? ticket.Price * booking.QuantityPurchased : 0; 

                bookingViews.Add(new BookingView
                {
                    MovieTitle = movie?.Title ?? "Không xác định",
                    Quantity = booking.QuantityPurchased,
                    PaymentMethod = booking.PaymentMethod,
                    PurchaseDate = booking.PurchaseDate.ToString("dd/MM/yyyy HH:mm"),
                    TotalPrice = $"{total:N0}đ"
                });
            }

            BookingListView.ItemsSource = bookingViews;
        }
    }

    public class BookingView
    {
        public string MovieTitle { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public string PurchaseDate { get; set; }

        public string TotalPrice { get; set; } 
    }
}
