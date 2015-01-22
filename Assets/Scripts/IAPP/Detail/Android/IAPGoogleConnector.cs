using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class IAPGoogleConnector : IIAPGoogleConnector
{

	public event Action BillingSupportedDelegate;
	public event Action<string> BillingNotSupportedDelegate;
	public event Action<List<IGooglePurchaseInfo>, List<IGoogleProductInfo>> ProductListReceivedDelegate;
	public event Action<string> ProductListRequestFailedDelegate;
	public event Action<IGooglePurchaseInfo> PurchaseSucceededDelegate;
	public event Action<string> PurchaseFailedDelegate;


	public void Initialize(string publicKey)
	{
		#if UNITY_ANDROID
		GoogleIAB.init(publicKey);
		#endif
	}

	public void PurchaseProduct(string iapProductId, string developerPayload)
	{
		#if UNITY_ANDROID
		GoogleIAB.purchaseProduct(iapProductId, developerPayload);
		#endif
	}

	public void ConsumeProduct(string iapProductId)
	{
		#if UNITY_ANDROID
		GoogleIAB.consumeProduct(iapProductId);
		#endif
	}

	public void GetProducts(string[] iapProductIds)
	{
		#if UNITY_ANDROID
		GoogleIAB.queryInventory(iapProductIds);
		#endif
	}

	#if UNITY_ANDROID
	public IAPGoogleConnector()
	{
		RegisterCallbacks();
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
			ProductListReceivedDelegate(ToPurchaseInfoList(purchasesList), ToProductInfoList(iabProductList));
	}

	private void OnProductListRequestFailed(string error)
	{
		if(ProductListRequestFailedDelegate != null)
			ProductListRequestFailedDelegate(error);
	}

	private void OnPurchaseSucceeded(GooglePurchase iapProductData)
	{
		if(PurchaseSucceededDelegate != null)
			PurchaseSucceededDelegate(CreatePurchaseInfo(iapProductData));
	}

	private void OnPurchaseFailed(string error, int quantiy)
	{
		if(PurchaseFailedDelegate != null)
			PurchaseFailedDelegate(error);
	}

	private List<IGooglePurchaseInfo> ToPurchaseInfoList(List<GooglePurchase> purchasesList)
	{
		List<IGooglePurchaseInfo> purchaseInfoList = new List<IGooglePurchaseInfo>();
		foreach(GooglePurchase googlePurchase in purchasesList)
			purchaseInfoList.Add(CreatePurchaseInfo(googlePurchase));

		return purchaseInfoList;
	}

	private List<IGoogleProductInfo> ToProductInfoList(List<GoogleSkuInfo> skuInfoList)
	{
		List<IGoogleProductInfo> productInfoList = new List<IGoogleProductInfo>();
		foreach(GoogleSkuInfo googleSkuInfo in skuInfoList)
			productInfoList.Add(CreateProductInfo(googleSkuInfo));
		
		return productInfoList;
	}

	private IGoogleProductInfo CreateProductInfo(GoogleSkuInfo skuInfo)
	{
		GoogleProductInfo productInfo = new GoogleProductInfo();
		productInfo.ProductInfo = skuInfo;
		return productInfo;
	}

	private IGooglePurchaseInfo CreatePurchaseInfo(GooglePurchase googlePurchaseInfo)
	{
		GooglePurchaseInfo purchaseInfo = new GooglePurchaseInfo();
		purchaseInfo.PurchaseInfo = googlePurchaseInfo;
		return purchaseInfo;
	}
	#endif
}
