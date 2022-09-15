using System.IO;

namespace EmailService.Models
{
    public class EmailAttachment
    {
        public string Name { get; set; }

        public Stream Stream { get; set; }
    }
}
