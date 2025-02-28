using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

class ImapClientCustom
{
    private readonly string imapServer;
    private readonly int imapPort;
    private readonly string username;
    private readonly string password;

    public ImapClientCustom(EmailConfig config)
    {
        imapServer = config.ImapServer;
        imapPort = config.ImapPort;
        username = config.ImapUsername;
        password = config.ImapPassword;
    }

    public async Task FetchEmailsAsync()
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(imapServer, imapPort);
                using (NetworkStream networkStream = client.GetStream())
                using (SslStream sslStream = new SslStream(networkStream, false,
                    new RemoteCertificateValidationCallback((sender, cert, chain, sslPolicyErrors) => true)))
                {
                    await sslStream.AuthenticateAsClientAsync(imapServer);
                    using (StreamReader reader = new StreamReader(sslStream, Encoding.ASCII))
                    using (StreamWriter writer = new StreamWriter(sslStream, Encoding.ASCII) { AutoFlush = true })
                    {
                        await ReadResponse(reader);
                        await SendCommand(writer, reader, $"A1 LOGIN \"{username}\" \"{password}\"");
                        await SendCommand(writer, reader, "A2 SELECT INBOX");
                        await SendCommandWithFullResponse(writer, reader, "A3 FETCH 1:* (BODY[HEADER.FIELDS (SUBJECT FROM DATE)])");
                        await SendCommand(writer, reader, "A4 LOGOUT");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching emails: {ex.Message}");
            throw;
        }
    }

    private async Task ReadResponse(StreamReader reader)
    {
        string response;
        while ((response = await reader.ReadLineAsync()) != null)
        {
            Console.WriteLine("S: " + response);
            if (response.StartsWith("A") || response.StartsWith("*")) break;
        }
    }

    private async Task<string> ReadMultiLineResponse(StreamReader reader)
    {
        StringBuilder response = new StringBuilder();
        string line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            response.AppendLine(line);
            Console.WriteLine("S: " + line);
            
            if (line.StartsWith("A3 OK") || line.StartsWith("A3 NO") || line.StartsWith("A3 BAD"))
                break;

            if (line.Contains("{") && line.Contains("}"))
            {
                string emailData = await reader.ReadLineAsync();
                response.AppendLine(emailData);
                Console.WriteLine("Email Data: " + emailData);
            }
        }
        return response.ToString();
    }



    private async Task SendCommand(StreamWriter writer, StreamReader reader, string command, bool isFetch = false)
    {
        Console.WriteLine("C: " + command);
        await writer.WriteLineAsync(command);

        if (isFetch)
            await ReadMultiLineResponse(reader);
        else
            await ReadResponse(reader);
    }

    private async Task SendCommandWithFullResponse(StreamWriter writer, StreamReader reader, string command)
    {
        Console.WriteLine("C: " + command);
        await writer.WriteLineAsync(command);
        string response = await ReadMultiLineResponse(reader);

        Console.WriteLine("Full Response: \n" + response);
    }


}
