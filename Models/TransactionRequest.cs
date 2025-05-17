using System.Diagnostics.CodeAnalysis;

namespace TransactionApi.Models;

public class TransactionRequest
{
    public string partnerkey { get; set; }
    public string partnerrefno { get; set; }
    public string partnerpassword { get; set; }
    public long totalamount { get; set; }
    public List<ItemDetail>? items { get; set; }
    public string timestamp { get; set; }
    public string sig { get; set; }
}

public class ItemDetail
{
    public string partneritemref { get; set; }
    public string name { get; set; }
    public int qty { get; set; }
    public long unitprice { get; set; }
}