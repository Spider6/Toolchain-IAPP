using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID
public class GoogleIAPPlatform : IAPPlatformBase
{
	private const string GOOGLE_IAB_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyI70qk86LJd/obggjgfETOccmXs5BmFmppuSad2EQ0CkJuysTb9QsvPj2d/vYxWQKdtx/fEIdG9AO6nyJ8q1u1fuZdRPT1ImZOxT9bAYNJuqWXGKdxcSKQPQ0gcNQ+1FA5z7+GwDv43QqJrhftANe4Td/nIRxex9FE7Eb0FK4XDRFn1nDNCwf02jAmKfz2wUumsKUdspfzOR0ZCo6A659nzg4/86tB0GdZ6rM9XfNviKRNRlMe4gkcIcbY2yXR5bIH71GNjWrQ+UwJNpJK9AlYyWt9YfPrXV9lGSaTaJV7FpXRoeMjsV3O3GrP+NfFkLjKjSo+0zcu5cVrh8MeRPrwIDAQAB";

	public static event Action BillingSupportedEvent;
	public static event Action<string> BillingNotSupportedSupportedEvent;

	private List<IAPProduct> products = new List<IAPProduct>();
	private List<GooglePurchase> pedingPurchases = new List<GooglePurchase>();
	private GooglePurchase lastTransactionData;
	private bool supportsBilling = false;
	private Dictionary<string,string> realPrices = new Dictionary<string,string> ();
	
	public override bool CanMakePayments
	{
		get { return supportsBilling; }
	}
	
	public override List<IAPProduct> Products
	{
		get { return products; }
	}
	
	public override IAPPlatformID PlatformId
	{
		get { return IAPPlatformID.GoogleIAB; }
	}
	
	public override string StoreName
	{
		get { return "Play Store"; }
	}

	public override void Dispose()
	{
		products.Clear();
		UnregisterCallbacks();
	}

	public GoogleIAPPlatform(List<IAPProductData> products): base(products)
	{
		GoogleIAB.init(GOOGLE_IAB_KEY);
		RegisterCallbacks();
	}

	public override Hashtable GetLastTransactionData()
	{
		Hashtable transactionData = new Hashtable();
		transactionData.Add("productIdentifier", lastTransactionData.productId);
		transactionData.Add("transactionIdentifier", lastTransactionData.orderId);
		transactionData.Add("base64EncodedTransactionReceipt", lastTransactionData.purchaseToken);
		transactionData.Add("quantity", 1);
		
		IAPProduct product = products.Find(p => BrainzProductIdToIAPProductId(p.brainzProductId) == lastTransactionData.productId);
		if (product != null)
		{
			string price = GetPriceByBrainzIAPProductId (product.brainzProductId);
			transactionData.Add("price",price );
			transactionData.Add("currencyCode", product.currencyCode);
		}
		
		return transactionData;
	}

	public override void PurchaseProduct (IAPProductID brainzProductId, int quantity)
	{
		GoogleIAB.purchaseProduct (BrainzProductIdToIAPProductId(brainzProductId), Guid.NewGuid().ToString ());
	}
	
	public override void ConsumeProduct (IAPProductID brainzProductId)
	{
		RemovePedingProductToPurchase (brainzProductId);
		GoogleIAB.consumeProduct(BrainzProductIdToIAPProductId(brainzProductId));
	}

	public override void ValidatePedingPurchases ()
	{
		foreach (GooglePurchase purchaseProduct in pedingPurchases)
			OnPurchaseSuccedded (purchaseProduct);
	}

	protected override void GetProductsDataFromStore ()
	{
		products.Clear();
		GoogleIAB.queryInventory(GetAllIAPProductId());
	}

