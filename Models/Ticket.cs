using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Ứng_dụng_bán_vé_xem_phim.Models
{
    public class Ticket
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int MovieId { get; set; }  // Liên kết tới phim

        public decimal Price { get; set; }

        public string TicketType { get; set; }  // Có thể là "VIP", "2D", "3D"...

        public int Quantity { get; set; }  // Số vé còn lại
    }
}
