using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DummyIAPProduct
{
	public DummyIAPProduct (IAPProductID id)
	{
		packageName = id.ToString ();
	}
	public string packageName { get; private set; }
	public string productId { get{return packageName;} }
	public string orderId { get {return "rqeoiffaksjldj8490324";} }
	public string purchaseToken { get {return "98tuoigji4jtiojfkasjdpoifad989jfadofu90eie";} }
}

public class DummyIAPPlatform : IAPPlatformBase
{
	protected DummyIAPProduct lastTransactionData;	
	private List<IAPProduct> dummyProducts = new List<IAPProduct>();

	public override bool CanMakePayments
	{
		get { return true; }
	}

	public override List<IAPProduct> Products
	{
		get { return dummyProducts; }
	}

	public override IAPPlatformID PlatformId
	{
		get { return IAPPlatformID.Dummy; }
	}

	public override string StoreName
	{
		get { return IAPPlatformID.Dummy.ToString(); }
	}

	public DummyIAPPlatform (List<IAPProductData> products) : base(products){}
	public override void ValidatePedingPurchases (){}
	public override void ConsumeProduct(IAPProductID id) {}

	public override void PurchaseProduct(IAPProductID id, int quantity)
	{
		Hashtable table = GetInfoPurchaseProduct (id, quantity);
		OnPurchaseSuccessful(id, quantity, PlatformId, table);
	}

	public override void Dispose()
	{
		dummyProducts.Clear();
	}
	
	public override Hashtable GetLastTransactionData()
	{
		Hashtable transactionData = new Hashtable();
		transactionData.Add("productIdentifier", lastTransactionData.productId);
		transactionData.Add("transactionIdentifier", lastTransactionData.orderId);
		transactionData.Add("base64EncodedTransactionReceipt", lastTransactionData.purchaseToken);
		transactionData.Add("quantity", 1);
		
		IAPProduct product = Products.Find(p => BrainzProductIdToIAPProductId(p.brainzProductId) == lastTransactionData.productId);
		if(product != null)
		{
			transactionData.Add("price", product.price);
			transactionData.Add("currencyCode", product.currencyCode);
		}
		return transactionData;
	}

	protected override void GetProductsDataFromStore ()
	{
		TurnOffTryToLoadProductsFlag ();
		dummyProducts.Clear();
		OnProductListRequestFailed(PlatformId, "failed!!");
	}

	protected Hashtable GetInfoPurchaseProduct (IAPProductID brainzProductId, int quantity)
	{
		DummyIAPProduct data = GetDummyIAPProduct (brainzProductId);
		lastTransactionData = data;
		Hashtable table = new Hashtable();
		table.Add ("orderId" , data.orderId);
		table.Add ("packageName" , data.packageName);
		table.Add ("productId" , data.productId);
		table.Add ("purchaseToken" , data.purchaseToken);
		table.Add ("pack", brainzProductId.ToString ());
		table.Add ("receipt", data.orderId);
		return table;
	}

	private DummyIAPProduct GetDummyIAPProduct (IAPProductID id)
	{
		return new DummyIAPProduct (id);
	}
}