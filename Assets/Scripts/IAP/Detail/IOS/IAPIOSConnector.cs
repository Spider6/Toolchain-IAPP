using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using IAP;

namespace IAP.Detail
{
	public class IAPIOSConnector : IIAPIOSConnector 
	{
		public event Action<List<IIOSProductInfo>> ProductListReceivedDelegate;
		public event Action<string> ProductListRequestFailedDelegate;
		public event Action<IIOSPurchaseInfo> PurchaseSucceededDelegate;
		public event Action<string> PurchaseFailedDelegate;
		public event Action<string> PurchaseCancelledDelegate;

		public bool CanMakePayments
		{ 
			get
			{
				#if UNITY_IOS
				return StoreKitBinding.canMakePayments(); 
				#else
				return false;
				#endif
			}
		}

		public void PurchaseProduct(string iapProductId, int quantity)
		{
			#if UNITY_IOS
			StoreKitBinding.purchaseProduct (iapProductId, quantity);
			#endif
		}

		public void GetProducts(string[] iapProductIds)
		{
			#if UNITY_IOS
			StoreKitBinding.requestProductData(iapProductIds);
			#endif
		}

		#if UNITY_IOS
		public IAPIOSConnector()
		{
			RegisterCallbacks();
		}

		private void RegisterCallbacks()
		{
			StoreKitManager.productListReceivedEvent += OnProductListReceived;
			StoreKitManager.productListRequestFailedEvent += OnProductListRequestFailed;
			StoreKitManager.purchaseCancelledEvent += OnPurchaseCancelled;
			StoreKitManager.purchaseFailedEvent += OnPurchaseFailed;
			StoreKitManager.purchaseSuccessfulEvent += OnPurchaseSucceeded;
		}

		private void OnProductListReceived(List<StoreKitProduct> skProductInfoList)
		{
			if(ProductListReceivedDelegate != null)
				ProductListReceivedDelegate(ToIOSProductInfo(skProductInfoList));
		}
		
		private void OnProductListRequestFailed(string error)
		{
			if(ProductListRequestFailedDelegate != null)
				ProductListRequestFailedDelegate(error);
		}
		
		private void OnPurchaseSucceeded(StoreKitTransaction skTransaction)
		{
			if(PurchaseSucceededDelegate != null)
				PurchaseSucceededDelegate(CreateIOSPurchaseInfo(skTransaction));
		}
		
		private void OnPurchaseFailed(string error)
		{
			if(PurchaseFailedDelegate != null)
				PurchaseFailedDelegate(error);
		}

		private void OnPurchaseCancelled(string error)
		{
			if(PurchaseCancelledDelegate != null)
				PurchaseCancelledDelegate(error);
		}

		private List<IIOSProductInfo> ToIOSProductInfo(List<StoreKitProduct> skProductInfoList)
		{
			List<IIOSProductInfo> productInfoList = new List<IIOSProductInfo>();
			foreach(StoreKitProduct skProduct in skProductInfoList)
				productInfoList.Add(CreateIOSProductInfo(skProduct));

			return productInfoList;
		}

		private IIOSProductInfo CreateIOSProductInfo(StoreKitProduct skProduct)
		{
			IOSProductInfo productInfo = new IOSProductInfo();
			productInfo.ProductInfo = skProduct;

			return productInfo;
		}

		private IIOSPurchaseInfo CreateIOSPurchaseInfo(StoreKitTransaction skTransaction)
		{
			IOSPurchaseInfo purchaseInfo = new IOSPurchaseInfo();
			purchaseInfo.PurchaseInfo = skTransaction;

			return purchaseInfo;
		}
		#endif
	}
}
