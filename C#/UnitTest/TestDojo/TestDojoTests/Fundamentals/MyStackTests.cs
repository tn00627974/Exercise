using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDojo.Fundamentals;

namespace TestDojoTests.Fundamentals
{
    [TestFixture]
    internal class MyStackTests
    {
        [Test]
        public void Count_WhenNoEmpty_ReturnCorrectCount()
        {
            // 3A
            // Arrange
            var myStack = new MyStack<int>();

            // Act
            myStack.Push(1);
            myStack.Push(2);
            myStack.Push(3);

            // Assert
            Assert.That(myStack.Count == 3);
        }

        [Test]
        public void Push_WhenNoEmpty_ReturnCorrectValue()
        { 
            var myStack = new MyStack<int>();
            myStack.Push(1);
            myStack.Push(2);
            myStack.Push(3);
            Assert.That(myStack.Count == 3);
        }

        [Test]
        public void Pop_WhenNoEmpty_ReturnCorrectValue()
        {
            var myStack = new MyStack<int>();
            myStack.Push(1);
            myStack.Push(2);
            myStack.Push(3);
            var result = myStack.Pop();
            Assert.That(result == 3);
            Assert.That(myStack.Count == 2);
        }

        [Test]
        public void Peek_WhenNoEmpty_ReturnCorrectValue()
        {
            var myStack = new MyStack<int>();
            myStack.Push(1);
            myStack.Push(2);
            var result = myStack.Peek();
            Assert.That(result == 2);
            Assert.That(myStack.Count == 2);
        }

    }
}
