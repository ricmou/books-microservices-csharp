using System;
using APIExemplar.Domain.Shared;

namespace APIExemplar.Domain.Exemplars;

public class Exemplar : Entity<ExemplarId>, IAggregateRoot
{
    public BookId Book { get; private set; }

    public ExemplarState BookState { get; private set; }
    
    public ClientId SellerId { get; private set; }
    
    public DateOnly DateOfAcquisition { get; private set; }

    private Exemplar()
    {
    }

    public Exemplar(BookId book, ExemplarState bookState, ClientId sellerId, DateOnly dateOfAcquisition)
    {
        this.Id = new ExemplarId(Guid.NewGuid());
        this.Book = book ?? throw new BusinessRuleValidationException("Invalid Book ID.");
        this.BookState = bookState ?? throw new BusinessRuleValidationException("Invalid BookState.");
        this.SellerId = sellerId ?? throw new BusinessRuleValidationException("Invalid SellerId.");
        this.DateOfAcquisition = dateOfAcquisition;
    }
    
    public Exemplar(string exemplarId, BookId book, ExemplarState bookState, ClientId sellerId, DateOnly dateOfAcquisition)
    {
        this.Id = new ExemplarId(exemplarId);
        this.Book = book ?? throw new BusinessRuleValidationException("Invalid Book ID.");
        this.BookState = bookState ?? throw new BusinessRuleValidationException("Invalid BookState.");
        this.SellerId = sellerId ?? throw new BusinessRuleValidationException("Invalid SellerId.");
        this.DateOfAcquisition = dateOfAcquisition;
    }

    public void ChangeBook(BookId bookId)
    {
        this.Book = bookId ?? throw new BusinessRuleValidationException("Invalid Book ID.");
    }
    
    public void ChangeBookState(ExemplarState bookState)
    {
        this.BookState = bookState ?? throw new BusinessRuleValidationException("Invalid BookState.");
    }
    
    public void ChangeSellerId(ClientId sellerId)
    {
        this.SellerId = sellerId ?? throw new BusinessRuleValidationException("Invalid SellerId.");
    }
    
    public void ChangeDateOfAcquisition(DateOnly dateOfAcquisition)
    {
        this.DateOfAcquisition = dateOfAcquisition;
    }
}