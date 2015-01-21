using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#if UNITY_ANDROID
public class IAPGoolgleConnector : IIAPGoolgleConnector
{
	public event Action BillingSupportedDelegate;
	public event Action<string> BillingNotSupportedDelegate;
	public event Action<List<GooglePurchase>, List<GoogleSkuInfo>> ProductListReceivedDelegate;
	public event Action<string> ProductListRequestFailedDelegate;
	public event Action<GooglePurchase> PurchaseSucceededDelegate;
	public event Action<string> PurchaseFailedDelegate;
	
	public void Initialize(string publicKey)
	{
		GoogleIAB.init(publicKey);
	}

	public void PurchaseProduct(string iapProductId, string developerPayload)
	{
		GoogleIAB.purchaseProduct(iapProductId, developerPayload);
	}

	public void ConsumeProduct(string iapProductId)
	{
		GoogleIAB.consumeProduct(iapProductId);
	}

	public void GetProducts(string[] iapProductIds)
	{
		GoogleIAB.queryInventory(iapProductIds);
	}

	private void RegisterCallbacks()
	{
		GoogleIABManager.billingSupportedEvent += OnBillingSupported;
		GoogleIABManager.billingNotSupportedEvent += OnBillingNotSupported;
		GoogleIABManager.queryInventorySucceededEvent += OnProductListReceived;
		GoogleIABManager.queryInventoryFailedEvent += OnProductListRequestFailed;
		GoogleIABManager.purchaseSucceededEvent += OnPurchaseSucceeded;
		GoogleIABManager.purchaseFailedEvent += OnPurchaseFailed;
	}

	private void OnBillingSupported()
	{
		if(BillingSupportedDelegate != null)
			BillingSupportedDelegate();
	}

	private void OnBillingNotSupported(string error)
	{
		if(BillingNotSupportedDelegate != null)
			BillingNotSupportedDelegate(error);
	}
	
	private void OnProductListReceived(List<GooglePurchase> purchasesList, List<GoogleSkuInfo> iabProductList)
	{
		if(ProductListReceivedDelegate != null)
			ProductListReceivedDelegate(purchasesList, iabProductList);
	}

	private void OnProductListRequestFailed(string error)
	{
		if(ProductListRequestFailedDelegate != null)
			ProductListRequestFailedDelegate(error);
	}

	private void OnPurchaseSucceeded(GooglePurchase iapProductData)
	{
		if(PurchaseSucceededDelegate != null)
			PurchaseSucceededDelegate(iapProductData);
	}

	private void OnPurchaseFailed(string error)
	{
		if(PurchaseFailedDelegate != null)
			PurchaseFailedDelegate(error);
	}
}
#endif