using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum IAPProductID 
{
	PouchOfJade = 0,
	HeapOfJade = 1,
	ChestOfJade  = 2,
	CartOfJade = 3,
	VaultOfJade = 4,
	None = 10,
}

[System.Serializable]
public class IAPProduct
{
	public IAPProductID brainzProductId;
	public string title;
	public string description;
	public string price;
	public string formattedPrice;
	public string currencySymbol;
	public string currencyCode;
	
	public override string ToString()
	{	
		return System.String.Format( "<Product>\nID: {0}\nTitle: {1}\nDescription: {2}\nPrice: {3}\nPrice Formatted: {4}\nCurrency Symbol: {5}\nCurrency Code: {6}",
		                            brainzProductId, title, description, price, formattedPrice, currencySymbol, currencyCode );
	}
}

public interface IIAPPlatform 
{
	void Dispose ();
	bool CanMakePayments  { get; }
	void PurchaseProduct (IAPProductID id, int i);
	void ConsumeProduct (IAPProductID id);
	List<IAPProduct> Products{get;}
	event Action<IAPPlatformID> ProductListReceivedDelegate;
	event Action<IAPPlatformID, string /*error*/> ProductListRequestFailedDelegate;
	event Action<IAPProductID /*id*/, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessfulDelegate;
	event Action<IAPPlatformID, string /*error*/> PurchaseFailedDelegate;
	event Action<IAPPlatformID, string /*error*/> PurchaseCancelledDelegate;
	void RequestAllProductData(MonoBehaviour caller);
	string GetCurrencyPrice (IAPProductID productId);
	string GetPriceWithoutDiscount (IAPProductID product, float discountPercent);
	float GetPriceInFloat (IAPProductID productID);
	void ValidatePedingPurchases ();
	string StoreName { get; }
	Hashtable GetLastTransactionData();
	IAPProductID GetBrainzProductIdByIAPProductId (string productID);
	string GetPriceStringByBrainzProductId (IAPProductID productID);
}