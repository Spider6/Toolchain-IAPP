using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace IAP
{
	public interface IIOSPurchaseInfo
	{
		string ProductId {get;}
		string TransactionId {get;}
		string Base64EncodedTransactionReceipt {get;}
		int Quantity {get;}
	}

	public interface IIOSProductInfo
	{
		string ProductId {get;}
		string Title {get;}
		string Description {get;}
		string Price {get;}
		string CurrencySymbol {get;}
		string CurrencyCode {get;}
		string FormattedPrice {get;}
	}

	public interface IIAPIOSConnector
	{
		event Action<List<IIOSProductInfo>> ProductListReceivedDelegate;
		event Action<string> ProductListRequestFailedDelegate;
		event Action<IIOSPurchaseInfo> PurchaseSucceededDelegate;
		event Action<string> PurchaseFailedDelegate;
		event Action<string> PurchaseCancelledDelegate;

		bool CanMakePayments{ get; }
		
		void PurchaseProduct(string iapProductId, int quantity);
		void GetProducts(string[] iapProductIds);
	}
}