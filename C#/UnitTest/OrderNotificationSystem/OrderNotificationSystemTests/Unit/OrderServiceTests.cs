using NSubstitute;
using OrderNotificationSystem.Models;
using OrderNotificationSystem;
using OrderNotificationSystem.Service;



namespace OrderNotificationSystemTests.Unit
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
        public void PlaceOrder_When_ValidatorPasses_ReturnsSuccess()
        {
            // Arrange
            var order = new Order { OrderNo = "order001", Amount = 1 };
            var validationResult = new NotificationResult { Success = true, Message = "驗證通過" };
            _mockValidator.Validate(order).Returns(validationResult);
            var orderService = new OrderService(_mockValidator, _mockSender);

            // Act 
            var result = orderService.PlaceOrder(order);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo($"{order.OrderNo}:金額為{order.Amount}元"));

            // ✅ 驗證 Send 被呼叫
            _mockSender.Received(1).Send(order);
        }

        [Test]
        public void PlaceOrder_When_ValidatorFails_ReturnsFailed()
        {
            // Arrange
            var order = new Order { OrderNo = "order001", Amount = 0 };
            var validationResult = new NotificationResult { Success = false, Message = "訂單錯誤" };
            _mockValidator.Validate(order).Returns(validationResult);
            var orderService = new OrderService(_mockValidator,_mockSender);

            // Act 
            var result = orderService.PlaceOrder(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo($"訂單錯誤"));

            // ✅ 驗證 Send 沒有被呼叫（因為驗證失敗）
            _mockSender.DidNotReceive().Send(Arg.Any<Order>());
        }

        [Test]
        public void FakeNotificationSender_When_ShouldSucceedIsTrue_ReturnSuccess()
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

        [Test]
        public void FakeNotificationSender_When_ShouldSucceedIsFalse_ReturnFailure()
        {
            // Arrange
            var order = new Order { OrderNo = "order003", Amount = 100 };
            INotificationSender fakeSender = new FakeNotificationSender(shouldSucceed: false);

            // Act
            var result = fakeSender.Send(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to send notification."));
        }

        #region DefaultOrderValidator 測試

        [Test]
        public void DefaultOrderValidator_When_AmountIsPositive_ReturnsSuccess()
        {
            // Arrange
            var order = new Order { OrderNo = "order004", Amount = 100 };
            IOrderValidator validator = new DefaultOrderValidator();

            // Act
            var result = validator.Validate(order);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("驗證通過"));
        }

        [Test]
        public void DefaultOrderValidator_When_AmountIsZero_ReturnsFailed()
        {
            // Arrange
            var order = new Order { OrderNo = "order005", Amount = 0 };
            IOrderValidator validator = new DefaultOrderValidator();

            // Act
            var result = validator.Validate(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("訂單錯誤"));
        }

        [Test]
        public void DefaultOrderValidator_When_AmountIsNegative_ReturnsFailed()
        {
            // Arrange
            var order = new Order { OrderNo = "order006", Amount = -50 };
            IOrderValidator validator = new DefaultOrderValidator();

            // Act
            var result = validator.Validate(order);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("訂單錯誤"));
        }

        #endregion

        #region 整合測試 (使用真實實作)

        [Test]
        public void PlaceOrder_With_RealValidator_And_RealSender_CompleteFlow()
        {
            // Arrange
            var order = new Order { OrderNo = "ORD-2024-001", Amount = 1000 };
            IOrderValidator validator = new DefaultOrderValidator();
            INotificationSender sender = new EmailNotificationSender();
            var orderService = new OrderService(validator, sender);

            // Act
            var result = orderService.PlaceOrder(order);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo($"{order.OrderNo}:金額為{order.Amount}元"));
        }

        [Test]
        public void PlaceOrder_With_RealValidator_FailedValidation_DoesNotSend()
        {
            // Arrange
            var order = new Order { OrderNo = "ORD-2024-002", Amount = -100 };
            IOrderValidator validator = new DefaultOrderValidator();
            var mockSender = Substitute.For<INotificationSender>();
            var orderService = new OrderService(validator, mockSender);

            // Act
            var result = orderService.PlaceOrder(order);

            // Assert
            Assert.That(result.Success, Is.False);
            mockSender.DidNotReceive().Send(Arg.Any<Order>());
        }

        #endregion
    }
}
