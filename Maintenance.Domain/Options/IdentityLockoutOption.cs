namespace Common.Options {
  public class IdentityLockoutOption {
    public int DefaultLockoutTimeSpan { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public bool AllowedForNewUsers { get; set; }
    
  }
}