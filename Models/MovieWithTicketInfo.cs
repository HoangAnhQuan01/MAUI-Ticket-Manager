namespace Ứng_dụng_bán_vé_xem_phim.Models
{
    public class MovieWithTicketInfo
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string TicketInfo { get; set; }
        public int TicketQuantity { get; set; }
        public string PosterUrl { get; set; }
    }
}
