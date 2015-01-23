using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace IAP
{
	public interface IIAPManager  
	{
		IIAPPlatform IAPPlatform {get;}
		bool AreProductsLoaded {get;}
		bool PurchaseProduct(string brainzProductId);
		void ConsumeProduct(string brainzProductId);
	}

	public interface IIAPProductData
	{
		string BrainzProductId { get; }
		string IAPProductId { get; }
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
		event Action BillingSupportedDelegate;
		event Action<string/*error*/> BillingNotSupportedDelegate;
		void RequestAllProductData(MonoBehaviour caller);
		string GetCurrencyPrice (string brainzProductId);
		void ValidatePedingPurchases ();
		Hashtable GetLastTransactionData();
		string GetBrainzProductIdByIAPProductId (string productID);
		string GetPriceStringByBrainzProductId (string brainzProductId);
	}
}