	private void RemovePedingProductToPurchase (IAPProductID brainzProductId)
	{
		GooglePurchase currentProduct;
		GooglePurchase product = pedingPurchases.Find(p => p.productId == BrainzProductIdToIAPProductId (brainzProductId));
		currentProduct = product;
		
		if (currentProduct != null)
			pedingPurchases.Remove (currentProduct);
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

	private void OnProductListReceived(List<GooglePurchase> purchasesList, List<GoogleSkuInfo> iabProductList)
	{
		Debug.Log("Pending purchases: " + purchasesList.Count + " List products: " + iabProductList.Count);
		products.Clear();
		realPrices = new Dictionary<string,string> ();
		pedingPurchases = purchasesList;
		FillProductList (iabProductList);
		TurnOffTryToLoadProductsFlag ();
		products.Sort(delegate(IAPProduct p1, IAPProduct p2) { return p1.brainzProductId.CompareTo(p2.brainzProductId); });
		OnProductListReceived(PlatformId);
	}

	private void FillProductList (List<GoogleSkuInfo> iabProductList)
	{
		foreach (GoogleSkuInfo iabProduct in iabProductList) 
		{
			Debug.Log (iabProduct.ToString () + " this is the products to purchase");
			IAPProductID brainzProductId = IAPProductIDToBrainzProductId (iabProduct.productId);
			if (brainzProductId != IAPProductID.None)
				AddIAPProduct (iabProduct, brainzProductId);
			else
				Debug.LogWarning ("An unrecognized IAP product was reported by Google Store. Identifier: " + iabProduct.productId);
		}
	}

	private void AddIAPProduct (GoogleSkuInfo iabProduct, IAPProductID brainzProductId)
	{
		if (iabProduct.priceCurrencyCode != null && iabProduct.description != null && iabProduct.price != null && iabProduct.title != null) 
		{
			IAPProduct newProduct = CreateIAPProduct (iabProduct, brainzProductId);
			products.Add (newProduct);
			CurrencyCode = newProduct.currencyCode;
			if (!realPrices.ContainsKey (iabProduct.productId))
				realPrices.Add (iabProduct.productId, iabProduct.price);
			SetCurrencyPrice (iabProduct.productId, iabProduct.price);
			Debug.Log ("Loaded product: " + newProduct.ToString ());
		}
		else
			Debug.LogWarning ("IAP product ignored because it contains null data: " + brainzProductId);
	}

	private IAPProduct CreateIAPProduct(GoogleSkuInfo iabProduct, IAPProductID brainzProductId)
	{
		IAPProduct newProduct = new IAPProduct();
		newProduct.currencySymbol = iabProduct.priceCurrencyCode;
		newProduct.currencyCode = iabProduct.priceCurrencyCode;
		newProduct.description = iabProduct.description;
		newProduct.price = iabProduct.price;
		newProduct.formattedPrice = iabProduct.price;
		newProduct.brainzProductId = brainzProductId;
		newProduct.title = iabProduct.title;

		return newProduct;
	}

	private void OnProductListRequestFailed(string error)
	{
		TurnOffTryToLoadProductsFlag ();
		OnProductListRequestFailed(PlatformId, error);
	}
	
	private void OnPurchaseCancelled(string error)
	{
		OnPurchaseCancelled (PlatformId, error);
	}
	
	private void OnPurchaseFailed(string error)
	{
		OnPurchaseFailed (PlatformId, error);
	}
	
	private void OnPurchaseSuccedded(GooglePurchase data)
	{
		IAPProductID brainzProductId = IAPProductIDToBrainzProductId(data.productId);
		if ( brainzProductId != IAPProductID.None)
		{
			lastTransactionData = data;
			OnPurchaseSuccessful(brainzProductId, 1, PlatformId, CreateHashtableForPurchaseSuccedded(data, brainzProductId));
		}
		else
			Debug.LogWarning("An unrecognized IAP product was reported by Google IAB. Identifier=" + data.productId);
	}

	private Hashtable CreateHashtableForPurchaseSuccedded(GooglePurchase data, IAPProductID brainzProductId)
	{
		Hashtable table = new Hashtable();
		table.Add ("orderId" , data.orderId);
		table.Add ("brainzProductId" , brainzProductId.ToString ());
		table.Add ("purchaseTime" , data.purchaseTime.ToString ());
		table.Add ("productId" , data.productId);
		table.Add ("purchaseState" , data.purchaseState.ToString ());
		table.Add ("developerPayload" , data.developerPayload);
		table.Add ("receipt", data.orderId);
		table.Add ("price", realPrices[data.productId]);
		table.Add ("signature", data.signature);
		table.Add ("r_data", data.originalJson);

		return table;
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
}
#endif