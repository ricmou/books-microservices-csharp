using System;
using System.Text.RegularExpressions;
using APIAuthors.Domain.Shared;
using Newtonsoft.Json;

namespace APIAuthors.Domain.Authors
{
    public class AuthorId : EntityId
    {
        [JsonConstructor]
        public AuthorId(String value) : base(IsValidAuthorID(value))
        {
        }

        
            protected override Object createFromString(String text){
            return text;
        }

        
            public override String AsString(){
            var obj = (string) base.ObjValue;
            return obj;
        }
        
        
        //This is most definitely a hack...
        private static string IsValidAuthorID(string authorId)
        {
            if (string.IsNullOrEmpty(authorId) || !Regex.IsMatch(authorId, "[A-Za-z0-9]{3}"))
            {
                throw new BusinessRuleValidationException("Invalid Author ID");
            }

            return authorId;
        }
    }
}