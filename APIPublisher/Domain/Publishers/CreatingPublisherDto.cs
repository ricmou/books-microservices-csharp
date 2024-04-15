namespace APIPublisher.Domain.Publishers;

public class CreatingPublisherDto
{
    public string PublisherId { get; set; }

    public string Name { get; set; }

    public string Country { get; set; }

    public CreatingPublisherDto(string publisherId, string name, string country)
    {
        PublisherId = publisherId;
        Name = name;
        Country = country;
    }
}