using Models;

public class CartItem
{
    public int Id { get; set; } // Identificatore univoco del singolo elemento nel carrello
    public Guid CartId { get; set; } // Identificatore del carrello a cui appartiene l'elemento
    public int ProductId { get; set; } // Identificatore del prodotto associato all'elemento del carrello
    public int Quantity { get; set; } // Quantità del prodotto nel carrello

    // Proprietà di navigazione per accedere all'oggetto Product associato
    public Product Product { get; set; }
    public decimal Price { get; set; }
}