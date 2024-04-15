
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using APIClients.Domain.Shared;

namespace APIClients.Domain.Clients
{
    public class Address : ValueObject
    {
        public String Street { get; private set; }
        public String Local { get; private set; }
        public String PostalCode { get; private set; }
        public RegionInfo Country { get; private set; }

        private Address()
        {
            
        }

        public Address(string street, string local, string postalCode, string country)
        {
            if(string.IsNullOrEmpty(street))
                throw new BusinessRuleValidationException("Null or empty Street");
            this.Street = street;
            
            if(string.IsNullOrEmpty(local))
                throw new BusinessRuleValidationException("Null or empty Local");
            this.Local = local;
            
            if (IsValidPostalCode(postalCode))
            {
                this.PostalCode = postalCode;
            }
            else
            {
                throw new BusinessRuleValidationException("Invalid Postal Code");
            }
            
            this.Country = new RegionInfo(country);;
        }

        private bool IsValidPostalCode(String postalCode)
        {
            return (!string.IsNullOrEmpty(postalCode) && Regex.IsMatch(postalCode, "([0-9]{4})-([0-9]{3})"));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Street;
            yield return this.Local;
            yield return this.PostalCode;
            yield return this.Country;
        }
    }
}