using System;
using APIClients.Domain.Shared;

namespace APIClients.Domain.Clients;

public class Client : Entity<ClientId>, IAggregateRoot
{
    public string Name { get;  private set; }
    
    public Address Address { get; private set; }

    private Client()
    {
    }

    public Client(string name, Address address)
    {
        this.Id = new ClientId(Guid.NewGuid());
        
        if (IsValidName(name))
        {
            this.Name = name;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Names must have at least a character and no more than 150");
        }

        if (address == null)
        {
            throw new BusinessRuleValidationException(
                "Invalid Address");
        }
        else
        {
            Address = address;
        }
    }
    
    public Client(string id, string name, Address address)
    {
        this.Id = new ClientId(id);
        
        if (IsValidName(name))
        {
            this.Name = name;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Names must have at least a character and no more than 150");
        }

        if (address == null)
        {
            throw new BusinessRuleValidationException(
                "Invalid Address");
        }
        else
        {
            Address = address;
        }
    }
    
    private bool IsValidName(string name)
    {
        return (!string.IsNullOrEmpty(name) && name.Length <= 150);
    }

    public void ChangeName(string name)
    {
        if (IsValidName(name))
        {
            this.Name = name;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Names must have at least a character and no more than 150");
        }
    }

    public void ChangeAddress(Address address)
    {
        this.Address = address ?? throw new BusinessRuleValidationException("Invalid Address.");
    }
}