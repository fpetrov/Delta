namespace Delta.Core.Services.Security;

public interface IHashService
{
    public bool Verify(string content, string hash);
    public string Hash(string content);
}