namespace Maintenance.Domain.Interfaces
{
  public interface IAuditService
  {
    T CreateEntity<T>(T entity);
    T UpdateEntity<T>(T entity);
    T DeleteEntity<T>(T entity);
    string UserName { get; }
    string UserId { get; }
    string UserLanguage { get;  }
    string WebToken { get;  }
    }
}