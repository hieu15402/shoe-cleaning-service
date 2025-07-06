using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using TP4SCS.Library.Utils.Healpers;
using TP4SCS.Services.Interfaces;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        const int maxRetries = 3;
        var delay = TimeSpan.FromSeconds(2);

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    var mailMessage = new MimeMessage();
                    mailMessage.From.Add(new MailboxAddress("ShoeCareHub", _emailSettings.SenderEmail));
                    mailMessage.To.Add(new MailboxAddress(string.Empty, toEmail));
                    mailMessage.Subject = subject;
                    mailMessage.Body = new TextPart(TextFormat.Html) { Text = body };

                    smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                    if (!string.IsNullOrEmpty(_emailSettings.SenderEmail) && !string.IsNullOrEmpty(_emailSettings.SenderPassword))
                    {
                        await smtpClient.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                    }

                    await smtpClient.SendAsync(mailMessage);

                    _logger.LogInformation($"Email sent successfully to {toEmail}.");
                    return;
                }
                catch (SmtpCommandException smtpEx)
                {
                    _logger.LogError(smtpEx, $"SMTP command error (Code: {smtpEx.StatusCode}) while sending email to {toEmail}.");
                }
                catch (SmtpProtocolException protocolEx)
                {
                    _logger.LogError(protocolEx, $"SMTP protocol error while sending email to {toEmail}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unexpected error while sending email to {toEmail}.");
                }
                finally
                {
                    try
                    {
                        if (smtpClient.IsConnected)
                        {
                            await smtpClient.DisconnectAsync(true);
                        }
                    }
                    catch (Exception disconnectEx)
                    {
                        _logger.LogWarning(disconnectEx, "Error while disconnecting SMTP client.");
                    }
                }
            }

            if (attempt < maxRetries)
            {
                _logger.LogWarning($"Retrying to send email to {toEmail}. Attempt {attempt} of {maxRetries}.");
                await Task.Delay(delay);
            }
            else
            {
                _logger.LogError($"Failed to send email to {toEmail} after {maxRetries} attempts.");
            }
        }
    }
}
