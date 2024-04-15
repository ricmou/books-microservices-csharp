namespace APIClients.Domain.Clients;

public class ClientDto
{
    public string ClientId { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string Local { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    

    public ClientDto(string clientId, string name, string street, string local, string postalCode, string country)
    {
        ClientId = clientId;
        Name = name;
        Street = street;
        Local = local;
        PostalCode = postalCode;
        Country = country;
    }
}