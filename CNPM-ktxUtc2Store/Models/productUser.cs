namespace CNPM_ktxUtc2Store.Models
{
    public class productUser
    {
        public string applicationUserId { get; set; }
        public virtual applicationUser applicationUser { get; set; }
        public int productId { get; set; }
        public virtual product Product { get; set;}
    }
}
