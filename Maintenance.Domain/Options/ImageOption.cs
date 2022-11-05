namespace Common.Options {
  public class ImageOption {
    public bool SaveInDatabse { get; set; }
    public string AllowedExt { get; set; }
    public int AllowedSizeInKB { get; set; }
    public string CachedImageDir { get; set; }
  }
}