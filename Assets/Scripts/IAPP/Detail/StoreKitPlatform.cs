using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_IPHONE
public class StoreKitPlatform : IAPPlatformBase
{
	private List<IAPProduct> products = new List<IAPProduct>();
	private StoreKitTransaction lastTransactionData;
	
	#region IIAPPlatform implementation

	public override event Action<IAPPlatformID> ProductListReceived;
	public override event Action<IAPPlatformID, string /*error*/> ProductListRequestFailed;
	public override event Action<IAPProductID /*id*/, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessful;
	public override event Action<IAPPlatformID, string /*error*/> PurchaseFailed;
	public override event Action<IAPPlatformID, string /*error*/> PurchaseCancelled;
	public override void ValidatePedingPurchases (){}

	public override bool CanMakePayments
	{
		get { return StoreKitBinding.canMakePayments(); }
	}
	
	public override List<IAPProduct> Products
	{
		get { return products; }
	}
	
	public override IAPPlatformID ID
	{
		get { return IAPPlatformID.StoreKit; }
	}
	
	public override string StoreName
	{
		get { return "the Apple Store"; }
	}
	
	
	public override void ConsumeProduct(IAPProductID id){}

	protected override void GetProductsDataFromStore ()
	{
		products.Clear();
		
		int numberOfProducts = (int)IAPProductID.ProductCount;
		string[] ids = new string[numberOfProducts];
		
		for ( int i = 0; i < numberOfProducts; ++i )
			ids[i] = IAPProductIDToString ((IAPProductID) i);
		
		StoreKitBinding.requestProductData(ids);
	}
	
	public override void PurchaseProduct (IAPProductID id, int quantity)
	{
		string idProductToPurchase = IAPProductIDToString (id);
		StoreKitBinding.purchaseProduct (idProductToPurchase, quantity);
	}


	public override void Dispose()
	{
		products.Clear();
		UnregisterCallbacks();
	}
	
	public override Hashtable GetLastTransactionData()
	{
		Hashtable transactionData = new Hashtable ();
		transactionData.Add("productIdentifier", lastTransactionData.productIdentifier);
		transactionData.Add("transactionIdentifier", lastTransactionData.transactionIdentifier);
		transactionData.Add("base64EncodedTransactionReceipt", lastTransactionData.base64EncodedTransactionReceipt);
		transactionData.Add("quantity", lastTransactionData.quantity);
		transactionData.Add("price", "");
		transactionData.Add("currencyCode", "");
		foreach(IAPProduct product in products)
		{
			if ( IAPProductIDToString(product.productIdentifier) == lastTransactionData.productIdentifier)
			{
				string price = GetPriceByPackageID (product.productIdentifier);
				transactionData["price"] = price;
				transactionData["currencyCode"] = product.currencyCode;
				break;
			}
		}
		
		return transactionData;
	}
	

	#endregion // IIAPPlatform implementation
			
	public StoreKitPlatform()
	{
		CreatePackagesInfo ();
		RegisterCallbacks();
	}

	private void RegisterCallbacks()
	{
		StoreKitManager.productListReceivedEvent += OnProductListReceived;
		StoreKitManager.productListRequestFailedEvent += OnProductListRequestFailed;
		StoreKitManager.purchaseCancelledEvent += OnPurchaseCancelled;
		StoreKitManager.purchaseFailedEvent += OnPurchaseFailed;
		StoreKitManager.purchaseSuccessfulEvent += OnPurchaseSuccedded;
	}
	
	private void UnregisterCallbacks()
	{
		StoreKitManager.productListReceivedEvent -= OnProductListReceived;
		StoreKitManager.productListRequestFailedEvent -= OnProductListRequestFailed;
		StoreKitManager.purchaseCancelledEvent -= OnPurchaseCancelled;
		StoreKitManager.purchaseFailedEvent -= OnPurchaseFailed;
		StoreKitManager.purchaseSuccessfulEvent -= OnPurchaseSuccedded;
	}
	
	#region Callbacks

	private void OnProductListReceived(List<StoreKitProduct> skProductList)
	{
		products.Clear();

		foreach (StoreKitProduct skProduct in skProductList)
		{
			IAPProductID id;
			if ( StringToIAPProductID(skProduct.productIdentifier, out id) )	
			{
				if (skProduct.currencySymbol != null && skProduct.description != null && skProduct.price != null && skProduct.title != null)
				{
					IAPProduct product = new IAPProduct();
					product.currencySymbol = skProduct.currencySymbol;
					product.currencyCode = skProduct.currencyCode;
					product.description = skProduct.description;
					product.price = skProduct.price;
					product.formattedPrice = skProduct.formattedPrice;
					product.productIdentifier = id;
					product.title = skProduct.title;
					products.Add(product);
					CurrencyCode = product.currencyCode;
					SetCurrencyPrice (skProduct.productIdentifier,skProduct.formattedPrice);
					
					Logger.Log("Loaded product: " + product.ToString(), DebugLogType.IAPs);
				}
				else
				{
					Debug.LogWarning ("IAP product ignored because it contains null data: " + id);
				}
			}
			else
			{
				Debug.LogWarning("An unrecognized IAP product was reported by StoreKit. Identifier=" + skProduct.productIdentifier);
			}			
		}
	
        products.Sort(delegate(IAPProduct p1, IAPProduct p2) { return p1.productIdentifier.CompareTo(p2.productIdentifier); });
		TurnOffTryToLoadProductsFlag ();
		if (ProductListReceived != null)
			ProductListReceived(ID);
	}
	
	private void OnProductListRequestFailed(string error)
	{
		TurnOffTryToLoadProductsFlag ();
		if (ProductListRequestFailed != null)
			ProductListRequestFailed(ID, error);
	}
	
	private void OnPurchaseCancelled(string error)
	{
		if (PurchaseCancelled != null)
			PurchaseCancelled (ID, error);
	}
	
	private void OnPurchaseFailed(string error)
	{
		if (PurchaseFailed != null)
			PurchaseFailed (ID, error);
	}
	
	private void OnPurchaseSuccedded(StoreKitTransaction data)
	{
		if (PurchaseSuccessful != null)
		{
			IAPProductID id;
			if ( StringToIAPProductID( data.productIdentifier, out id ) )
			{
				lastTransactionData = data;
				
				Hashtable table = new Hashtable();
				table.Add ("receipt", data.base64EncodedTransactionReceipt);
				table.Add ("sandbox", !ServerSettings.Instance.IsLiveEnvironment);
				table.Add ("pack", id.ToString ());
				PurchaseSuccessful(id, data.quantity, ID, table);
			}
			else
			{
				Debug.LogWarning("An unrecognized IAP product was reported by StoreKit. Identifier=" + data.productIdentifier);
			}
		}
	}

	private string GetCurrencyCode (string productIdentifier)
	{
		throw new NotImplementedException ();
	}
	
	#endregion // Callbacks
}
#endif