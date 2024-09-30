using Azure;
using Azure.Data.Tables;
using JokeStorageFunctionApp.Models;
using JokeStorageFunctionApp.Services;
using Moq;

namespace JokeStorageFunctionApp.Tests;

public class LogServiceTests
{
    [Fact]
    public async Task AddLogAsync_ShouldAddLog()
    {
        var tableClientMock = new Mock<TableClient>();
        var tableServiceClientMock = new Mock<TableServiceClient>();
        tableServiceClientMock.Setup(x => x.GetTableClient(It.IsAny<string>()))
            .Returns(tableClientMock.Object);

        var logService = new LogService(tableServiceClientMock.Object, "LogsTable");

        var logEntry = new LogEntry
        {
            PartitionKey = "log-partition",
            RowKey = Guid.NewGuid().ToString(),
            TimeStamp = DateTime.UtcNow,
            Status = "Success",
            Message = "Log message"
        };

        await logService.AddLogAsync(logEntry);

        tableClientMock.Verify(x => x.AddEntityAsync(logEntry, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLogsAsync_ShouldReturnLogsInDateRange()
    {
        var from = DateTime.UtcNow.AddDays(-2);
        var to = DateTime.UtcNow;

        var logEntries = new List<LogEntry>
        {
            new() { PartitionKey = "log-partition", RowKey = Guid.NewGuid().ToString(), TimeStamp = from.AddHours(1), Status = "Success", Message = "Log message 1" },
            new() { PartitionKey = "log-partition", RowKey = Guid.NewGuid().ToString(), TimeStamp = from.AddHours(2), Status = "Failure", Message = "Log message 2" }
        };

        var tableClientMock = new Mock<TableClient>();
        var tableServiceClientMock = new Mock<TableServiceClient>();
        tableServiceClientMock.Setup(x => x.GetTableClient(It.IsAny<string>()))
            .Returns(tableClientMock.Object);

        var asyncEnumerable = GetAsyncEnumerable(logEntries);

        tableClientMock.Setup(x => x.QueryAsync<LogEntry>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(asyncEnumerable);

        var logService = new LogService(tableServiceClientMock.Object, "LogsTable");

        var result = await logService.GetLogsAsync(from, to);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, log => log.Message == "Log message 1");
        Assert.Contains(result, log => log.Message == "Log message 2");
    }

    private static AsyncPageable<LogEntry> GetAsyncEnumerable(IEnumerable<LogEntry> logEntries)
    {
        return AsyncPageable<LogEntry>.FromPages(new[] { Page<LogEntry>.FromValues(logEntries.ToList(), continuationToken: null, Mock.Of<Response>()) });
    }

}