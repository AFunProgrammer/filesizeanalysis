namespace favs
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmFavs());
        }
    }
}