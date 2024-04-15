namespace APIPublisher.Domain.Publishers;

public class PublisherDto
{
    public string PublisherId { get; set; }
    
    public string Name { get; set; }
    
    public string Country { get; set; }

    public PublisherDto(string publisherId, string name, string country)
    {
        PublisherId = publisherId;
        Name = name;
        Country = country;
    }
}