namespace XieyiES.Api.Models
{
    public class Person
    {
        public long? Uid { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public Location Address { get; set; }
    }

    public class Location
    {
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
    }
}