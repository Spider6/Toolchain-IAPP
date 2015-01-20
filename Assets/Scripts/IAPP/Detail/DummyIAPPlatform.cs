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
	private List<IAPProduct> products = new List<IAPProduct>();
	protected override string Package1{ get{ return IAPProductID.AmberPack1.ToString ();} }
	protected override string Package2{ get{ return IAPProductID.AmberPack2.ToString ();} }
	protected override string Package3{ get{ return IAPProductID.AmberPack3.ToString ();} }
	protected override string Package4{ get{ return IAPProductID.AmberPack4.ToString ();} }
	protected override string Package5{ get{ return IAPProductID.AmberPack5.ToString ();} }

	protected override string SpecialPackage1{ get{ return IAPProductID.SpecialPack1.ToString ();} }
	protected override string SpecialPackage2{ get{ return IAPProductID.SpecialPack2.ToString ();} }
	protected override string SpecialPackage3{ get{ return IAPProductID.SpecialPack3.ToString ();} }
	protected override string SpecialPackage4{ get{ return IAPProductID.SpecialPack4.ToString ();} }

	protected DummyIAPProduct lastTransactionData;	
	#region IIAPPlatform implementation
	public override event Action<IAPPlatformID> ProductListReceived;
	public override event Action<IAPPlatformID, string /*error*/> ProductListRequestFailed;
	public override event Action<IAPProductID /*id*/, int /*quantity*/, IAPPlatformID, Hashtable /* transactionData */> PurchaseSuccessful;
	public override event Action<IAPPlatformID, string /*error*/> PurchaseFailed;
	public override event Action<IAPPlatformID, string /*error*/> PurchaseCancelled;
	public override void ValidatePedingPurchases (){}

	public override bool CanMakePayments
	{
		get { return true; }
	}

	public override List<IAPProduct> Products
	{
		get { return products; }
	}

	public override IAPPlatformID ID
	{
		get { return IAPPlatformID.Dummy; }
	}

	public override string StoreName
	{
		get { return IAPPlatformID.Dummy.ToString(); }
	}

	public DummyIAPPlatform ()
	{
		this.CreatePackagesInfo ();
	}

	protected override void GetProductsDataFromStore ()
	{
		TurnOffTryToLoadProductsFlag ();
		products.Clear();
		
		if (ProductListRequestFailed != null)
			ProductListRequestFailed( ID, "failed!!" );
	}
	
	public override void PurchaseProduct(IAPProductID id, int quantity)
	{
		if (PurchaseSuccessful != null)
		{
			Hashtable table = GetInfoPurchaseProduct (id, quantity);
			PurchaseSuccessful(id, quantity, ID, table);
		}
	}

	protected Hashtable GetInfoPurchaseProduct (IAPProductID id, int quantity)
	{
		DummyIAPProduct data = GetDummyIAPProduct (id);
		lastTransactionData = data;
		Hashtable table = new Hashtable();
		table.Add ("orderId" , data.orderId);
		table.Add ("packageName" , data.packageName);
		table.Add ("productId" , data.productId);
		table.Add ("purchaseToken" , data.purchaseToken);
		table.Add ("pack", id.ToString ());
		table.Add ("receipt", data.orderId);
		return table;
	}

	private DummyIAPProduct GetDummyIAPProduct (IAPProductID id)
	{
		return new DummyIAPProduct (id);
	}

	public override void ConsumeProduct(IAPProductID id) {}
	public override void Dispose()
	{
		products.Clear();
	}

	public override Hashtable GetLastTransactionData()
	{
		Hashtable transactionData = new Hashtable();
		transactionData.Add("productIdentifier", lastTransactionData.productId);
		transactionData.Add("transactionIdentifier", lastTransactionData.orderId);
		transactionData.Add("base64EncodedTransactionReceipt", lastTransactionData.purchaseToken);
		transactionData.Add("quantity", 1);

		foreach(IAPProduct product in Products)
		{
			if ( IAPProductIDToString(product.productIdentifier) == lastTransactionData.productId)
			{
				transactionData.Add("price", product.price);
				transactionData.Add("currencyCode", product.currencyCode);
				break;
			}
		}
		
		return transactionData;
	}
	#endregion // IIAPPlatform implementation
}