using System;
using System.Collections.Generic;
using FluentAssertions;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using Xunit;
using static PingAI.DialogManagementService.Domain.Model.ResolutionPart;

namespace PingAI.DialogManagementService.Domain.UnitTests.Responses
{
    public class ResponseTests
    {
        [Fact]
        public void SetRteWithTextOnly()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var sut = new Response(Guid.NewGuid(), new ResolutionPart[0],
                projectId, ResponseType.RTE, 0);
            
            // Act
            sut.SetRte("Hello, World!", new Dictionary<string, EntityName>());
            
            // Assert
            sut.Resolution.Should().BeEquivalentTo(
                TextPart("Hello, World!")
            );
        }

        [Fact]
        public void SetRteWithOnlyParam()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var sut = new Response(Guid.NewGuid(), new ResolutionPart[0],
                projectId, ResponseType.RTE, 0);
            var entityNames = new Dictionary<string, EntityName>
            {
                { "foo", new EntityName(Guid.NewGuid(), "foo", projectId, true) },
            };
            
            // Act
            sut.SetRte("${foo}", entityNames);
            
            // Assert
            sut.Resolution.Should().BeEquivalentTo(
                EntityNamePart(entityNames["foo"].Id)
            ); 
        }
        
        [Fact]
        public void SetRteWithParams()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var sut = new Response(Guid.NewGuid(), new ResolutionPart[0],
                projectId, ResponseType.RTE, 0);
            var entityNames = new Dictionary<string, EntityName>
            {
                { "foo", new EntityName(Guid.NewGuid(), "foo", projectId, true) },
                { "bar", new EntityName(Guid.NewGuid(), "bar", projectId, true)}
            };
            
            // Act
            sut.SetRte("Hello, I'm ${foo} from ${bar}!", entityNames);
            
            // Assert
            sut.Resolution.Should().BeEquivalentTo(
                TextPart("Hello, I'm "), 
                EntityNamePart(entityNames["foo"].Id),
                TextPart(" from "), 
                EntityNamePart(entityNames["bar"].Id), 
                TextPart("!"));
            sut.Type.Should().Be(ResponseType.RTE);
        }
        
        [Fact]
        public void SetRteWithNonExistentEntityName()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var sut = new Response(Guid.NewGuid(), new ResolutionPart[0],
                projectId, ResponseType.RTE, 0);
            var entityNames = new Dictionary<string, EntityName>
            {
                { "foo", new EntityName(Guid.NewGuid(), "foo", projectId, true) },
            };
            
            // Act
            void Act() => sut.SetRte("Hi, ${bar}", entityNames);

            // Assert
            Assert.Throws<BadRequestException>(Act);
        }
    }
}