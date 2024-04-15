using System;
using System.Collections.Generic;
using System.Globalization;
using APIAuthors.Domain.Books;
using APIAuthors.Domain.Shared;

namespace APIAuthors.Domain.Authors;

public class Author : Entity<AuthorId>, IAggregateRoot
{
    public AuthorName Name { get; private set; }
    public DateOnly BirthDate { get; private set; }

    public RegionInfo Country { get; private set; }

    public List<Book> Books { get; private set; }

    private Author()
    {
    }

    public Author(string id, string firstName, string lastName, DateOnly birthDate, string countryCode)
    {
        this.Id = new AuthorId(id);
        this.Name = new AuthorName(firstName, lastName);
        this.BirthDate = birthDate;
        this.ChangeCountry(countryCode);
        this.Books = new List<Book>();
    }

    public void ChangeName(AuthorName name)
    {
        this.Name = name ?? throw new BusinessRuleValidationException("Invalid Author Name.");
    }
    public void ChangeBirthDate(DateOnly birthDate)
    {
        this.BirthDate = birthDate;
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
    
    public override string ToString()
    {
        return String.Format("{0}, {1}, {2}, {3}, {4}",Id.AsString(), Name.FirstName, Name.LastName, BirthDate.ToString(), Country.Name);
    }
}