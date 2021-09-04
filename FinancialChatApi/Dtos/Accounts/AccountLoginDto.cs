using System.ComponentModel.DataAnnotations;

namespace FinancialChatApi.Dtos.Accounts
{
    public class AccountLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
