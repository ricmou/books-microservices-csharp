using System;
using System.Collections.Generic;
using System.Linq;
using APIExemplar.Domain.Shared;

namespace APIExemplar.Domain.Exemplars;

public class ExemplarState : ValueObject
{
    public static readonly String[] StateList =
    {
        "Unknown",
        "Poor",
        "Well-Worn",
        "Good",
        "As New"
    };
    
    public int State { get; private set; }
    
    public ExemplarState()
    {
    }

    public ExemplarState(int state)
    {
        if (state >= 0 && state < StateList.Length)
            this.State = state;
        else
            throw new BusinessRuleValidationException("Invalid State.");
    }
    
    public ExemplarState(string state)
    {
        if(string.IsNullOrEmpty(state))
            throw new BusinessRuleValidationException("Null or empty State.");
        
        var intState = StateFromString(state);
        
        if (intState != -1)
        {
            this.State = intState;
        }
        else
        {
            throw new BusinessRuleValidationException("Invalid State.");
        }
    }
    
    private static int StateFromString(string role)
    {
        return StateList.ToList().IndexOf(role);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.State;
    }
}