
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Ứng_dụng_bán_vé_xem_phim.Data;

namespace Ứng_dụng_bán_vé_xem_phim.Views
{
    public partial class Administration : ContentPage
    {
        private ObservableCollection<MovieWithTicketInfo> allMovies = new();
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

        public Administration()
        {
            InitializeComponent();
            LoadMoviesWithTickets();
        }

        private async void OnAddMovieClicked(object sender, EventArgs e)
        {
            string title = titleEntry.Text;
            string genre = genreEntry.Text;
            string description = descriptionEntry.Text;
            string posterUrl = posterEntry.Text;
            string ticketType = ticketTypeEntry.Text;
            bool isValidQuantity = int.TryParse(ticketQuantityEntry.Text, out int quantity);
            bool isValidPrice = decimal.TryParse(ticketPriceEntry.Text, out decimal price);

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(ticketType) || !isValidQuantity || !isValidPrice)
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ thông tin hợp lệ", "OK");
                return;
            }

            var movie = new Movie
            {
                Title = title,
                Genre = genre,
                Description = description,
                PosterUrl = posterUrl
            };

            await MovieDatabase.AddMovieAsync(movie);
            var addedMovie = await MovieDatabase.GetMovieByTitleAsync(title);
            if (addedMovie != null)
            {
                var ticket = new Ticket
                {
                    MovieId = addedMovie.Id,
                    TicketType = ticketType,
                    Quantity = quantity,
                    Price = price // Thêm giá tiền
                };
                await TicketData.AddTicketAsync(ticket);
            }

            await DisplayAlert("Thành công", "Đã thêm phim", "OK");
            ClearAddMovieForm();
            LoadMoviesWithTickets();
        }

        private void ClearAddMovieForm()
        {
            titleEntry.Text = "";
            genreEntry.Text = "";
            descriptionEntry.Text = "";
            posterEntry.Text = "";
            ticketTypeEntry.Text = "";
            ticketQuantityEntry.Text = "";
            ticketPriceEntry.Text = ""; 
        }

        private async void LoadMoviesWithTickets()
        {
            var movies = await MovieDatabase.GetAllMoviesAsync();
            allMovies = new ObservableCollection<MovieWithTicketInfo>();

            foreach (var movie in movies)
            {
                var tickets = await TicketData.GetTicketsByMovieIdAsync(movie.Id);
                var ticketInfo = string.Join(", ", tickets.Select(t => $"{t.TicketType}: {t.Quantity} vé - {t.Price:N0}đ"));

                allMovies.Add(new MovieWithTicketInfo
                {
                    MovieId = movie.Id,
                    Title = movie.Title,
                    Genre = movie.Genre,
                    Description = movie.Description,
                    PosterUrl = movie.PosterUrl,
                    TicketInfo = ticketInfo
                });
            }

            movieListView.ItemsSource = allMovies;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = e.NewTextValue?.ToLower() ?? "";
            var filtered = allMovies.Where(m => m.Title.ToLower().Contains(keyword)).ToList();
            movieListView.ItemsSource = new ObservableCollection<MovieWithTicketInfo>(filtered);
        }

        private async void OnEditMovieInlineClicked(object sender, EventArgs e)
        {
            var movieId = (int)((Button)sender).CommandParameter;
            var movie = await MovieDatabase.GetMovieByIdAsync(movieId);
            if (movie == null)
            {
                await DisplayAlert("Lỗi", "Không tìm thấy phim", "OK");
                return;
            }

            movie.Title = await DisplayPromptAsync("Sửa tên", "Nhập tên mới", initialValue: movie.Title);
            movie.Genre = await DisplayPromptAsync("Sửa thể loại", "Nhập thể loại", initialValue: movie.Genre);
            movie.Description = await DisplayPromptAsync("Sửa mô tả", "Nhập mô tả", initialValue: movie.Description);
            movie.PosterUrl = await DisplayPromptAsync("Sửa ảnh", "Nhập PosterUrl", initialValue: movie.PosterUrl);

            await MovieDatabase.UpdateMovieAsync(movie);

            var tickets = await TicketData.GetTicketsByMovieIdAsync(movieId);
            foreach (var ticket in tickets)
            {
                string quantityStr = await DisplayPromptAsync("Cập nhật vé", $"Số lượng vé loại '{ticket.TicketType}':", initialValue: ticket.Quantity.ToString());
                string priceStr = await DisplayPromptAsync("Cập nhật giá vé", $"Giá vé loại '{ticket.TicketType}':", initialValue: ticket.Price.ToString());

                if (int.TryParse(quantityStr, out int quantity) && quantity >= 0 &&
                    decimal.TryParse(priceStr, out decimal price) && price >= 0)
                {
                    ticket.Quantity = quantity;
                    ticket.Price = price;
                    await TicketData.UpdateTicketAsync(ticket);
                }
                else
                {
                    await DisplayAlert("Lỗi", "Số lượng hoặc giá tiền không hợp lệ", "OK");
                }
            }

            await DisplayAlert("Thành công", "Phim đã được cập nhật", "OK");
            LoadMoviesWithTickets();
        }

        private async void OnDeleteMovieInlineClicked(object sender, EventArgs e)
        {
            var movieId = (int)((Button)sender).CommandParameter;

            bool confirm = await DisplayAlert("Xóa phim", $"Bạn có chắc muốn xóa phim ID {movieId} không?", "Xóa", "Hủy");
            if (!confirm) return;

            var movie = await MovieDatabase.GetMovieByIdAsync(movieId);
            if (movie != null)
            {
                await MovieDatabase.DeleteMovieAsync(movie);
                var tickets = await TicketData.GetTicketsByMovieIdAsync(movieId);
                foreach (var ticket in tickets)
                {
                    await TicketData.DeleteTicketAsync(ticket);
                }
                await DisplayAlert("Thành công", "Đã xóa phim và vé", "OK");
                LoadMoviesWithTickets();
            }
            else
            {
                await DisplayAlert("Lỗi", "Không tìm thấy phim", "OK");
            }
        }
    }
}
