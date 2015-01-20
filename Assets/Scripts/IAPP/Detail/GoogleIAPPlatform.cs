using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID
public class GoogleIAPPlatform : IAPPlatformBase
{
	private List<IAPProduct> products = new List<IAPProduct>();
	private List<GooglePurchase> pedingPurchases = new List<GooglePurchase>();
	private GooglePurchase lastTransactionData;
	private const string GOOGLE_IAB_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyI70qk86LJd/obggjgfETOccmXs5BmFmppuSad2EQ0CkJuysTb9QsvPj2d/vYxWQKdtx/fEIdG9AO6nyJ8q1u1fuZdRPT1ImZOxT9bAYNJuqWXGKdxcSKQPQ0gcNQ+1FA5z7+GwDv43QqJrhftANe4Td/nIRxex9FE7Eb0FK4XDRFn1nDNCwf02jAmKfz2wUumsKUdspfzOR0ZCo6A659nzg4/86tB0GdZ6rM9XfNviKRNRlMe4gkcIcbY2yXR5bIH71GNjWrQ+UwJNpJK9AlYyWt9YfPrXV9lGSaTaJV7FpXRoeMjsV3O3GrP+NfFkLjKjSo+0zcu5cVrh8MeRPrwIDAQAB";
	private bool supportsBilling = false;
	
	private string sintaxPackages = "com.gamevilusa.markofthedragon.android.google.global.normal.";
	
	protected override string Package1{ get{ return sintaxPackages+ "500ambers";} }
	protected override string Package2{ get{ return sintaxPackages+ "1100ambers";} }
	protected override string Package3{ get{ return sintaxPackages+ "2400ambers";} }
	protected override string Package4{ get{ return sintaxPackages+ "6750ambers";} }
	protected override string Package5{ get{ return sintaxPackages+ "15000ambers";} }

	protected override string SpecialPackage1{ get{ return sintaxPackages+ "hiddentreasure";} }
	protected override string SpecialPackage2{ get{ return sintaxPackages+ "gnomespack";} }
	protected override string SpecialPackage3{ get{ return sintaxPackages+ "luckyrider";} }
	protected override string SpecialPackage4{ get{ return sintaxPackages+ "expertrider";} }
	
	#region IIAPPlatform implementation
	
	public static event Action BillingSupportedEvent;
	
	public static event Action<string> BillingNotSupportedSupportedEvent;
	public override event Action<IAPPlatformID> ProductListReceived;
	public override event Action<IAPPlatformID, string /*error*/> ProductListRequestFailed;
	public override event Action<IAPProductID /*id*/, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessful;
	public override event Action<IAPPlatformID, string /*error*/> PurchaseFailed;
	public override event Action<IAPPlatformID, string /*error*/> PurchaseCancelled;

	private Dictionary<string,string> realPrices = new Dictionary<string,string> ();
	
	public override bool CanMakePayments
	{
		get { return supportsBilling; }
	}
	
	public override List<IAPProduct> Products
	{
		get { return products; }
	}
	
	public override IAPPlatformID ID
	{
		get { return IAPPlatformID.GoogleIAB; }
	}
	
	public override string StoreName
	{
		get { return "Play Store"; }
	}

	protected override void GetProductsDataFromStore ()
	{
		products.Clear();
		
		int numberOfProducts = (int)IAPProductID.ProductCount;
		string[] ids = new string[numberOfProducts];
		
		for ( int i = 0; i < numberOfProducts; ++i )
			ids[i] = IAPProductIDToString ((IAPProductID) i);
		
		GoogleIAB.queryInventory(ids);
	}
	
	public override void PurchaseProduct (IAPProductID id, int quantity)
	{
		GoogleIAB.purchaseProduct (IAPProductIDToString (id),Guid.NewGuid().ToString ());
	}
	
	public override void ConsumeProduct (IAPProductID sku)
	{
		RemovePedingProductToPurchase (sku);
		GoogleIAB.consumeProduct( IAPProductIDToString(sku) );
	}

	private void RemovePedingProductToPurchase (IAPProductID sku)
	{
		GooglePurchase currentProduct = null;
		foreach (GooglePurchase product in pedingPurchases) 
		{
			if (product.productId == IAPProductIDToString (sku))
				currentProduct = product;
		}

		if (currentProduct != null)
			pedingPurchases.Remove (currentProduct);
	}
	
	public override void Dispose()
	{
		products.Clear();
		UnregisterCallbacks();
	}
	
	public override Hashtable GetLastTransactionData()
	{
		Hashtable transactionData = new Hashtable();
		transactionData.Add("productIdentifier", lastTransactionData.productId);
		transactionData.Add("transactionIdentifier", lastTransactionData.orderId);
		transactionData.Add("base64EncodedTransactionReceipt", lastTransactionData.purchaseToken);
		transactionData.Add("quantity", 1);
		
		foreach(IAPProduct product in products)
		{
			if ( IAPProductIDToString(product.productIdentifier) == lastTransactionData.productId)
			{
				string price = GetPriceByPackageID (product.productIdentifier);
				transactionData.Add("price",price );
				transactionData.Add("currencyCode", product.currencyCode);
				break;
			}
		}
		
		return transactionData;
	}
	
