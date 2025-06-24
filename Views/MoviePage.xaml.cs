using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Ứng_dụng_bán_vé_xem_phim.Models; // Đảm bảo namespace này đúng với Movie model của bạn
using Ứng_dụng_bán_vé_xem_phim.ViewModels;


namespace Ứng_dụng_bán_vé_xem_phim.Views;

public partial class MoviePage : ContentPage
{
    private MovieViewModel viewModel;
    public MoviePage()
	{
		InitializeComponent();
        viewModel = new MovieViewModel();
        BindingContext = viewModel;
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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.InitAsync();
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

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(keyword))
        {
            viewModel.FilterMovies(); // Hiển thị lại tất cả
        }
        else
        {
            viewModel.FilteredMovies = new ObservableCollection<Movie>(
                viewModel.MovieList.Where(m =>
                    (!string.IsNullOrEmpty(m.Title) && m.Title.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(m.Genre) && m.Genre.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(m.Description) && m.Description.ToLower().Contains(keyword))
                ));
        }
    }
}