namespace RadiantConnect.Services
{
    public record UserAuth(string LockFile, int AuthorizationPort, string OAuth)
    {
        public string LockFile { get; set; } = LockFile;
        public int AuthorizationPort { get; set; } = AuthorizationPort;
        public string OAuth { get; set; } = OAuth;
    }
}
