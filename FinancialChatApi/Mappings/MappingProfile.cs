using AutoMapper;
using FinancialChatApi.Dtos.Accounts;
using FinancialChatApi.Dtos.Messages;
using FinancialChatApi.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FinancialChatApi.Mappings
{
    public class MappingProfile : Profile
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IConfiguration _configuration;

        public MappingProfile(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _configuration = configuration;

            CreateAccountMappings();
            CreateMessageMappings();
        }

        private void CreateAccountMappings()
        {
            CreateMap<AccountRegisterDto, Account>()
                .ConstructUsing(a => new Account(a.Email));
        }

        private void CreateMessageMappings()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => Decrypt(src.Content)))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
        }

        

        private string Decrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(_configuration["EncryptionKey"]);
            return protector.Unprotect(input);
        }
    }
}
