namespace ElisBackend.Gateways.Repositories.Daos {

    public class DateDao(DateTime dateTimeUtc) {
        public int Id { get; set; }
        public DateTime DateTimeUtc { get; set; } = dateTimeUtc;
    }
}
