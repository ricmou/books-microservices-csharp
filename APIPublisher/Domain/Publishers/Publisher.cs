using System.Collections.Generic;
using System.Globalization;
using APIPublisher.Domain.Books;
using APIPublisher.Domain.Shared;

namespace APIPublisher.Domain.Publishers;

public class Publisher : Entity<PublisherId>, IAggregateRoot
{
    public string Name { get; private set; }

    public RegionInfo Country { get; private set; }

    public List<Book> Books { get; private set; }

    private Publisher()
    {
    }

    public Publisher(string id, string name, string country)
    {
        Id = new PublisherId(id);
        Name = name;
        if (IsValidName(name))
        {
            Name = name;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Publisher Names must have at least a character and no more than 128");
        }

        Country = new RegionInfo(country);
        Books = new List<Book>();
    }

    private bool IsValidName(string name)
    {
        return (!string.IsNullOrEmpty(name) && name.Length <= 128);
    }

    public void ChangeName(string name)
    {
        if (IsValidName(name))
        {
            Name = name;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Publisher Names must have at least a character and no more than 128");
        }
    }

    public void ChangeCountry(string country)
    {
        if (string.IsNullOrEmpty(country))
        {
            throw new BusinessRuleValidationException(
                "Invalid Country");
        }
        this.Country = new RegionInfo(country);
    }
}