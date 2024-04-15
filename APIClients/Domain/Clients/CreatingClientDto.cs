namespace APIClients.Domain.Clients;

public class CreatingClientDto
{
    public string Name { get; set; }
    public string Street { get; set; }
    public string Local { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public CreatingClientDto(string name, string street, string local, string postalCode, string country)
    {
        Name = name;
        Street = street;
        Local = local;
        PostalCode = postalCode;
        Country = country;
    }
}