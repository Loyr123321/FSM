using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models.Mobile
{
    public class ClientTypeMobile
    {
        public string? TypeId { get; set; }
        public string? TypeName { get; set; }
        public ClientTypeMobile(ClientType originalClientType)
        {
            this.TypeId = originalClientType.TypeId;
            this.TypeName = originalClientType.TypeName;
        }
    }

    public class ClientMobile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string ClientPhone { get; set; }
        public string ClientMail { get; set; }
        public string OrganizationName { get; set; }
        public AddressMobile Address { get; set; }
        public List<ClientContactMobile> Contacts { get; set; }
        public ClientTypeMobile ClientType { get; set; }
        //public string MyCompany { get; set; }
        public ClientMobile(Client originalClient)
        {
            this.Id = originalClient.Id;
            this.FirstName = originalClient.FirstName;
            this.LastName = originalClient.LastName;
            this.MiddleName = originalClient.MiddleName;
            this.ClientPhone = originalClient.ClientPhone;
            this.ClientMail = originalClient.ClientMail;
            this.OrganizationName = originalClient.OrganizationName;
            this.Address = new AddressMobile(originalClient.Address);

            var contacts = new List<ClientContactMobile>();
            if (originalClient.Contacts != null)
            {
                foreach (var contact in originalClient.Contacts)
                {
                    if (contact != null)
                        contacts.Add(new ClientContactMobile(contact));
                }
                this.Contacts = contacts;
            }
            else { this.Contacts = null; }
            

            this.ClientType = new ClientTypeMobile(originalClient.ClientType);
        }
    }

    public class ClientContactMobile
    {
        public string Id { get; set; }
        public string ClientContactName { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public bool IsMain { get; set; }
        public ClientContactMobile(ClientContact originalContact)
        {
            this.Id = originalContact.Id;
            this.ClientContactName = originalContact.ClientContactName;
            this.Phone = originalContact.Phone;
            this.Mail = originalContact.Mail;
            this.IsMain = originalContact.IsMain;
        }
    }
}
