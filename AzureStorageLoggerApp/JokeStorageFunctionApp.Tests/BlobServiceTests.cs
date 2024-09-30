using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JokeStorageFunctionApp.Exceptions;
using JokeStorageFunctionApp.Services;
using Moq;

namespace JokeStorageFunctionApp.Tests;

public class BlobServiceTests
{
    private readonly Mock<BlobContainerClient> _blobContainerClientMock;
    private readonly Mock<BlobClient> _blobClientMock;
    private readonly BlobService _blobService;

    public BlobServiceTests()
    {
        _blobContainerClientMock = new Mock<BlobContainerClient>();
        _blobClientMock = new Mock<BlobClient>();

        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        _blobService = new BlobService(_blobContainerClientMock.Object);
    }

    [Fact]
    public async Task SavePayloadAsync_ShouldUploadBlob()
    {
        // Arrange
        var id = "test-id";
        var content = "{\"type\":\"general\",\"setup\":\"Why are 'Dad Jokes' so good?\",\"punchline\":\"Because the punchline is apparent.\"}";

        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        var blobContentInfo = BlobsModelFactory.BlobContentInfo(
            eTag: new ETag("etag"),
            lastModified: DateTimeOffset.UtcNow,
            contentHash: null,
            encryptionKeySha256: null,
            encryptionScope: null,
            blobSequenceNumber: 0);

        _blobClientMock.Setup(x => x.UploadAsync(It.IsAny<BinaryData>()))
            .ReturnsAsync(Response.FromValue(blobContentInfo, Mock.Of<Response>()));

        // Act
        await _blobService.SavePayloadAsync(id, content);

        // Assert
        _blobClientMock.Verify(x => x.UploadAsync(It.IsAny<BinaryData>()), Times.Once);
    }


    [Fact]
    public async Task GetPayloadAsync_ShouldThrowBlobNotFoundException_WhenBlobDoesNotExist()
    {
        // Arrange
        var id = "non-existing-id";

        _blobClientMock.Setup(x => x.ExistsAsync(It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BlobNotFoundException>(() => _blobService.GetPayloadAsync(id));

        Assert.Equal($"Blob with id {id} not found.", exception.Message);
    }
}