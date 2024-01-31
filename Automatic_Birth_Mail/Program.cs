using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using Automatic_Birth_Mail;
class Program
{
    static void Main()
    {
        string smtpServer = "smtp.gmail.com";
        int smtpPort = 587;
        string smtpUsername = "rohitgabane1234@gmail.com";
        string smtpPassword = "fzir fvrv rjnf xtzq";
        // Replace these values with the sender's email address and display name
        string senderEmail = "rohitgabane1234@gmail.com";
        string senderName = "Rohit Gabane";

        string connectionString = "Data Source=DESKTOP-OOM2M0F\\MSSQLSERVER01;Initial Catalog=Autometed_Email;Integrated Security=True";
        // Fetch birthday people from the database
        var birthdayPeople = GetBirthdayPeople(connectionString);
        // Send birthday emails
        foreach (var person in birthdayPeople)
        {
            // Replace these values with the actual message content
           
            string subject = "Happy Birthday!";
            string body = $"Dear {person.FirstName},\n\nHappy Birthday! Wishing you a fantastic day and a wonderful year ahead.";
            body += $" On this special day, we celebrate you and the positive impact you bring to our team. Your hard work and dedication do not go unnoticed. Here's to another year of success and achievements!\n\n";
            body += $"Best regards,\n{senderName}";
            // Send the birthday email
            SendEmail(smtpServer, smtpPort, smtpUsername, smtpPassword, senderEmail, senderName, person.Email_ID, person.FirstName, subject, body);
        }
    }
    static void SendEmail(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string senderEmail, string senderName, string recipientEmail, string recipientName, string subject, string body)
    {
        using (var client = new SmtpClient(smtpServer))
        {
            client.Port = smtpPort;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            client.EnableSsl = true;
            var message = new MailMessage();
            message.From = new MailAddress(senderEmail, senderName);
            message.To.Add(new MailAddress(recipientEmail, recipientName));
            message.Subject = subject;
            message.Body = body;
            client.Send(message);
        }
        Console.WriteLine($"Birthday email sent successfully to {recipientEmail}!");
    }
    static Employee[] GetBirthdayPeople(string connectionString)
    {
        var birthdayPeople = new List<Employee>();
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("SELECT Emp_ID, FirstName, LastName, BirthDate, Email_ID FROM Employee WHERE MONTH(BirthDate) = @Month AND DAY(BirthDate) = @Day", connection))
            {
                command.Parameters.AddWithValue("@Month", DateTime.Now.Month);
                command.Parameters.AddWithValue("@Day", DateTime.Now.Day);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var person = new Employee
                        {
                            Emp_ID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            BirthDate = reader.GetDateTime(3),
                            Email_ID = reader.GetString(4)
                        };
                        birthdayPeople.Add(person);
                    }
                }
            }
        }
        return birthdayPeople.ToArray();
    }

}