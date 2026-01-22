using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OrderNotificationSystem;
using OrderNotificationSystem.Models;


namespace OrderNotificationSystemTests.Unit
{
    public class CompositeNotificationSenderTests
    {
        [Test]
        public void Send_WithMultipleSenders_AllSuccess()
        {
            // Arrange
            var order = new Order { OrderNo = "ORD001", Amount = 1000 };

            var emailSender = Substitute.For<INotificationSender>();
            var lineSender = Substitute.For<INotificationSender>();

            emailSender.Send(Arg.Any<Order>()).Returns(new NotificationResult
            {
                Success = true,
                Message = "Email sent"
            });

            lineSender.Send(Arg.Any<Order>()).Returns(new NotificationResult
            {
                Success = true,
                Message = "Line sent"
            });

            var composite = new CompositeNotificationSender(emailSender, lineSender);

            // Act
            var result = composite.Send(order);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Contains.Substring("2"));

            // 👉 驗證兩個發送器都被呼叫
            emailSender.Received(1).Send(order);
            lineSender.Received(1).Send(order);
        }

        [Test]
        public void Send_WhenOneSenderFails_ReturnsFailure()
        {
            // Arrange
            var order = new Order { OrderNo = "ORD002", Amount = 500 };

            var emailSender = Substitute.For<INotificationSender>();
            var lineSender = Substitute.For<INotificationSender>();

            emailSender.Send(Arg.Any<Order>()).Returns(new NotificationResult
            {
                Success = true,
                Message = "Email sent"
            });

            lineSender.Send(Arg.Any<Order>()).Returns(new NotificationResult
            {
                Success = false,
                Message = "Line service down"
            });

            var composite = new CompositeNotificationSender(emailSender, lineSender);

            // Act
            var result = composite.Send(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Contains.Substring("Line service down"));
        }

        [Test]
        public void Send_WhenNoSendersConfigured_ReturnsFailed()
        {
            // Arrange
            var order = new Order { OrderNo = "ORD003", Amount = 100 };
            var composite = new CompositeNotificationSender();

            // Act
            var result = composite.Send(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Contains.Substring("沒有配置"));
        }

        [Test]
        public void Send_WhenSenderThrowsException_CatchesAndContinues()
        {
            // Arrange
            var order = new Order { OrderNo = "ORD004", Amount = 200 };

            var faultySender = Substitute.For<INotificationSender>();
            var healthySender = Substitute.For<INotificationSender>();

            faultySender.Send(Arg.Any<Order>()).Throws(new Exception("Connection timeout"));

            healthySender.Send(Arg.Any<Order>()).Returns(new NotificationResult
            {
                Success = true,
                Message = "Healthy sender OK"
            });

            var composite = new CompositeNotificationSender(faultySender, healthySender);

            // Act
            var result = composite.Send(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Contains.Substring("Connection timeout"));
        }
    }
}
