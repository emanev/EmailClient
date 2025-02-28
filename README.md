
# EmailClient - Custom .NET Email Client

This is a custom-built email client for sending and receiving emails using the **SMTP** and **IMAP** protocols.  
The goal of this project is to demonstrate **manual implementation of these protocols**, without using third-party libraries.

---

## 🚀 Features

✅ Send emails using **SMTP** with SSL/TLS  
✅ Fetch email headers (Subject & Date) using **IMAP** with SSL/TLS  
✅ Configurable server settings via `config.json`  
✅ Error handling and retry mechanism  
✅ Supports Gmail (and other IMAP/SMTP servers)

---

## 📂 Project Structure

```
EmailClient/
├── EmailClient.sln                # Solution file
├── EmailClient/
│   ├── Program.cs                  # Entry point
│   ├── SmtpClientCustom.cs         # SMTP implementation
│   ├── ImapClientCustom.cs         # IMAP implementation
│   ├── EmailConfig.cs              # Config model and loader
│   ├── EmailClient.csproj          # Project file
│   ├── config.json (excluded)      # Configuration file (ignored by git)
├── .gitignore                      # Exclude unnecessary files
```

---

## ⚙️ Configuration (config.json)

Example `config.json` file (do NOT commit with real credentials):

```json
{
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 465,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",

    "ImapServer": "imap.gmail.com",
    "ImapPort": 993,
    "ImapUsername": "your-email@gmail.com",
    "ImapPassword": "your-app-password",

    "RecipientEmail": "recipient@example.com"
}
```

---

## 🛠️ How to Run

1. Configure `config.json` with your server and credentials.
2. Build the project using Visual Studio.
3. Run the application — select to send or fetch emails.

---

## ⚠️ Security Note
- Use **App Password** for Gmail.
- `config.json` is excluded from git by default to protect credentials.
- TLS/SSL is mandatory for both SMTP and IMAP.

---

## 📧 Example Flow

✅ Connect to `smtp.gmail.com` via TLS  
✅ Authenticate using `AUTH LOGIN` (Base64 username/password)  
✅ Send `MAIL FROM`, `RCPT TO` and `DATA` commands  
✅ Send email successfully!

✅ Connect to `imap.gmail.com` via TLS  
✅ Authenticate using `LOGIN`  
✅ Select `INBOX`  
✅ Fetch email headers with `FETCH 1:* (BODY[HEADER.FIELDS (SUBJECT FROM DATE)])`  
✅ List emails successfully!

---

## 👨‍💻 Author
**Emanuil Manev**  
📧 emanuil.manev@gmail.com

---

## 📜 License
This project is for educational purposes only.
