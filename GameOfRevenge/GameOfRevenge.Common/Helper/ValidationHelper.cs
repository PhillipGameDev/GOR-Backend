using System;
using System.Text.RegularExpressions;

namespace GameOfRevenge.Common.Helper
{
    public static class ValidationHelper
    {
        public static bool IsEmail(string value)
        {
            try
            {
                Email(value);
                return true;
            }
            catch (InvalidModelExecption)
            {
                try
                {
                    Password(value);
                    return false;
                }
                catch (Exception ex)
                {
                    throw new InvalidModelExecption("Invalid emaild or password", ex);
                }
            }
            catch (Exception)
            {
                throw new InvalidModelExecption("Invalid emaild or password");
            }
        }

        public static void PhoneNumber(string number)
        {
            var pattern1 = Regex.Match(number, @"^[0-9]{10}$").Success;
            var pattern2 = Regex.Match(number, @"^\+[0-9]{2}\s+[0-9]{2}\s+[0-9]{8}$").Success;
            var pattern3 = Regex.Match(number, @"^[0-9]{3}-[0-9]{4}-[0-9]{4}$").Success;
            bool isPhoneNumber = pattern1 || pattern2 || pattern3;
            if (!isPhoneNumber) throw new InvalidModelExecption("Invalid phone number was provided");
        }

        public static void Email(string email)
        {
            //bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            //Match match = regex.Match(email);
            //if (match.Success) returnMessage = string.Empty;
            //else returnMessage = "Invalid username was provided";

            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
            }
            catch (Exception ex)
            {
                throw new InvalidModelExecption("Invalid email id was provided", ex);
            }
            finally
            {
                //try
                //{
                //    string[] host = (email.Split('@'));
                //    string hostname = host[1];

                //    IPHostEntry IPhst = Dns.GetHostEntry(hostname);
                //    IPEndPoint endPt = new IPEndPoint(IPhst.AddressList[0], 25);
                //    Socket s = new Socket(endPt.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                //    s.Connect(endPt);
                //}
                //catch (Exception ex)
                //{
                //    throw new InvalidModelExecption("Invalid email was provided", ex);
                //}
            }
        }

        public static void Password(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new InvalidModelExecption("Password was not provided");
            else if (password.Length < 6 || password.Length > 36) throw new InvalidModelExecption("Password length must be between 6 to 32 characters");
        }

        public static void KeyId(int id, string errorMessage = "Invalid id was provided")
        {
            if (id <= 0) throw new InvalidModelExecption(errorMessage);
        }
    }
}
