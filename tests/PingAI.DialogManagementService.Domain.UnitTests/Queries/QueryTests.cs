using System;
using PingAI.DialogManagementService.Domain.Model;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Domain.UnitTests.Queries
{
    public class QueryTests
    {
        [Fact]
        public void InsertUpdatesDisplayOrders()
        {
            // Arrange
            var q1 = CreateQueryWithDisplayOrder(0);
            var q2 = CreateQueryWithDisplayOrder(1);
            var q3 = CreateQueryWithDisplayOrder(2);
            var q4 = CreateQueryWithDisplayOrder(4);
            var q = CreateQueryWithDisplayOrder(1);

            // Act
            q.Insert(new []{q1, q2, q3, q4});
            
            // Assert
            Equal(0, q1.DisplayOrder);
            Equal(1, q.DisplayOrder);
            Equal(2, q2.DisplayOrder);
            Equal(3, q3.DisplayOrder);
            Equal(4, q4.DisplayOrder);
        }

        [Fact]
        public void SwapDisplayOrder()
        {
            // Arrange
            var q1 = CreateQueryWithDisplayOrder(1);
            var q2 = CreateQueryWithDisplayOrder(2);
            
            // Arrange
            q2.Swap(q1);
            
            // Assert
            Equal(2, q1.DisplayOrder);
            Equal(1, q2.DisplayOrder);
        }
        
        private static Query CreateQueryWithDisplayOrder(int displayOrder) => new Query(
            Guid.NewGuid().ToString(), Guid.Parse("0c8b446c-142b-42b6-8e16-44d545f03de2"),
            new Expression[0], Guid.NewGuid().ToString(), null, displayOrder);
    }
}
