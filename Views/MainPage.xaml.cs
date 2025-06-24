using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Ứng_dụng_bán_vé_xem_phim.Models; // Đảm bảo namespace này đúng với Movie model của bạn
using Ứng_dụng_bán_vé_xem_phim.ViewModels; // Đảm bảo namespace này đúng với HomeViewModel của bạn

namespace Ứng_dụng_bán_vé_xem_phim.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        private async void OnMovieSelected(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra xem có phim nào được chọn không
            if (e.CurrentSelection.FirstOrDefault() is Movie selectedMovie)
            {
                // Bỏ chọn mục để cho phép chọn lại sau khi quay lại trang
                ((CollectionView)sender).SelectedItem = null;

                // Điều hướng đến MovieDetailPage, truyền ID của phim đã chọn
                await Shell.Current.GoToAsync($"MovieDetailPage?movieId={selectedMovie.Id}");
            }
        }

        private async void OnImagePointerEntered(object sender, PointerEventArgs e)
        {
            if (sender is Image img)
                await img.ScaleTo(1.2, 150);
        }

        private async void OnImagePointerExited(object sender, PointerEventArgs e)
        {
            if (sender is Image img)
                await img.ScaleTo(1.0, 150);
        }

        private CancellationTokenSource _blinkToken;
        private int _currentIndex = 0;
        private CancellationTokenSource _carouselToken;
        private ObservableCollection<Movie> _carouselItems;

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is HomeViewModel viewModel)
            {
                await viewModel.InitAsync();

                // Gán carousel
                if (viewModel.FeaturedMovies != null)
                {
                    _carouselItems = new ObservableCollection<Movie>(viewModel.FeaturedMovies);
                    MyCarousel.ItemsSource = _carouselItems;
                    _currentIndex = 0;
                    StartAutoScroll();
                }

                // Gán danh sách phim mới
                if (NewMoviesCollectionView != null)
                {
                    NewMoviesCollectionView.ItemsSource = viewModel.NewMoviesList;
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _carouselToken?.Cancel();
        }

        private async void StartAutoScroll()
        {
            _carouselToken = new CancellationTokenSource();
            var token = _carouselToken.Token;

            if (_carouselItems == null || _carouselItems.Count < 2)
                return;

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(2000);
                if (token.IsCancellationRequested)
                    break;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _currentIndex++;

                    if (_currentIndex >= _carouselItems.Count)
                    {
                        _currentIndex = 0;

                        // 🔁 Reset carousel bằng cách tạo list mới & gán lại
                        var refreshedItems = new ObservableCollection<Movie>(_carouselItems);
                        _carouselItems = refreshedItems;
                        MyCarousel.ItemsSource = null;
                        MyCarousel.ItemsSource = _carouselItems;
                        MyCarousel.Position = 0;
                    }
                    else
                    {
                        MyCarousel.Position = _currentIndex;
                    }
                });
            }
        }





    }
}
