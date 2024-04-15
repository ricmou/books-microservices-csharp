namespace APIExemplar.Domain.Exemplars;

public class CreatingExemplarDto
{
    public string BookId { get; set; }
    
    public int BookState { get; set; }
    
    public string SellerId { get; set; }
    
    public string DateOfAcquisition { get; set; }

    public CreatingExemplarDto(string bookId, int bookState, string sellerId, string dateOfAcquisition)
    {
        BookId = bookId;
        BookState = bookState;
        SellerId = sellerId;
        DateOfAcquisition = dateOfAcquisition;
    }
}