namespace APIExemplar.Domain.Exemplars;

public class ExemplarDto
{
    public string ExemplarId { get; set; }
    
    public string BookId { get; set; }
    
    public int BookState { get; set; }
    
    public string SellerId { get; set; }
    
    public string DateOfAcquisition { get; set; }

    public ExemplarDto(string exemplarId, string bookId, int bookState, string sellerId, string dateOfAcquisition)
    {
        ExemplarId = exemplarId;
        BookId = bookId;
        BookState = bookState;
        SellerId = sellerId;
        DateOfAcquisition = dateOfAcquisition;
    }
}