public enum IAPPlatformID
{
	Dummy = 0,
	StoreKit = 1,
	GoogleIAB = 2
}

public class IAPProductInfo
{
	public IAPProductID BrainzProductId {get; private set;}
	public string Price {get; private set;}
	public string CurrencyPrice {get; set;}
	public IAPProductInfo (IAPProductID brainzProductId, string price)
	{
		this.BrainzProductId = brainzProductId;
		this.Price = price;
	}
	
}
