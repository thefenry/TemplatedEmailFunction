namespace EmailService.Models
{
    public class EmailAdressInfo
    {
        public EmailAdressInfo(string emailAddress)
        {
            Email = emailAddress;
        }

        public string Email { get; set; }

        private string? _name;

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_name))
                {
                    return Email;
                }

                return _name;
            }
            set => _name = value;
        }
    }
}
