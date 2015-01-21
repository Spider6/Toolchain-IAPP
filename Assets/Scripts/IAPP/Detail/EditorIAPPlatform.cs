using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

public class EditorIAPPlatform : DummyIAPPlatform
{
	public override void ConsumeProduct(IAPProductID id){}
	public override void Dispose(){}

	public EditorIAPPlatform (List<IIAPProductData> products, List<IAPProduct> debugProducts) : base(products)
	{
		dummyProducts = debugProducts;
	}

	public override void PurchaseProduct(IAPProductID brainzProductId, int quantity)
	{
		caller.StartCoroutine (PurchaseAsync(brainzProductId, quantity));
	}

	protected override void GetProductsDataFromStore ()
	{
		if(HasProducts)
			caller.StartCoroutine (ReceiveProductListAsync());
	}
	
	private IEnumerator ReceiveProductListAsync()
	{
		yield return new WaitForSeconds (10);//StorePlatformDelayInSeconds
		foreach (IAPProduct product in Products)
		{
			SetCurrencyPrice(product.brainzProductId.ToString (), product.formattedPrice);
			CurrencyCode = product.currencyCode;
		}
		TurnOffTryToLoadProductsFlag ();
	    OnProductListReceived(PlatformId);
	}
	
	private IEnumerator PurchaseAsync(IAPProductID brainzProductId, int quantity)
	{
		yield return new WaitForSeconds (10);//StorePlatformDelayInSeconds

		Hashtable table = GetInfoPurchaseProduct (brainzProductId, quantity);
		OnPurchaseSuccessful(brainzProductId, quantity, PlatformId, table);
	}
}
#endif