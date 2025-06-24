using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Microsoft.Maui.Storage;

namespace Ứng_dụng_bán_vé_xem_phim.Data
{
    public class MovieDatabase
    {
        private static SQLiteAsyncConnection _database;

        public static async Task InitAsync()
        {
            if (_database != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "movies.db");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<Movie>();
        }

        public static Task<Movie> GetMovieByTitleAsync(string title)
        {
            return _database.Table<Movie>().Where(m => m.Title == title).FirstOrDefaultAsync();
        }



        public static Task<List<Movie>> GetAllMoviesAsync()
        {
            return _database.Table<Movie>().ToListAsync();
        }

        public static Task<Movie> GetMovieByIdAsync(int id)
        {
            return _database.Table<Movie>().Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        public static Task<int> AddMovieAsync(Movie movie)
        {
            return _database.InsertAsync(movie);
        }

        public static Task<int> UpdateMovieAsync(Movie movie)
        {
            return _database.UpdateAsync(movie);
        }

        public static Task<int> DeleteMovieAsync(Movie movie)
        {
            return _database.DeleteAsync(movie);
        }
    }
}
