using APIClients.Domain.Clients;
using APIClients.Domain.Shared;

namespace APIClientsTest.Domain.Clients
{
    public class AddressTest
    {
        [Fact]
        public void CheckAddressNotAcceptNullStreet()
        {
            Assert.Throws<BusinessRuleValidationException>(() => new Address(null, "Local", "1111-111", "Country"));
        }
        
        [Fact]
        public void CheckAddressNotAcceptEmptyStreet()
        {
            Assert.Throws<BusinessRuleValidationException>(() => new Address("", "Local", "1111-111", "Country"));
        }
        
        [Fact]
        public void CheckAddressNotAcceptNullLocal()
        {
            Assert.Throws<BusinessRuleValidationException>(() => new Address("Street", null, "1111-111", "Country"));
        }
        
        [Fact]
        public void CheckAddressNotAcceptEmptyLocal()
        {
            Assert.Throws<BusinessRuleValidationException>(() => new Address("Street", "", "1111-111", "Country"));
        }
        
        [Fact]
        public void CheckAddressNotAcceptNullPostalCode()
        {
            Assert.Throws<BusinessRuleValidationException>(() => new Address("Street", "Local", null, "Country"));
        }
        
        [Fact]
        public void CheckAddressNotAcceptEmptyPostalCode()
        {
            Assert.Throws<BusinessRuleValidationException>(() => new Address("Street", "Local", "", "Country"));
        }
        
        [Fact]
        public void CheckAddressNotAcceptInvalidPostalCode()
        {
            Assert.Throws<BusinessRuleValidationException>(() => new Address("Street", "Local", "A111-111", "Country"));
        }

        [Fact]
        public void CheckAddressNotAcceptNullCountry()
        {
            Assert.Throws<ArgumentNullException>(() => new Address("Street", "Local", "1111-111", null));
        }
        
        [Fact]
        public void CheckAddressNotAcceptEmptyCountry()
        {
            Assert.Throws<ArgumentException>(() => new Address("Street", "Local", "1111-111", ""));
        }
    }
}