using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace IAP
{
	public interface IGooglePurchaseInfo
	{
		string OrderId {get;}
		string ProductId {get;}
		string PurchaseToken {get;}
		long PurchaseTime {get;}
		string Signature {get;}
		string OriginalJson {get;}
		string PurchaseState {get;}
		string DeveloperPayload {get;}
	}

	public interface IGoogleProductInfo
	{
		string Title {get;}
		string Price {get;}
		string Description {get;}
		string ProductId {get;}
		string PriceCurrencyCode {get;}
	}

	public interface IIAPGoogleConnector
	{
		event Action BillingSupportedDelegate;
		event Action<string> BillingNotSupportedDelegate;
		event Action<List<IGooglePurchaseInfo>, List<IGoogleProductInfo>> ProductListReceivedDelegate;
		event Action<string> ProductListRequestFailedDelegate;
		event Action<IGooglePurchaseInfo> PurchaseSucceededDelegate;
		event Action<string> PurchaseFailedDelegate;

		void Initialize(string publicKey);
		void PurchaseProduct(string iapProductId, string developerPayload);
		void ConsumeProduct(string iapProductId);
		void GetProducts(string[] iapProductIds);
	}
}