using NSubstitute;
using OrderNotificationSystem;
using OrderNotificationSystem.Models;

namespace OrderNotificationSystemTests
{
    public class OrderServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PlaceOrder_When_OrderAmountIsPositive_ReturnsSuccess()
        {
            // Arrange
            var order = new Order { OrderNo = "order001", Amount = 1 };
            var mockSender = Substitute.For<INotificationSender>();
            var orderService = new OrderService(mockSender);

            // Act 
            var result = orderService.PlaceOrder(order);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo($"訂單金額為{order.Amount}元"));
        }
        [Test]
        public void PlaceOrder_When_OrderAmountIsPositive_ReturnsFailed()
        {
            // Arrange
            var order = new Order { OrderNo = "order001", Amount = 0 };
            var mockSender = Substitute.For<INotificationSender>();
            var orderService = new OrderService(mockSender);

            // Act 
            var result = orderService.PlaceOrder(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("訂單金額錯誤"));
        }
    }
}
