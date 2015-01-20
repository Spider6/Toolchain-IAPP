using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public abstract class IAPPlatformBase : IIAPPlatform
{
	protected virtual string Package1{ get{ return "motd_500ambers";} }
	protected virtual string Package2{ get{ return "motd_1100ambers";} }
	protected virtual string Package3{ get{ return "motd_2400ambers";} }
	protected virtual string Package4{ get{ return "motd_6750ambers";} }
	protected virtual string Package5{ get{ return "motd_15000ambers";} }

	protected virtual string SpecialPackage1{ get{ return "motd_hiddentreasure";} }
	protected virtual string SpecialPackage2{ get{ return "motd_gnomespack";} }
	protected virtual string SpecialPackage3{ get{ return "motd_luckyrider";} }
	protected virtual string SpecialPackage4{ get{ return "motd_expertrider";} }

	public abstract void ValidatePedingPurchases ();
	public virtual event Action<IAPPlatformID> ProductListReceived;
	public virtual event Action<IAPPlatformID, string /*error*/> ProductListRequestFailed;
	public virtual event Action<IAPProductID, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessful;
	public virtual event Action<IAPPlatformID, string /*error*/> PurchaseFailed;
	public virtual event Action<IAPPlatformID, string /*error*/> PurchaseCancelled;
	private Dictionary<string,IAPProductInfo> allPackages;

	public abstract bool CanMakePayments { get; }
	public abstract List<IAPProduct> Products { get; }
	public abstract IAPPlatformID ID { get; }
	public abstract string StoreName { get; }
	public bool HasProducts { get { return Products.Count > 0; } }
	public string CurrencyCode { get; protected set; }
	private bool isTryToLoadProducts = false;
	private Action<IAPPlatformID,string> callbackTimeOut = null;
	private MonoBehaviour caller;

	public bool IsSpecialPackage (IAPProductID productId)
	{
		return (productId == GetIAPProductID(SpecialPackage1) || productId == GetIAPProductID(SpecialPackage2) || productId == GetIAPProductID(SpecialPackage3) || productId == GetIAPProductID(SpecialPackage4));
	}

	public IAPProductID GetIApproductByStringProductID (string productID)
	{
		return GetIAPProductID (productID);
	}
	
	public abstract void PurchaseProduct(IAPProductID id, int quantity);
	public abstract void ConsumeProduct(IAPProductID id);
	public abstract void Dispose();
	public abstract Hashtable GetLastTransactionData();
	protected abstract void GetProductsDataFromStore();

	public virtual void RequestAllProductData(MonoBehaviour caller,Action<IAPPlatformID,string> callbackFailed)
	{
		if(!isTryToLoadProducts)
		{
			isTryToLoadProducts = true;
			callbackTimeOut = callbackFailed;
			this.caller = caller;
			this.caller.StartCoroutine (CheckProductsTimeOut ());
			GetProductsDataFromStore();
		}
	}

	private IEnumerator CheckProductsTimeOut ()
	{
		yield return new WaitForSeconds(10);//TimeOutToStore
		if(!HasProducts && callbackTimeOut != null)
		{
			callbackTimeOut (ID,"Failed");
			callbackTimeOut = null;
		}
		this.caller.StopCoroutine (CheckProductsTimeOut ());
		TurnOffTryToLoadProductsFlag ();
	}

	protected void TurnOffTryToLoadProductsFlag ()
	{
		isTryToLoadProducts = false;
		this.caller.StopCoroutine (CheckProductsTimeOut ());
	}

	public string GetCurrencyPrice (IAPProductID productId)
	{
		string currentPackage = IAPProductIDToString (productId);
		IAPProductInfo product = allPackages[currentPackage];

		if(product != null && product.ID == productId)
			return product.CurrencyPrice;

		return string.Empty;
	}
	
	protected string IAPProductIDToString( IAPProductID id )
	{
		foreach(KeyValuePair<string, IAPProductInfo> entry in allPackages)
		{
			IAPProductInfo info = entry.Value as IAPProductInfo;
			if(info.ID == id)
				return entry.Key.ToString ();
		}
		return "";
	}

	protected void CreatePackagesInfo ()
	{
		allPackages = new Dictionary<string, IAPProductInfo> ();
		allPackages.Add (Package1, new IAPProductInfo( IAPProductID.AmberPack1 , "4.99" ));
		allPackages.Add (Package2, new IAPProductInfo( IAPProductID.AmberPack2 , "9.99" ));
		allPackages.Add (Package3, new IAPProductInfo( IAPProductID.AmberPack3 , "19.99" ));
		allPackages.Add (Package4, new IAPProductInfo( IAPProductID.AmberPack4 , "49.99" ));
		allPackages.Add (Package5, new IAPProductInfo( IAPProductID.AmberPack5 , "99.99" ));
		allPackages.Add (SpecialPackage1, new IAPProductInfo( IAPProductID.SpecialPack1 , "2.99" ));
		allPackages.Add (SpecialPackage2, new IAPProductInfo( IAPProductID.SpecialPack2 , "19.99" ));
		allPackages.Add (SpecialPackage3, new IAPProductInfo( IAPProductID.SpecialPack3 , "19.99" ));
		allPackages.Add (SpecialPackage4, new IAPProductInfo( IAPProductID.SpecialPack4 , "49.99" ));
	}

	public string GetPriceWithoutDiscount (IAPProductID product, float discountPercent)
	{
		float price = GetPriceInFloat (product);
		float originalValue = (price * 100f) / (100f - discountPercent);
		return CurrencyCode + originalValue.ToString ("0");
	}

	private float GetValueFromPriceString (string priceCurrency)
	{
		Regex numberRegex = new Regex (@"(([0-9]\.*)*[0-9])");
		Match numberMatch = numberRegex.Match (priceCurrency);
		return float.Parse (numberMatch.Value);
	}

	public float GetPriceInFloat (IAPProductID productID)
	{
		string currencyPrice = GetCurrencyPrice(productID);
		return GetValueFromPriceString (currencyPrice);
	}

	protected void SetCurrencyPrice (string packageID,string price)
	{
		allPackages[packageID].CurrencyPrice = price;
	}

	protected bool StringToIAPProductID( string stringId, out IAPProductID id )
	{
		stringId = GetPackageID (stringId);
		
		if (Enum.IsDefined(typeof(IAPProductID), stringId))
		{
			id = (IAPProductID) Enum.Parse(typeof(IAPProductID), stringId, true);
			return true;
		}
		else
		{
			id = IAPProductID.ProductCount;  // Assign some value
			return false;
		}
	}

	protected string GetPriceByPackageID (IAPProductID productID)
	{
		string priceValue = GetPriceStringByPackageID (productID);
		string price = Regex.Replace (priceValue, "[^,'0-9.]", "");
		return price;
	}

	public string GetPriceStringByPackageID (IAPProductID productID)
	{
		foreach(KeyValuePair<string, IAPProductInfo> entry in allPackages)
		{
			IAPProductInfo info = entry.Value as IAPProductInfo;
			if(info.ID == productID)
				return info.Price;
		}
		return "0";
	}

	private string GetPackageID (string productID)
	{
		return GetIAPProductID (productID).ToString ();
	}

	private IAPProductID GetIAPProductID (string productID)
	{
		IAPProductInfo info = allPackages [productID] as IAPProductInfo;
		return info.ID;
	}
}