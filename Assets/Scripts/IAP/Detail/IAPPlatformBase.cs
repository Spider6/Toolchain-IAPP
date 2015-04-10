using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IAP;

namespace IAP.Detail
{
	public abstract class IAPPlatformBase : IIAPPlatform
	{
		public event Action<IAPPlatformID> ProductListReceivedDelegate;
		public event Action<IAPPlatformID, string /*error*/> ProductListRequestFailedDelegate;
		public event Action<string/*brainzProductId*/, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessfulDelegate;
		public event Action<IAPPlatformID, string /*error*/> PurchaseFailedDelegate;
		public event Action<IAPPlatformID, string /*error*/> PurchaseCancelledDelegate;
		public event Action BillingSupportedDelegate;
		public event Action<string/*error*/> BillingNotSupportedDelegate;

		private float timeOutToStore;

		public abstract bool CanMakePayments { get; }
		public abstract List<IAPProduct> Products { get; }
		public abstract IAPPlatformID PlatformId { get; }

		public bool HasProducts 
		{ 
			get { return Products.Count > 0; } 
		}

		public string CurrencyCode 
		{ 
			get; 
			protected set; 
		}

		protected MonoBehaviour caller;
		private bool isTryToLoadProducts = false;
		private Dictionary<string, IAPProductInfo> allProducts;

		public IAPPlatformBase(List<IIAPProductData> products, float timeOutToStore)
		{
			this.timeOutToStore = timeOutToStore;
			CreateProductInfo(products);
		}

		public abstract void ValidatePedingPurchases ();
		public abstract void PurchaseProduct(string brainzProductId, int quantity);
		public abstract void ConsumeProduct(string brainzProductId);
		public abstract void Dispose();
		public abstract Hashtable GetLastTransactionData();
		protected abstract void GetProductsDataFromStore();

		public string GetBrainzProductIdByIAPProductId (string productID)
		{
			return IAPProductIDToBrainzProductId (productID);
		}

		public virtual void RequestAllProductData(MonoBehaviour caller)
		{
			if(!isTryToLoadProducts)
			{
				isTryToLoadProducts = true;
				this.caller = caller;
				this.caller.StartCoroutine (CheckProductsTimeOut ());
				GetProductsDataFromStore();
			}
		}

		public string GetCurrencyPrice (string brainzProductId)
		{
			string productId = BrainzProductIdToIAPProductId (brainzProductId);
			IAPProductInfo product = allProducts[productId];
			
			if(product != null)
				return product.CurrencyPrice;
			
			return string.Empty;
		}

		public string GetPriceStringByBrainzProductId (string brainzProductID)
		{
			foreach(KeyValuePair<string, IAPProductInfo> entry in allProducts)
			{
				IAPProductInfo info = entry.Value as IAPProductInfo;
				if(info.BrainzProductId == brainzProductID)
					return info.Price;
			}
			return "0";
		}

		protected void SetCurrencyPrice (string productId, string price)
		{
			if(allProducts.ContainsKey(productId))
				allProducts[productId].CurrencyPrice = price;
		}
		
		protected string GetPriceByBrainzIAPProductId (string brainzProductID)
		{
			string priceValue = GetPriceStringByBrainzProductId (brainzProductID);
			string price = Regex.Replace (priceValue, "[^,'0-9.]", "");
			return price;
		}
		
		protected string BrainzProductIdToIAPProductId(string brainzProductId)
		{
			foreach(KeyValuePair<string, IAPProductInfo> entry in allProducts)
			{
				IAPProductInfo info = entry.Value as IAPProductInfo;
				if(info.BrainzProductId == brainzProductId)
					return entry.Key.ToString ();
			}
			return string.Empty;
		}
		
		protected string IAPProductIDToBrainzProductId (string productId)
		{
			if(allProducts.ContainsKey(productId))
				return allProducts [productId].BrainzProductId;
			
			return string.Empty;
		}
		
		protected void OnProductListReceived(IAPPlatformID platformId)
		{
			if(ProductListReceivedDelegate != null)
				ProductListReceivedDelegate(platformId);
		}
		
		protected void OnProductListRequestFailed(IAPPlatformID platformId, string error)
		{
			Debug.Log("Request Filed: " + error);
			if(ProductListRequestFailedDelegate != null)
				ProductListRequestFailedDelegate(platformId, error);
		}
		
		protected void OnPurchaseSuccessful(string brainzProductId, int quantity, IAPPlatformID platformId, Hashtable transactionData)
		{
			if(PurchaseSuccessfulDelegate != null)
				PurchaseSuccessfulDelegate(brainzProductId, quantity, platformId, transactionData);
		}
		
		protected void OnPurchaseFailed(IAPPlatformID platformId, string error)
		{
			if(PurchaseFailedDelegate != null)
				PurchaseFailedDelegate(platformId, error);
		}
		
		protected void OnPurchaseCancelled(IAPPlatformID platformId, string error)
		{
			if(PurchaseCancelledDelegate != null)
				PurchaseCancelledDelegate(platformId, error);
		}

		protected virtual void OnBillingSupported()
		{
			if(BillingSupportedDelegate != null)
				BillingSupportedDelegate();
		}

		protected virtual void OnBillingNotSupported(string error)
		{
			if(BillingNotSupportedDelegate != null)
				BillingNotSupportedDelegate(error);
		}

		protected void TurnOffTryToLoadProductsFlag ()
		{
			isTryToLoadProducts = false;
			if(caller != null)
				this.caller.StopCoroutine (CheckProductsTimeOut ());
		}

		protected string[] GetAllIAPProductId()
		{
			int index = 0;
			string[] ids = new string[allProducts.Keys.Count];
			foreach( KeyValuePair<string, IAPProductInfo> entry in allProducts)
			{
				Debug.Log("ProductId: " + entry.Key);
				ids[index] = entry.Key;
				index++;
			}
			return ids;
		}

		private IEnumerator CheckProductsTimeOut ()
		{
			yield return new WaitForSeconds(timeOutToStore);
			if(!HasProducts)
				OnProductListRequestFailed (PlatformId, "Time Out");
			this.caller.StopCoroutine (CheckProductsTimeOut ());
			TurnOffTryToLoadProductsFlag ();
		}

		private void CreateProductInfo (List<IIAPProductData> products)
		{
			allProducts = new Dictionary<string, IAPProductInfo> ();
			foreach(IIAPProductData product in products)
				allProducts.Add (product.IAPProductId, new IAPProductInfo( product.BrainzProductId , "0.00"));
		}

		private float GetValueFromPriceString (string priceCurrency)
		{
			Regex numberRegex = new Regex (@"(([0-9]\.*)*[0-9])");
			Match numberMatch = numberRegex.Match (priceCurrency);
			return float.Parse (numberMatch.Value);
		}
	}
}