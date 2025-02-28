using System;
using System.IO;
using Newtonsoft.Json;

class EmailConfig
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string ImapServer { get; set; }
    public int ImapPort { get; set; }
    public string ImapUsername { get; set; }
    public string ImapPassword { get; set; }
    public string RecipientEmail { get; set; }  // Новото поле

    public static EmailConfig LoadConfig(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Config file not found!");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<EmailConfig>(json);
    }
}
