using System.Text;

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
                Console.WriteLine("Please enter subject:");
                string subject = Console.ReadLine();
                Console.WriteLine("Please enter email text:");
                string multilineText = ReadMultiLineInput();
                await smtpClient.SendEmailAsync(subject, multilineText);
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

    private static string ReadMultiLineInput()
    {
        StringBuilder sb = new StringBuilder();

        string? line;

        while ((line = Console.ReadLine()) != null)
        {
            if (line.Trim().Equals("END"))
            {
                break;
            }
            sb.AppendLine(line);        
        }

        return sb.ToString();
    }
}
