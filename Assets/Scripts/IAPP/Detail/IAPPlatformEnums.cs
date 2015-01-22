public enum IAPPlatformID
{
	Dummy = 0,
	StoreKit = 1,
	GoogleIAB = 2
}

public class IAPProductInfo
{
	public string BrainzProductId {get; private set;}
	public string Price {get; private set;}
	public string CurrencyPrice {get; set;}
	public IAPProductInfo (string brainzProductId, string price)
	{
		this.BrainzProductId = brainzProductId;
		this.Price = price;
	}
	
}
