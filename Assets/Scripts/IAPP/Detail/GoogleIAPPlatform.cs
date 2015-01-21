using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GoogleIAPPlatform : IAPPlatformBase
{
	public static event Action BillingSupportedEvent;
	public static event Action<string> BillingNotSupportedSupportedEvent;

	private List<IAPProduct> products = new List<IAPProduct>();
	private List<IGooglePurchaseInfo> pedingPurchases = new List<IGooglePurchaseInfo>();
	private IGooglePurchaseInfo lastTransactionData;
	private bool supportsBilling = false;
	private Dictionary<string,string> realPrices = new Dictionary<string,string> ();
	private IIAPGoogleConnector connector;

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

	public GoogleIAPPlatform(List<IIAPProductData> products, float timeOutToStore, IIAPGoogleConnector connector, string publicKey): base(products, timeOutToStore)
	{
		this.connector = connector;
		this.connector.Initialize(publicKey);
		RegisterCallbacks();
	}

	public override Hashtable GetLastTransactionData()
	{
		Hashtable transactionData = new Hashtable();
		transactionData.Add("productIdentifier", lastTransactionData.ProductId);
		transactionData.Add("transactionIdentifier", lastTransactionData.OrderId);
		transactionData.Add("base64EncodedTransactionReceipt", lastTransactionData.PurchaseToken);
		transactionData.Add("quantity", 1);
		
		IAPProduct product = products.Find(p => BrainzProductIdToIAPProductId(p.brainzProductId) == lastTransactionData.ProductId);
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
		connector.PurchaseProduct (BrainzProductIdToIAPProductId(brainzProductId), Guid.NewGuid().ToString ());
	}
	
	public override void ConsumeProduct (IAPProductID brainzProductId)
	{
		RemovePedingProductToPurchase (brainzProductId);
		connector.ConsumeProduct(BrainzProductIdToIAPProductId(brainzProductId));
	}

	public override void ValidatePedingPurchases ()
	{
		foreach (IGooglePurchaseInfo purchaseProduct in pedingPurchases)
			OnPurchaseSuccedded (purchaseProduct);
	}

	protected override void GetProductsDataFromStore ()
	{
		products.Clear();
		connector.GetProducts(GetAllIAPProductId());
	}

	private void RemovePedingProductToPurchase (IAPProductID brainzProductId)
	{
		IGooglePurchaseInfo currentProduct;
		IGooglePurchaseInfo product = pedingPurchases.Find(p => p.ProductId == BrainzProductIdToIAPProductId (brainzProductId));
		currentProduct = product;
		
		if (currentProduct != null)
			pedingPurchases.Remove (currentProduct);
	}

	private void RegisterCallbacks()
	{
		connector.BillingSupportedDelegate += OnBillingEvent;
		connector.BillingNotSupportedDelegate += OnBillingNotSupportedEvent;
		connector.ProductListReceivedDelegate += OnIAPProductListReceived;
		connector.ProductListRequestFailedDelegate += OnProductListRequestFailed;
		connector.PurchaseSucceededDelegate += OnPurchaseSuccedded;
		connector.PurchaseFailedDelegate += OnPurchaseFailed;
	}
	
	private void UnregisterCallbacks()
	{
		connector.BillingSupportedDelegate -= OnBillingEvent;
		connector.BillingNotSupportedDelegate -= OnBillingNotSupportedEvent;
		connector.ProductListReceivedDelegate -= OnIAPProductListReceived;
		connector.ProductListRequestFailedDelegate -= OnProductListRequestFailed;
		connector.PurchaseSucceededDelegate -= OnPurchaseSuccedded;
		connector.PurchaseFailedDelegate -= OnPurchaseFailed;
	}

	private void OnIAPProductListReceived(List<IGooglePurchaseInfo> purchasesList, List<IGoogleProductInfo> iabProductList)
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

	private void FillProductList (List<IGoogleProductInfo> iabProductList)
	{
		foreach (IGoogleProductInfo iabProduct in iabProductList) 
		{
			Debug.Log (iabProduct.ToString () + " this is the products to purchase");
			IAPProductID brainzProductId = IAPProductIDToBrainzProductId (iabProduct.ProductId);
			if (brainzProductId != IAPProductID.None)
				AddIAPProduct (iabProduct, brainzProductId);
			else
				Debug.LogWarning ("An unrecognized IAP product was reported by Google Store. Identifier: " + iabProduct.ProductId);
		}
	}

	private void AddIAPProduct (IGoogleProductInfo iabProduct, IAPProductID brainzProductId)
	{
		if (iabProduct.PriceCurrencyCode != null && iabProduct.Description != null && iabProduct.Price != null && iabProduct.Title != null) 
		{
			IAPProduct newProduct = CreateIAPProduct (iabProduct, brainzProductId);
			products.Add (newProduct);
			UpdateProductData (iabProduct, newProduct);
			Debug.Log ("Loaded product: " + newProduct.ToString ());
		}
		else
			Debug.LogWarning ("IAP product ignored because it contains null data: " + brainzProductId);
	}

	private void UpdateProductData (IGoogleProductInfo iabProduct, IAPProduct newProduct)
	{
		CurrencyCode = newProduct.currencyCode;
		if (!realPrices.ContainsKey (iabProduct.ProductId))
			realPrices.Add (iabProduct.ProductId, iabProduct.Price);
		SetCurrencyPrice (iabProduct.ProductId, iabProduct.Price);
	}

	private IAPProduct CreateIAPProduct(IGoogleProductInfo iabProduct, IAPProductID brainzProductId)
	{
		IAPProduct newProduct = new IAPProduct();
		newProduct.currencySymbol = iabProduct.PriceCurrencyCode;
		newProduct.currencyCode = iabProduct.PriceCurrencyCode;
		newProduct.description = iabProduct.Description;
		newProduct.price = iabProduct.Price;
		newProduct.formattedPrice = iabProduct.Price;
		newProduct.brainzProductId = brainzProductId;
		newProduct.title = iabProduct.Title;

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
	
	private void OnPurchaseSuccedded(IGooglePurchaseInfo data)
	{
		IAPProductID brainzProductId = IAPProductIDToBrainzProductId(data.ProductId);
		if ( brainzProductId != IAPProductID.None)
		{
			lastTransactionData = data;
			OnPurchaseSuccessful(brainzProductId, 1, PlatformId, CreateHashtableForPurchaseSuccedded(data, brainzProductId));
		}
		else
			Debug.LogWarning("An unrecognized IAP product was reported by Google IAB. Identifier=" + data.ProductId);
	}

	private Hashtable CreateHashtableForPurchaseSuccedded(IGooglePurchaseInfo data, IAPProductID brainzProductId)
	{
		Hashtable table = new Hashtable();
		table.Add ("orderId" , data.OrderId);
		table.Add ("brainzProductId" , brainzProductId.ToString ());
		table.Add ("purchaseTime" , data.PurchaseTime.ToString ());
		table.Add ("productId" , data.ProductId);
		table.Add ("purchaseState" , data.PurchaseState);
		table.Add ("developerPayload" , data.DeveloperPayload);
		table.Add ("receipt", data.OrderId);
		table.Add ("price", realPrices[data.ProductId]);
		table.Add ("signature", data.Signature);
		table.Add ("r_data", data.OriginalJson);

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