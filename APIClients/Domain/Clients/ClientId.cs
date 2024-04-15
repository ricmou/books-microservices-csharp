using System;
using APIClients.Domain.Shared;
using Newtonsoft.Json;

namespace APIClients.Domain.Clients;

public class ClientId : EntityId
{
    [JsonConstructor]
    public ClientId(Guid value) : base(value)
    {
    }

    public ClientId(String value) : base(value)
    {
    }
    
    protected override Object createFromString(String text)
    {
        return new Guid(text);
    }
    
    public override String AsString()
    {
        Guid obj = (Guid)base.ObjValue;
        return obj.ToString();
    }

    public Guid AsGuid()
    {
        return (Guid)base.ObjValue;
    }
}