namespace Common.Options {
  public class AppInfoOption {
    public string Name { get; set; }

    public string Issuer { get; set; }
    public string TokenLifetime { get; set; }
    public int ExpireDays { get; set; }

  }
}