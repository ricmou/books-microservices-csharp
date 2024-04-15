using System;
using System.Text.RegularExpressions;
using APICategories.Domain.Shared;
using Newtonsoft.Json;

namespace APICategories.Domain.Categories;

public class CategoryId : EntityId
{
    [JsonConstructor]
    public CategoryId(String value) : base(IsValidCategoryID(value))
    {
    }

    override
        protected Object createFromString(String text)
    {
        return text;
    }

    override
        public String AsString()
    {
        var obj = (string)base.ObjValue;
        return obj;
    }


    //This is most definitely a hack...
    private static string IsValidCategoryID(string bookId)
    {
        if (string.IsNullOrEmpty(bookId) || !Regex.IsMatch(bookId, "[A-Za-z0-9]{3}"))
        {
            throw new BusinessRuleValidationException("Invalid Category ID");
        }

        return bookId;
    }
}