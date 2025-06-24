using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Ứng_dụng_bán_vé_xem_phim.Models;
using System.IO;

namespace Ứng_dụng_bán_vé_xem_phim.Data
{
    public class UserData
    {
        private static SQLiteAsyncConnection _database;

        public static async Task InitAsync()
        {
            if (_database != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "users.db");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<User>();
        }

        public static async Task UpdateUserAsync(User user)
        {
            await InitAsync();
            await _database.UpdateAsync(user);
        }



        public static async Task<int> AddUserAsync(User user)
        {
            await InitAsync();
            return await _database.InsertAsync(user);
        }

        public static async Task<User> GetUserByUsernameAsync(string username)
        {
            await InitAsync();
            return await _database.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
        }

        public static async Task<User> GetUserByEmailAsync(string email)
        {
            await InitAsync();
            return await _database.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public static async Task<List<User>> GetAllUsersAsync()
        {
            await InitAsync();
            return await _database.Table<User>().ToListAsync();
        }

        public static async Task<int> DeleteUserAsync(User user)
        {
            await InitAsync();
            return await _database.DeleteAsync(user);
        }
    }
}
