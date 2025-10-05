namespace HotkeyTyper;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // Check for test mode
        if (args.Length > 0 && args[0] == "--test")
        {
            // Run data layer tests
            DataLayerTest.RunTests();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }    
}