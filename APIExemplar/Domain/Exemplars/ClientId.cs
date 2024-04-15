using System;
using APIExemplar.Domain.Shared;
using Newtonsoft.Json;

namespace APIExemplar.Domain.Exemplars;

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