using AutoMapper;
using FinancialChatApi.Dtos.Messages;
using FinancialChatApi.Mappings;
using FinancialChatApi.Messsaging;
using FinancialChatApi.Models;
using FinancialChatApi.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FinancialChatApi.Test.Messaging
{
    [TestClass]
    public class MessageHubTest
    {
        private readonly MessageHub _messageHub;
        private readonly Mock<IDataProtectionProvider> _dataProtectionProviderMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMessageService> _messageServiceMock;
        private readonly Mock<UserManager<Account>> _userManagerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IHubCallerClients> _hubCallerClientsMock;

        public MessageHubTest()
        {
            _messageServiceMock = new Mock<IMessageService>();
            var users = new List<Account>();
            _userManagerMock = MockUserManager(users);
            _dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
            _configurationMock = new Mock<IConfiguration>();

            var dataProtectorMock = new Mock<IDataProtector>();
            _dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
            _dataProtectionProviderMock.Setup(d => d.CreateProtector(It.IsAny<string>()))
                .Returns(dataProtectorMock.Object);

            _hubCallerClientsMock = new Mock<IHubCallerClients>();

            var configurationMock = new Mock<IConfiguration>();

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(_dataProtectionProviderMock.Object, configurationMock.Object));
            }).CreateMapper();

            _messageHub = new MessageHub(_messageServiceMock.Object, _userManagerMock.Object, _mapper, _dataProtectionProviderMock.Object, _configurationMock.Object);
        }


        [TestMethod]
        public async Task Message_Test()
        {
            // Arrenge
            var content = "Content";

            _messageHub.Clients = MockHubCallerClients();
            _messageHub.Context = MockHubCallerContext();

            // Act
            await _messageHub.Message(content);

            // Assert
            _userManagerMock.Verify(m => m.FindByNameAsync(_messageHub.Context.User.Identity.Name));
            _hubCallerClientsMock.Verify(m => m.All);
        }

        private IHubCallerClients MockHubCallerClients()
        {
            var clientProxyMock = new Mock<IClientProxy>();

            
            _hubCallerClientsMock.SetupGet(h => h.All)
                .Returns(clientProxyMock.Object);

            return _hubCallerClientsMock.Object;
        }

        private HubCallerContext MockHubCallerContext()
        {
            var identityMock = new Mock<IIdentity>();
            var claimsMock = new Mock<ClaimsPrincipal>();
            claimsMock.Setup(m => m.Identity).Returns(identityMock.Object);
            var hubCallerContextMock = new Mock<HubCallerContext>();
            hubCallerContextMock.SetupGet(h => h.User)
                .Returns(claimsMock.Object);
            return hubCallerContextMock.Object;
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
    }
}
