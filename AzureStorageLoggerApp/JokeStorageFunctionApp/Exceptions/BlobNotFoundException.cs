using System;

namespace JokeStorageFunctionApp.Exceptions;

public class BlobNotFoundException : Exception
{
    public BlobNotFoundException(string message) : base(message) { }
}
