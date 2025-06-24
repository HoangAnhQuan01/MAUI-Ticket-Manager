using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Ứng_dụng_bán_vé_xem_phim.Models
{
    public class Booking
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int UserId { get; set; }     // Ai đặt vé

        public int MovieId { get; set; }    // Phim nào

        public int TicketId { get; set; }   // Loại vé nào

        public int QuantityPurchased { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime PurchaseDate { get; set; }
    }
}
