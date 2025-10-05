using LangMate.Persistence.NoSQL.MongoDB.Repository;
using LangMate.Abstractions.Abstracts.Settings;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace LangMate.Persistence.Tests
{
    public class MongoRepositoryTests
    {
        private readonly Mock<IMongoCollection<TestDocument>> _collectionMock;
        private readonly MongoRepository<TestDocument> _repository;

        public MongoRepositoryTests()
        {
            _collectionMock = new Mock<IMongoCollection<TestDocument>>();
            var settingsMock = new Mock<IMongoDbSettings>();
            settingsMock.SetupGet(s => s.DatabaseName).Returns("TestDb");

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:mongo", "mongodb://localhost:27017" }
                })
                .Build();

            // Create repository with required constructor arguments
            _repository = new MongoRepository<TestDocument>(settingsMock.Object, configuration);

            // Use reflection to inject the mock collection
            typeof(MongoRepository<TestDocument>)
                .GetField("_collection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_repository, _collectionMock.Object);
        }

        [Fact]
        public void InsertOne_CallsInsertOneOnCollection()
        {
            var doc = new TestDocument();
            _repository.InsertOne(doc);
            _collectionMock.Verify(c => c.InsertOne(
                doc,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void InsertMany_CallsInsertManyOnCollection()
        {
            var docs = new List<TestDocument> { new TestDocument(), new TestDocument() };
            _repository.InsertMany(docs);
            _collectionMock.Verify(c => c.InsertMany(
                docs,
                It.IsAny<InsertManyOptions>(),
                It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void InsertOneAsync_CallsInsertOneAsyncOnCollection()
        {
            var doc = new TestDocument();
            _repository.InsertOneAsync(doc);
            _collectionMock.Verify(c => c.InsertOneAsync(
                doc,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void InsertManyAsync_CallsInsertManyAsyncOnCollection()
        {
            var docs = new List<TestDocument> { new TestDocument(), new TestDocument() };
            _repository.InsertManyAsync(docs);
            _collectionMock.Verify(c => c.InsertManyAsync(
                docs,
                It.IsAny<InsertManyOptions>(),
                It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}