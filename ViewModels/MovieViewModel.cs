using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Ứng_dụng_bán_vé_xem_phim.Data;

//Phần model cho MoviePage

namespace Ứng_dụng_bán_vé_xem_phim.ViewModels
{
    public class MovieViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Movie> MovieList { get; set; } = new();
        public ObservableCollection<Movie> FeaturedMovies { get; set; } = new();
        public ObservableCollection<string> Genres { get; set; } = new();

        private string _selectedGenre;
        public string SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (_selectedGenre != value)
                {
                    _selectedGenre = value;
                    OnPropertyChanged(nameof(SelectedGenre));
                    FilterMovies();
                }
            }
        }

        private ObservableCollection<Movie> _filteredMovies;
        public ObservableCollection<Movie> FilteredMovies
        {
            get => _filteredMovies;
            set
            {
                if (_filteredMovies != value)
                {
                    _filteredMovies = value;
                    OnPropertyChanged(nameof(FilteredMovies));
                }
            }
        }

        public async Task InitAsync()
        {
            var movies = await MovieDatabase.GetAllMoviesAsync();
            MovieList = new ObservableCollection<Movie>(movies);

            // Chọn 3 phim đầu làm phim nổi bật
            FeaturedMovies = new ObservableCollection<Movie>(MovieList.Take(3));

            // Lấy danh sách thể loại duy nhất và sắp xếp
            Genres = new ObservableCollection<string>(MovieList.Select(m => m.Genre).Distinct().OrderBy(g => g));
            Genres.Insert(0, "Tất cả");
            OnPropertyChanged(nameof(Genres));

            // Khởi tạo mặc định
            SelectedGenre = "Tất cả";
            FilterMovies();
        }

        public void FilterMovies()
        {
            if (SelectedGenre == "Tất cả")
            {
                FilteredMovies = new ObservableCollection<Movie>(MovieList);
            }
            else
            {
                FilteredMovies = new ObservableCollection<Movie>(
                    MovieList.Where(m => m.Genre == SelectedGenre));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
