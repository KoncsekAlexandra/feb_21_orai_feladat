namespace WFAC220221
{
    internal static class Program
    {
        internal static string ConnectionString => 
            "Server = (localdb)\\MSSQLLocalDB;" +
            "Database = tanverseny;" +
            "Integrated Security = True;";
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmMain());
        }
    }
}