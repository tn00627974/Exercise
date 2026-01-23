using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OrderNotificationSystem;
using OrderNotificationSystem.Models;


namespace OrderNotificationSystemTests.Integration
{
    #region 整合測試 (使用真實實作)

    public class OrderServiceIntegrationTests
    {
        // 宣告變數
        private INotificationSender _senders;
        private IOrderValidator _validator;

        [SetUp]
        public void Setup()
        {
            // 初始化組合發送器
            _senders = new CompositeNotificationSender(
                new EmailNotificationSender(),
                new LineNotificationSender(),
                new FakeNotificationSender(true)
            );
            
            // 初始化驗證器
            _validator = new DefaultOrderValidator();
        }

        [Test]
        public void PlaceOrder_WithValidOrder_AllSendersExecuteAndReturnsSuccess()
        {
            // Arrange
            var order = new Order { OrderNo = "order001", Amount = 1000 };
            var orderService = new OrderService(_validator, _senders);
            
            // Act 
            var result = orderService.PlaceOrder(order);
            
            // Assert - 驗證訂單成功
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo($"{order.OrderNo}:金額為{order.Amount}元"));
        }

        [Test]
        public void PlaceOrder_WithNegativeAmount_ValidationFailsAndNotificationSkipped()
        {
            // Arrange
            var order = new Order { OrderNo = "order002", Amount = -1 };
            var orderService = new OrderService(_validator, _senders);
            
            // Act 
            var result = orderService.PlaceOrder(order);
            
            // Assert - 驗證訂單失敗，不會發送通知
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("訂單錯誤"));
        }

        [Test]
        public void PlaceOrder_WithZeroAmount_ValidationFailsAndNotificationSkipped()
        {
            // Arrange
            var order = new Order { OrderNo = "order003", Amount = 0 };
            var orderService = new OrderService(_validator, _senders);
            
            // Act 
            var result = orderService.PlaceOrder(order);
            
            // Assert - 金額為 0 也應該失敗
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("訂單錯誤"));
        }
    }
    #endregion
}
