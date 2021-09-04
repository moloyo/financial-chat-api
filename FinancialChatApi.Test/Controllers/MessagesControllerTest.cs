using AutoMapper;
using FinancialChatApi.Controllers;
using FinancialChatApi.Dtos.Messages;
using FinancialChatApi.Mappings;
using FinancialChatApi.Models;
using FinancialChatApi.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialChatApi.Test.Controllers
{
    [TestClass]
    public class MessagesControllerTest
    {
        private readonly MessagesController _controller;
        private readonly Mock<IMessageService> _messageServiceMock;
        private readonly IMapper _mapper;

        public MessagesControllerTest()
        {
            _messageServiceMock = new Mock<IMessageService>();

            var dataProtectorMock = new Mock<IDataProtector>();
            var dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
            dataProtectionProviderMock.Setup(d => d.CreateProtector(It.IsAny<string>()))
                .Returns(dataProtectorMock.Object);

            var configurationMock = new Mock<IConfiguration>();

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(dataProtectionProviderMock.Object, configurationMock.Object));
            }).CreateMapper();

            _controller = new MessagesController(_messageServiceMock.Object, _mapper);
        }


        [TestMethod]
        public async Task GetAll_Test()
        {
            // Arrenge
            var user = new Account("username");
            var messages = new List<Message>() 
            {
                new Message("content", user)
            };
            _messageServiceMock.Setup(m => m.GetLatestMessagesAsync(It.IsAny<int>()))
                .ReturnsAsync(messages);

            // Act
            var response = await _controller.GetAll();

            // Assert
            var result = response as OkObjectResult;
            var messagesDtos = result.Value as IEnumerable<MessageDto>;
            _messageServiceMock.Verify(m => m.GetLatestMessagesAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(messages[0].User.UserName, messagesDtos.First().UserName);
        }

        [TestMethod]
        public async Task DeleteAll_Test()
        {
            // Arrenge

            // Act
            var response = await _controller.DeleteAll();

            // Assert
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
            _messageServiceMock.Verify(m => m.DeleteAllAsync(), Times.Once);
        }
    }
}
