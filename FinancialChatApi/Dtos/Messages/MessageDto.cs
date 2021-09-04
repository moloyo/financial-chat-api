using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialChatApi.Dtos.Messages
{
    public class MessageDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }

        public string UserName { get; set; }

        public DateTime Date { get; set; }
    }
}
