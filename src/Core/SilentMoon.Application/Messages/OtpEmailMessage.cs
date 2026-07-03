namespace SilentMoon.Application.Messages
{
    public class OtpEmailMessage
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string OtpCode { get; set; }
    }
}
