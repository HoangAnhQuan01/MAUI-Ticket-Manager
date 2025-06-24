
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Ứng_dụng_bán_vé_xem_phim.Data;

//Phần model cho MainPage

namespace Ứng_dụng_bán_vé_xem_phim.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Movie> MovieList { get; set; } = new();
        public ObservableCollection<Movie> FeaturedMovies { get; set; } = new(); // 3 phim cho carousel
        public ObservableCollection<Movie> NewMoviesList { get; set; } = new();  // 5 phim có Id từ 1 đến 5

        public HomeViewModel()
        {
            Task.Run(async () => await LoadMoviesAsync());
        }

        public async Task InitAsync()
        {
            await LoadMoviesAsync();
        }

        private async Task LoadMoviesAsync()
        {
            await MovieDatabase.InitAsync();
            var moviesFromDb = await MovieDatabase.GetAllMoviesAsync();

            MovieList = new ObservableCollection<Movie>(moviesFromDb);
            FeaturedMovies = new ObservableCollection<Movie>(MovieList.Take(3));  //Xóa hoặc để dòng này thành comment khi gọi hàm FeaturedMoviesTake()

            NewMovies(); // Gọi hàm lấy phim vào trang chủ
          //FeaturedMoviesTake() // Gọi hàm lấy 3 phim lên băng chuyền
        }

        private void NewMovies()
        {
            var newMovies = MovieList
                .Where(m => m.Id >= 3 && m.Id <= 7)
                //.OrderByDescending(m => m.Id) xếp từ lớn đến nhỏ
                .OrderBy(m => m.Id) //Xếp từ nhỏ đến lớn
                //.Take(5) // Điều chỉnh số lượng lấy lấy những phim đều tiên theo cách sắp xếp
                .ToList();

            NewMoviesList = new ObservableCollection<Movie>(newMovies);
            OnPropertyChanged(nameof(NewMoviesList));
        }

        //hàm lấy 3 phim lên băng chuyền
        //private void FeaturedMoviesTake()
        //{
        //    var featured = MovieList
        //        .OrderByDescending(m => m.Id) // xếp từ lớn đến nhỏ
        //      //.OrderBy(m => m.Id) //Xếp từ nhỏ đến lớn
        //        .Take(3) // Lấy 3 phim đầu tiên
        //        .ToList();
        //    FeaturedMovies = new ObservableCollection<Movie>(featured);
        //    OnPropertyChanged(nameof(FeaturedMovies));

        //}

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
