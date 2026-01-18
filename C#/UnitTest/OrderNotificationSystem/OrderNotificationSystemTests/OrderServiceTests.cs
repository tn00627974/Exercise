using NSubstitute;
using OrderNotificationSystem.Models;
using OrderNotificationSystem;


namespace OrderNotificationSystemTests
{
    public class OrderServiceTests
    {
        // 宣告變數
        private INotificationSender _mockSender;
        private IOrderValidator _mockValidator;

        [SetUp]
        public void Setup()
        {
            // 初始化變數
            _mockSender = Substitute.For<INotificationSender>();
            _mockValidator = Substitute.For<IOrderValidator>();
        }

        [Test]
        public void PlaceOrder_When_OrderAmountIsPositive_ReturnsSuccess()
        {
            // Arrange
            var order = new Order { OrderNo = "order001", Amount = 1 };
            var validationResult = new NotificationResult { Success = true, Message = "驗證通過" };
            _mockValidator.Validate(order).Returns(validationResult);
            var orderService = new OrderService(_mockSender, _mockValidator);

            // Act 
            var result = orderService.PlaceOrder(order);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo($"{order.OrderNo}:金額為{order.Amount}元"));

        }
        [Test]
        public void PlaceOrder_When_OrderAmountIsPositive_ReturnsFailed()
        {
            // Arrange
            var order = new Order { OrderNo = "order001", Amount = 0 };
            var validationResult = new NotificationResult { Success = false, Message = "訂單錯誤" };
            _mockValidator.Validate(order).Returns(validationResult);
            var orderService = new OrderService(_mockSender, _mockValidator);

            // Act 
            var result = orderService.PlaceOrder(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo($"訂單錯誤"));
        }
        // 測試 FakeNotificationSender 的 _shouldSucceed 為 true 的情況
        [Test]
        public void FakeNotificationSender_ShouldSucceed_ReturnsSuccess()
        {
            // Arrange
            var order = new Order { OrderNo = "order002", Amount = 100 };
            INotificationSender fakeSender = new FakeNotificationSender(shouldSucceed: true);
            // Act
            var result = fakeSender.Send(order);
            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo($"{order.OrderNo}:Notification sent successfully."));
        }
    }
}