	#endregion // IIAPPlatform implementation
	
	public GoogleIAPPlatform()
	{
		CreatePackagesInfo ();
		GoogleIAB.init( GOOGLE_IAB_KEY );
		RegisterCallbacks();
	}
	
	private void RegisterCallbacks()
	{
		GoogleIABManager.billingSupportedEvent += OnBillingEvent;
		GoogleIABManager.billingNotSupportedEvent += OnBillingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent += OnProductListReceived;
		GoogleIABManager.queryInventoryFailedEvent += OnProductListRequestFailed;
		GoogleIABManager.purchaseSucceededEvent += OnPurchaseSuccedded;
		GoogleIABManager.purchaseFailedEvent += OnPurchaseFailed;
	}
	
	private void UnregisterCallbacks()
	{
		GoogleIABManager.billingSupportedEvent -= OnBillingEvent;
		GoogleIABManager.billingNotSupportedEvent -= OnBillingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= OnProductListReceived;
		GoogleIABManager.queryInventoryFailedEvent -= OnProductListRequestFailed;
		GoogleIABManager.purchaseSucceededEvent -= OnPurchaseSuccedded;
		GoogleIABManager.purchaseFailedEvent -= OnPurchaseFailed;
	}
	
	#region Callbacks

	public override void ValidatePedingPurchases ()
	{
		foreach (GooglePurchase purchaseProduct in pedingPurchases)
			OnPurchaseSuccedded (purchaseProduct);
	}

	private void OnProductListReceived(List<GooglePurchase> purchasesList, List<GoogleSkuInfo> iabProductList)
	{
		Debug.Log("Pending purchases: " + purchasesList.Count + " List products: " + iabProductList.Count);
		products.Clear();
		realPrices = new Dictionary<string,string> ();
		pedingPurchases = purchasesList;
		foreach (GoogleSkuInfo iabProduct in iabProductList)
		{
			Debug.Log (iabProduct.ToString () + " this is the products to purchase");
			IAPProductID id;
			if ( StringToIAPProductID(iabProduct.productId, out id) )	
			{
				if (iabProduct.priceCurrencyCode != null && iabProduct.description != null && iabProduct.price != null && iabProduct.title != null)
				{
					IAPProduct product = new IAPProduct();
					product.currencySymbol = iabProduct.priceCurrencyCode;
					product.currencyCode = iabProduct.priceCurrencyCode;
					product.description = iabProduct.description;
					product.price = iabProduct.price;
					product.formattedPrice = iabProduct.price;
					product.productIdentifier = id;
					product.title = iabProduct.title;
					products.Add(product);
					CurrencyCode = product.currencyCode;

					if(!realPrices.ContainsKey (iabProduct.productId))
						realPrices.Add(iabProduct.productId,iabProduct.price);

					SetCurrencyPrice (iabProduct.productId,iabProduct.price);
					
					Debug.Log("Loaded product: " + product.ToString());
				}
				else
					Debug.LogWarning ("IAP product ignored because it contains null data: " + id);
			}
			else
				Debug.LogWarning("An unrecognized IAP product was reported by StoreKit. Identifier=" + iabProduct.productId);		
		}

		TurnOffTryToLoadProductsFlag ();
		// Sort by product id
		products.Sort(delegate(IAPProduct p1, IAPProduct p2) { return p1.productIdentifier.CompareTo(p2.productIdentifier); });
		
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
	
	private void OnPurchaseSuccedded(GooglePurchase data)
	{
		if (PurchaseSuccessful != null)
		{
			IAPProductID id;
			if ( StringToIAPProductID( data.productId, out id ) )
			{
				lastTransactionData = data;
				Hashtable table = new Hashtable();
				table.Add ("orderId" , data.orderId);
				table.Add ("packageName" , data.packageName);
				table.Add ("purchaseTime" , data.purchaseTime.ToString ());
				table.Add ("productId" , data.productId);
				table.Add ("purchaseState" , data.purchaseState.ToString ());
				table.Add ("developerPayload" , data.developerPayload);
				table.Add ("purchaseToken" , data.purchaseToken);
				table.Add ("pack", id.ToString ());
				table.Add ("receipt", data.orderId);
				table.Add ("price",realPrices[data.productId]);
				table.Add ("signature",data.signature);
				table.Add ("r_data",data.originalJson);
				//Logger.Log (string.Format ("This is the data to confirm purchase == {0}",JSON.JsonEncode(table)), DebugLogType.IAPs);
				
				PurchaseSuccessful(id, 1, ID, table);
			}
			else
			{
				Debug.LogWarning("An unrecognized IAP product was reported by Google IAB. Identifier=" + data.productId);
			}
		}
	}
	
	private void OnBillingEvent ()
	{
		supportsBilling = true;
		
		if (BillingSupportedEvent != null)
			BillingSupportedEvent ();
	}
	
	private void OnBillingNotSupportedEvent(string error)
	{
		supportsBilling = false;
		
		if (BillingNotSupportedSupportedEvent != null)
			BillingNotSupportedSupportedEvent (error);
	}
	
	#endregion // Callbacks
}
#endif