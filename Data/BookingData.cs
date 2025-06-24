using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Microsoft.Maui.Storage;

namespace Ứng_dụng_bán_vé_xem_phim.Data
{
    public class BookingData
    {
        private static SQLiteAsyncConnection _database;

        public static async Task InitAsync()
        {
            if (_database != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<Booking>();
        }

        public static Task<int> AddBookingAsync(Booking booking)
        {
            return _database.InsertAsync(booking);
        }

        public static Task<Booking> GetBookingByIdAsync(int id)
        {
            return _database.Table<Booking>().Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public static Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return _database.Table<Booking>().Where(b => b.UserId == userId).ToListAsync();
        }

        public static Task<List<Booking>> GetBookingsByMovieIdAsync(int movieId)
        {
            return _database.Table<Booking>().Where(b => b.MovieId == movieId).ToListAsync();
        }

        public static Task<int> UpdateBookingAsync(Booking booking)
        {
            return _database.UpdateAsync(booking);
        }

        public static Task<int> DeleteBookingAsync(Booking booking)
        {
            return _database.DeleteAsync(booking);
        }

        public static Task<List<Booking>> GetAllBookingsAsync()
        {
            return _database.Table<Booking>().ToListAsync();
        }
    }
}
