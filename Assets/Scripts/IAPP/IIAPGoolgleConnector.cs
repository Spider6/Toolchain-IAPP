using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IGooglePurchase
{
	string orderId {get;}
	string productId {get;}
	string purchaseToken {get;}
	long purchaseTime {get;}
	string signature {get;}
	string originalJson {get;}
	string purchaseState {get;}
}

public interface IGoogleSkuInfo
{
	string title {get;}
	string price {get;}
	string description {get;}
	string productId {get;}
	string priceCurrencyCode {get;}
}

public interface IIAPGoolgleConnector
{
	event Action BillingSupportedDelegate;
	event Action<string> BillingNotSupportedDelegate;
	event Action<List<GooglePurchase>, List<GoogleSkuInfo>> ProductListReceivedDelegate;
	event Action<string> ProductListRequestFailedDelegate;
	event Action<GooglePurchase> PurchaseSucceededDelegate;
	event Action<string> PurchaseFailedDelegate;

	void Initialize(string publicKey);
	void PurchaseProduct(string iapProductId, string developerPayload);
	void ConsumeProduct(string iapProductId);
	void GetProducts(string[] iapProductIds);
}
