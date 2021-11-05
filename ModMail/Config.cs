namespace ModMail;

public struct Config
{
    public string Token { get; init; }
    public string Prefix { get; init; }
    public ulong[] Owners { get; init; }
}