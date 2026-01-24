using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using OrderNotificationSystem;
using OrderNotificationSystem.Decorators;
using OrderNotificationSystem.Models;

namespace OrderNotificationSystemTests.Unit.Decorators
{
    public class LoggingSenderDecoratorTests
    {
        private INotificationSender _mockSender;
        private LoggingNotificationSenderDecorator _decorator;

        [SetUp]
        public void Setup()
        {
            _mockSender = Substitute.For<INotificationSender>();
            _decorator = new LoggingNotificationSenderDecorator(_mockSender);
        }

        [Test]
        public void Send_WhenSucceeds_PassThroughResult()
        {
            // Arrange
            var order = new Order { OrderNo = "ORD100", Amount = 300 };
            var expectedResult = new NotificationResult
            {
                Success = true,
                Message = "Notification sent successfully"
            };
            // 使用 Arg.Any 匹配任何Order（推薦做法）
            _mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
            //_mockSender.Send(order).Returns(expectedResult);

            // Act
            var result = _decorator.Send(order);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Notification sent successfully"));
            // 驗證底層的發送器被呼叫了 1 次
            _mockSender.Received(1).Send(order);
        }

        [Test]
        public void Send_WhenFails_PassFailureThrough()
        {
            // Arrange
            var order = new Order { OrderNo = "ORD101", Amount = 150 };
            var failureResult = new NotificationResult
            {
                Success = false,
                Message = "Failed to send notification"
            };
            // Arg.Any 匹配任何Order，都返回失敗結果
            _mockSender.Send(Arg.Any<Order>()).Returns(failureResult);

            // Act
            var result = _decorator.Send(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to send notification"));
            _mockSender.Received(1).Send(order);
        }

        [Test]
        public void Send_WhenThrowsException_ThrowsException()
        {
            var order = new Order { OrderNo = "ORD003", Amount = 500 };
            _mockSender.Send(order).Throws(new InvalidOperationException("Network error"));

            Assert.Throws<InvalidOperationException>(() => _decorator.Send(order));
        }
    }
}

