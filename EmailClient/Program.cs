class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            EmailConfig config = EmailConfig.LoadConfig("config.json");

            if (config == null)
            {
                Console.WriteLine("Error loading configuration.");
                return;
            }

            Console.WriteLine("1. Send Email");
            Console.WriteLine("2. Fetch Emails");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                SmtpClientCustom smtpClient = new SmtpClientCustom(config);
                await smtpClient.SendEmailAsync("Test Subject", "Hello, this is a test email.");
            }
            else if (choice == "2")
            {
                ImapClientCustom imapClient = new ImapClientCustom(config);
                await imapClient.FetchEmailsAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical error: {ex.Message}");
        }
    }

}
