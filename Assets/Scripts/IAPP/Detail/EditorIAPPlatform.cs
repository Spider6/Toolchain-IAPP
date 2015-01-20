using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

public class EditorIAPPlatform : DummyIAPPlatform
{	
	#region IIAPPlatform implementation
	public override event Action<IAPPlatformID> ProductListReceived;
	public override event Action<IAPPlatformID, string /*error*/> ProductListRequestFailed;
	public override event Action<IAPProductID /*id*/, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessful;
	public override event Action<IAPPlatformID, string /*error*/> PurchaseFailed;
	public override event Action<IAPPlatformID, string /*error*/> PurchaseCancelled;

	public EditorIAPPlatform ()
	{
		this.CreatePackagesInfo ();
	}

	public override List<IAPProduct> Products
	{
		get { return new List<IAPProduct>(); }//DebugPacks
	}

	protected override void GetProductsDataFromStore ()
	{
//		if(HasProducts)
//			((IAPManager)Locator.IAPManager).StartCoroutine (ReceiveProductListAsync());
	}
	
	private IEnumerator ReceiveProductListAsync()
	{
		yield return new WaitForSeconds (10);//StorePlatformDelayInSeconds
		foreach (IAPProduct product in Products)
		{
			SetCurrencyPrice(product.productIdentifier.ToString (),product.formattedPrice);
			CurrencyCode = product.currencyCode;
		}
		TurnOffTryToLoadProductsFlag ();
		if (ProductListReceived != null)
			ProductListReceived(ID);
	}
	
	public override void PurchaseProduct(IAPProductID id, int quantity)
	{
		//((IAPManager)Locator.IAPManager).StartCoroutine (PurchaseAsync(id, quantity));
	}
	
	private IEnumerator PurchaseAsync(IAPProductID id, int quantity)
	{
		yield return new WaitForSeconds (10);//StorePlatformDelayInSeconds

		if (PurchaseSuccessful != null)
		{
			Hashtable table = GetInfoPurchaseProduct (id,quantity);
			PurchaseSuccessful(id, quantity, ID, table);
		}
	}

	private DummyIAPProduct GetDummyIAPProduct (IAPProductID id)
	{
		return new DummyIAPProduct (id);
	}
		
	public override void ConsumeProduct(IAPProductID id){}
	public override void Dispose(){}
	#endregion // IIAPPlatform implementation
}
#endif