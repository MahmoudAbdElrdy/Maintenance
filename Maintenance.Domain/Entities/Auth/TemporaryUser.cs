namespace AuthDomain.Entities.Auth
{
    public class TemporaryUser
    {
        public int Id { get; set; }
        public long NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
