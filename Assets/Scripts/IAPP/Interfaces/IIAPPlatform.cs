using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class IAPProduct
{
	public string brainzProductId;
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
	void PurchaseProduct (string id, int i);
	void ConsumeProduct (string id);
	List<IAPProduct> Products{get;}
	event Action<IAPPlatformID> ProductListReceivedDelegate;
	event Action<IAPPlatformID, string /*error*/> ProductListRequestFailedDelegate;
	event Action<string /*brainzProductId*/, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessfulDelegate;
	event Action<IAPPlatformID, string /*error*/> PurchaseFailedDelegate;
	event Action<IAPPlatformID, string /*error*/> PurchaseCancelledDelegate;
	void RequestAllProductData(MonoBehaviour caller);
	string GetCurrencyPrice (string brainzProductId);
	string GetPriceWithoutDiscount (string brainzProductId, float discountPercent);
	float GetPriceInFloat (string brainzProductId);
	void ValidatePedingPurchases ();
	Hashtable GetLastTransactionData();
	string GetBrainzProductIdByIAPProductId (string productID);
	string GetPriceStringByBrainzProductId (string brainzProductId);
}