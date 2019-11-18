namespace Storage.Core
{
    public interface IFileItem
    {
        public string Key { get; }
        public string Owner { get; }
        public long Size { get; }
    }
}
