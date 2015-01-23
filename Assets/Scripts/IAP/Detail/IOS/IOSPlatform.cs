using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using IAP;

namespace IAP.Detail
{
	public class IOSPlatform : IAPPlatformBase
	{
		private List<IAPProduct> products = new List<IAPProduct>();
		private IIOSPurchaseInfo lastTransactionData;
		private IIAPIOSConnector connector;

		public override void ValidatePedingPurchases (){}

		public override bool CanMakePayments
		{
			get { return connector.CanMakePayments; }
		}
		
		public override List<IAPProduct> Products
		{
			get { return products; }
		}
		
		public override IAPPlatformID PlatformId
		{
			get { return IAPPlatformID.StoreKit; }
		}

		public IOSPlatform(List<IIAPProductData> products, float timeOutToStore, IIAPIOSConnector connector): base(products, timeOutToStore)
		{
			this.connector = connector;
			RegisterCallbacks();
		}

		public override void ConsumeProduct(string brainzProductId){}

		public override void PurchaseProduct (string brainzProductId, int quantity)
		{
			string iapProductId = BrainzProductIdToIAPProductId (brainzProductId);
			connector.PurchaseProduct(iapProductId, quantity);
		}
		
		public override void Dispose()
		{
			products.Clear();
			UnregisterCallbacks();
		}
		
		public override Hashtable GetLastTransactionData()
		{
			Hashtable transactionData = new Hashtable ();
			transactionData.Add("productIdentifier", lastTransactionData.ProductId);
			transactionData.Add("transactionIdentifier", lastTransactionData.TransactionId);
			transactionData.Add("base64EncodedTransactionReceipt", lastTransactionData.Base64EncodedTransactionReceipt);
			transactionData.Add("quantity", lastTransactionData.Quantity);
			transactionData.Add("price", "");
			transactionData.Add("currencyCode", "");
			
			IAPProduct product = products.Find(p => BrainzProductIdToIAPProductId(p.brainzProductId) == lastTransactionData.ProductId);
			
			if (product != null)
			{
				string price = GetPriceByBrainzIAPProductId (product.brainzProductId);
				transactionData["price"] = price;
				transactionData["currencyCode"] = product.currencyCode;
			}
			
			return transactionData;
		}

		protected override void GetProductsDataFromStore ()
		{
			products.Clear();
			connector.GetProducts(GetAllIAPProductId());
		}

		private void RegisterCallbacks()
		{
			connector.ProductListReceivedDelegate += OnIAPProductListReceived;
			connector.ProductListRequestFailedDelegate += OnIAPProductListRequestFailed;
			connector.PurchaseCancelledDelegate += OnIAPPurchaseCancelled;
			connector.PurchaseFailedDelegate += OnIAPPurchaseFailed;
			connector.PurchaseSucceededDelegate += OnIAPPurchaseSuccedded;
		}
		
		private void UnregisterCallbacks()
		{
			connector.ProductListReceivedDelegate -= OnIAPProductListReceived;
			connector.ProductListRequestFailedDelegate -= OnIAPProductListRequestFailed;
			connector.PurchaseCancelledDelegate -= OnIAPPurchaseCancelled;
			connector.PurchaseFailedDelegate -= OnIAPPurchaseFailed;
			connector.PurchaseSucceededDelegate -= OnIAPPurchaseSuccedded;
		}

		private void OnIAPProductListReceived(List<IIOSProductInfo> productInfoList)
		{
			products.Clear();
			FillProducts (productInfoList);
	        products.Sort(delegate(IAPProduct p1, IAPProduct p2) { return p1.brainzProductId.CompareTo(p2.brainzProductId); });
			TurnOffTryToLoadProductsFlag ();
			OnProductListReceived(PlatformId);
		}

		private void FillProducts (List<IIOSProductInfo> productList)
		{
			foreach (IIOSProductInfo productInfo in productList) 
			{
				string brainzProductId = IAPProductIDToBrainzProductId (productInfo.ProductId);
				if (!string.IsNullOrEmpty(brainzProductId)) 
					AddIAPProduct (productInfo, brainzProductId);
				else 
					Debug.LogWarning ("An unrecognized IAP product was reported by StoreKit. Identifier=" + productInfo.ProductId);
			}
		}

		private void AddIAPProduct (IIOSProductInfo productInfo, string brainzProductId)
		{
			if (productInfo.CurrencySymbol != null && productInfo.Description != null && productInfo.Price != null && productInfo.Title != null)
			{
				IAPProduct product = CreateIAPProduct (productInfo, brainzProductId);
				products.Add (product);
				CurrencyCode = product.currencyCode;
				SetCurrencyPrice (productInfo.ProductId, productInfo.FormattedPrice);
				Debug.Log ("Loaded product: " + product.ToString ());
			}
			else 
				Debug.LogWarning ("IAP product ignored because it contains null data: " + brainzProductId);
		}

		private IAPProduct CreateIAPProduct (IIOSProductInfo productInfo, string brainzProductId)
		{
			IAPProduct product = new IAPProduct ();
			product.currencySymbol = productInfo.CurrencySymbol;
			product.currencyCode = productInfo.CurrencyCode;
			product.description = productInfo.Description;
			product.price = productInfo.Price;
			product.formattedPrice = productInfo.FormattedPrice;
			product.brainzProductId = brainzProductId;
			product.title = productInfo.Title;
			return product;
		}
		
		private void OnIAPProductListRequestFailed(string error)
		{
			TurnOffTryToLoadProductsFlag ();
			OnProductListRequestFailed(PlatformId, error);
		}
		
		private void OnIAPPurchaseCancelled(string error)
		{
			OnPurchaseCancelled (PlatformId, error);
		}
		
		private void OnIAPPurchaseFailed(string error)
		{
			OnPurchaseFailed (PlatformId, error);
		}
		
		private void OnIAPPurchaseSuccedded(IIOSPurchaseInfo data)
		{
			string brainzProductId = IAPProductIDToBrainzProductId(data.ProductId);
			if (!string.IsNullOrEmpty(brainzProductId))
			{
				lastTransactionData = data;
				OnPurchaseSuccessful(brainzProductId, data.Quantity, PlatformId, CreateHashtableForPurchaseSuccedded (data, brainzProductId));
			}
			else
				Debug.LogWarning("An unrecognized IAP product was reported by StoreKit. Identifier=" + data.ProductId);
		}

		static Hashtable CreateHashtableForPurchaseSuccedded (IIOSPurchaseInfo data, string brainzProductId)
		{
			Hashtable table = new Hashtable ();
			table.Add ("receipt", data.Base64EncodedTransactionReceipt);
			table.Add ("sandbox", "!IsLiveEnvironment");//IsLiveEnvironment
			table.Add ("pack", brainzProductId);
			return table;
		}

		private string GetCurrencyCode (string productIdentifier)
		{
			return "";
		}
	}
}