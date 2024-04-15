using System;
using System.Text.RegularExpressions;
using APIAuthors.Domain.Shared;
using Newtonsoft.Json;

namespace APIAuthors.Domain.Books
{
    public class BookId : EntityId
    {

        [JsonConstructor]
        public BookId(String value) : base(IsValidBookId(value))
        {
        }

        override
        protected  Object createFromString(String text){
            return text;
        }

        override
        public String AsString(){
            var obj = (string) base.ObjValue;
            return obj;
        }
        
        
        //This is most definitely a hack...
        private static string IsValidBookId(string isbn)
        {
            if (string.IsNullOrEmpty(isbn) || !Regex.IsMatch(isbn, @"^97[8-9]{1}-[0-9]{10}$"))
            {
                throw new BusinessRuleValidationException("Invalid ISBN");
            }

            return isbn;
        }
    }
}