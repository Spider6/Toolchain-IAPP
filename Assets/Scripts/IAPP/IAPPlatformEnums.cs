public enum IAPPlatformID
{
	Dummy = 0,
	StoreKit = 1,
	GoogleIAB = 2
}

public class IAPProductInfo
{
	public IAPProductID ID {get; private set;}
	public string Price {get; private set;}
	public string CurrencyPrice {get; set;}
	public IAPProductInfo (IAPProductID id, string price)
	{
		this.ID = id;
		this.Price = price;
	}
	
}
