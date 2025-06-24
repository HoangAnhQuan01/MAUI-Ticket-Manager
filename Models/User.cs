using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Ứng_dụng_bán_vé_xem_phim.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Username { get; set; }

        [Unique]
        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } = "User";
    }
}
