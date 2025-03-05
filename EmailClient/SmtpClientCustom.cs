using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

class SmtpClientCustom
{
    private readonly string smtpServer;
    private readonly int smtpPort;
    private readonly string username;
    private readonly string password;
    private readonly string recipient;

    public SmtpClientCustom(EmailConfig config)
    {
        smtpServer = config.SmtpServer;
        smtpPort = config.SmtpPort;
        username = config.SmtpUsername;
        password = config.SmtpPassword;
        recipient = config.RecipientEmail;
    }

    public async Task SendEmailAsync(string subject, string body)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort);
                using (NetworkStream networkStream = client.GetStream())
                using (SslStream sslStream = new SslStream(networkStream, false,
                    new RemoteCertificateValidationCallback((sender, cert, chain, sslPolicyErrors) => true)))
                {
                    await sslStream.AuthenticateAsClientAsync(smtpServer);
                    using (StreamReader reader = new StreamReader(sslStream, Encoding.ASCII))
                    using (StreamWriter writer = new StreamWriter(sslStream, Encoding.ASCII) { AutoFlush = true })
                    {
                        await ReadResponse(reader);
                        await SendCommand(writer, reader, $"EHLO {smtpServer}");
                        await SendCommand(writer, reader, "AUTH LOGIN");
                        await SendCommand(writer, reader, Convert.ToBase64String(Encoding.UTF8.GetBytes(username)));
                        await SendCommand(writer, reader, Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));
                        await SendCommand(writer, reader, $"MAIL FROM:<{username}>");
                        await SendCommand(writer, reader, $"RCPT TO:<{recipient}>");
                        await SendCommand(writer, reader, "DATA");

                        await writer.WriteLineAsync($"Subject: {subject}\r");
                        await writer.WriteLineAsync($"From: {username}\r");
                        await writer.WriteLineAsync($"To: {recipient}\r");
                        await writer.WriteLineAsync("\r");
                        await writer.WriteLineAsync(body);
                        await writer.WriteLineAsync(".");

                        await ReadResponse(reader);
                        await SendCommand(writer, reader, "QUIT");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }


    private async Task ReadResponse(StreamReader reader)
    {
        string response;
        do
        {
            response = await reader.ReadLineAsync();
            Console.WriteLine("S: " + response);
        } while (!string.IsNullOrEmpty(response) && (response[3] != ' '));
    }

    private async Task SendCommand(StreamWriter writer, StreamReader reader, string command, bool expectMultiple = true)
    {
        Console.WriteLine("C: " + command);
        await writer.WriteLineAsync(command);
        if (expectMultiple)
        {
            await ReadResponse(reader);
        }
    }
}
