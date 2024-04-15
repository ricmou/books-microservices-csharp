using System.Collections.Generic;
using APIAuthors.Domain.Shared;

namespace APIAuthors.Domain.Authors;

public class AuthorName : ValueObject
{
    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public AuthorName(string firstName, string lastName)
    {
        if (IsValidName(firstName))
        {
            this.FirstName = firstName;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Names must have at least a character and no more than 128");
        }
        
        if (IsValidName(lastName))
        {
            this.LastName = lastName;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Names must have at least a character and no more than 128");
        }
    }


    private bool IsValidName(string name)
    {
        return (!string.IsNullOrEmpty(name) && name.Length <= 128);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.FirstName;
        yield return this.LastName;
    }
}