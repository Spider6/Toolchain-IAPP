using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public abstract class IAPPlatformBase : IIAPPlatform
{
	public event Action<IAPPlatformID> ProductListReceivedDelegate;
	public event Action<IAPPlatformID, string /*error*/> ProductListRequestFailedDelegate;
	public event Action<IAPProductID/*brainzProductId*/, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessfulDelegate;
	public event Action<IAPPlatformID, string /*error*/> PurchaseFailedDelegate;
	public event Action<IAPPlatformID, string /*error*/> PurchaseCancelledDelegate;

	public abstract bool CanMakePayments { get; }
	public abstract List<IAPProduct> Products { get; }
	public abstract IAPPlatformID PlatformId { get; }
	public abstract string StoreName { get; }

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
	
	public IAPProductID GetBrainzProductIdByIAPProductId (string productID)
	{
		return IAPProductIDToBrainzProductId (productID);
	}

	public abstract void ValidatePedingPurchases ();
	public abstract void PurchaseProduct(IAPProductID brainzProductId, int quantity);
	public abstract void ConsumeProduct(IAPProductID brainzProductId);
	public abstract void Dispose();
	public abstract Hashtable GetLastTransactionData();
	protected abstract void GetProductsDataFromStore();

	public IAPPlatformBase(List<IAPProductData> products)
	{
		CreateProductInfo(products);
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

	public string GetCurrencyPrice (IAPProductID brainzProductId)
	{
		string productId = BrainzProductIdToIAPProductId (brainzProductId);
		IAPProductInfo product = allProducts[productId];
		
		if(product != null)
			return product.CurrencyPrice;
		
		return string.Empty;
	}

	public string GetPriceWithoutDiscount (IAPProductID product, float discountPercent)
	{
		float price = GetPriceInFloat (product);
		float originalValue = (price * 100f) / (100f - discountPercent);
		return CurrencyCode + originalValue.ToString ("0");
	}

	public float GetPriceInFloat (IAPProductID productID)
	{
		string currencyPrice = GetCurrencyPrice(productID);
		return GetValueFromPriceString (currencyPrice);
	}

	public string GetPriceStringByBrainzProductId (IAPProductID brainzProductID)
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
	
	protected string GetPriceByBrainzIAPProductId (IAPProductID brainzProductID)
	{
		string priceValue = GetPriceStringByBrainzProductId (brainzProductID);
		string price = Regex.Replace (priceValue, "[^,'0-9.]", "");
		return price;
	}
	
	protected string BrainzProductIdToIAPProductId(IAPProductID brainzProductId)
	{
		foreach(KeyValuePair<string, IAPProductInfo> entry in allProducts)
		{
			IAPProductInfo info = entry.Value as IAPProductInfo;
			if(info.BrainzProductId == brainzProductId)
				return entry.Key.ToString ();
		}
		return string.Empty;
	}
	
	protected IAPProductID IAPProductIDToBrainzProductId (string productId)
	{
		if(allProducts.ContainsKey(productId))
			return allProducts [productId].BrainzProductId;
		
		return IAPProductID.None;
	}
	
	protected void OnProductListReceived(IAPPlatformID platformId)
	{
		if(ProductListReceivedDelegate != null)
			ProductListReceivedDelegate(platformId);
	}
	
	protected void OnProductListRequestFailed(IAPPlatformID platformId, string error)
	{
		if(ProductListRequestFailedDelegate != null)
			ProductListRequestFailedDelegate(platformId, error);
	}
	
	protected void OnPurchaseSuccessful(IAPProductID brainzProductId, int quantity, IAPPlatformID platformId, Hashtable transactionData)
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

	protected void TurnOffTryToLoadProductsFlag ()
	{
		isTryToLoadProducts = false;
		this.caller.StopCoroutine (CheckProductsTimeOut ());
	}

	private IEnumerator CheckProductsTimeOut ()
	{
		yield return new WaitForSeconds(10);//TimeOutToStore
		if(!HasProducts)
		{
			OnProductListRequestFailed (PlatformId,"Failed");
			ProductListRequestFailedDelegate = null;
		}
		this.caller.StopCoroutine (CheckProductsTimeOut ());
		TurnOffTryToLoadProductsFlag ();
	}

	private void CreateProductInfo (List<IAPProductData> products)
	{
		allProducts = new Dictionary<string, IAPProductInfo> ();
		foreach(IAPProductData product in products)
			allProducts.Add (product.IAPProductId, new IAPProductInfo( product.BrainzProductId , product.Price.ToString()));
	}

	private float GetValueFromPriceString (string priceCurrency)
	{
		Regex numberRegex = new Regex (@"(([0-9]\.*)*[0-9])");
		Match numberMatch = numberRegex.Match (priceCurrency);
		return float.Parse (numberMatch.Value);
	}
}