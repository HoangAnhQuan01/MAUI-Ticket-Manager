using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ứng_dụng_bán_vé_xem_phim.Models;
using Microsoft.Maui.Storage;

namespace Ứng_dụng_bán_vé_xem_phim.Data
{
    public class TicketData
    {
        private static SQLiteAsyncConnection _database;

        public static async Task InitAsync()
        {
            if (_database != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<Ticket>();
        }



        public static Task<int> AddTicketAsync(Ticket ticket)
        {
            return _database.InsertAsync(ticket);
        }

        public static Task<Ticket> GetTicketByIdAsync(int id)
        {
            return _database.Table<Ticket>().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        public static Task<List<Ticket>> GetTicketsByMovieIdAsync(int movieId)
        {
            return _database.Table<Ticket>().Where(t => t.MovieId == movieId).ToListAsync();
        }

        public static Task<int> UpdateTicketAsync(Ticket ticket)
        {
            return _database.UpdateAsync(ticket);
        }

        public static Task<int> DeleteTicketAsync(Ticket ticket)
        {
            return _database.DeleteAsync(ticket);
        }

        public static Task<List<Ticket>> GetAllTicketsAsync()
        {
            return _database.Table<Ticket>().ToListAsync();
        }
    }
}
