using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialChatApi.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Required]
        public Account User { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }

        protected Message()
        {

        }

        public Message(string content, Account user)
        {
            this.Content = content;
            this.User = user;
            this.Date = DateTime.UtcNow;
        }
    }
}
