namespace Ứng_dụng_bán_vé_xem_phim
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            // Khởi tạo database
            Task.Run(async () =>
            {
                await Data.MovieDatabase.InitAsync();
                await Data.UserData.InitAsync();
                await Data.TicketData.InitAsync();
                await Data.BookingData.InitAsync();
            });
        }
    }
}
