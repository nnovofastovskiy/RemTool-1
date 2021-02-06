﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

using RemTool.Models;


namespace RemTool.Services.Additional
{
    public class RtMailMessageService
    {
        public int SendEMailMessageToHQ(string HQeMail, RtRequest request, string credentialsName, string credentialsPass, string smtpHost, string smtpPort)
        {
            if (!HQeMail.Equals("") && !credentialsName.Equals("") && !credentialsPass.Equals(""))
            {
                try
                {
                    // отправитель - устанавливаем адрес и отображаемое в письме имя
                    MailAddress from = new MailAddress(credentialsName, "RemTool");

                    // кому отправляем
                    MailAddress to = new MailAddress(HQeMail);

                    // адрес smtp-сервера и порт, с которого будем отправлять письмо
                    SmtpClient smtp = new SmtpClient(smtpHost, int.Parse(smtpPort));

                    // создаем объект сообщения
                    using (MailMessage m = new MailMessage(from, to))
                    {
                        // тема письма
                        m.Subject = "Запрос";

                        // текст письма
                        m.Body = $"<h3>RemTool, запрос от {request.Name}</h3><p>{request.ReqInfo}</p><p>Email:{request.Email}</p>";

                        // письмо представляет код html
                        m.IsBodyHtml = true;

                        // логин и пароль
                        smtp.Credentials = new NetworkCredential(credentialsName, credentialsPass);

                        smtp.EnableSsl = true;
                        //smtp.EnableSsl = false;

                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Send(m);
                        return 1;       // всё хорошо
                    }
                }
                catch (SmtpException smtp_ex)
                {
                    Console.WriteLine("Error send mail:" + smtp_ex);
                    return 2;   // ошибка в smtp (в отправке почты)
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex);
                    return 3;   // ошибка в чом то ещё - мб формы незаполнены 
                }
            }
            return 4;   // отправки не было, т.к. основные поля отправки невалидны
        }
    
        public int SendEMailMessageToClient(string ClientMail, string messageText, string credentialsName, string credentialsPass, string smtpHost, string smtpPort)
        {
            if (!ClientMail.Equals("") && !credentialsName.Equals("") && !credentialsPass.Equals(""))
            {
                try
                {
                    // отправитель - устанавливаем адрес и отображаемое в письме имя
                    MailAddress from = new MailAddress(credentialsName, "RemTool");

                    // кому отправляем
                    MailAddress to = new MailAddress(ClientMail);

                    // адрес smtp-сервера и порт, с которого будем отправлять письмо
                    SmtpClient smtp = new SmtpClient(smtpHost, int.Parse(smtpPort));

                    // создаем объект сообщения
                    using (MailMessage m = new MailMessage(from, to))
                    {
                        // тема письма
                        m.Subject = "Запрос";

                        // текст письма
                        m.Body = $"<h3>{messageText}</h3>";

                        // письмо представляет код html
                        m.IsBodyHtml = true;

                        // логин и пароль
                        smtp.Credentials = new NetworkCredential(credentialsName, credentialsPass);

                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network; 

                        smtp.EnableSsl = true;
                        //smtp.EnableSsl = false;

                        smtp.Send(m);
                        return 1;       // всё хорошо
                    }
                }
                catch (SmtpException smtp_ex)
                {
                    Console.WriteLine("Error send mail:" + smtp_ex);
                    return 2;   // ошибка в smtp (в отправке почты)
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex);
                    return 3;   // ошибка в чом то ещё - мб формы незаполнены 
                }
            }
            return 4;
        }
    }
}
