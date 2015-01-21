using UnityEngine;
using System.Collections;

public class GooglePurchaseInfo : IGooglePurchaseInfo
{
	public GooglePurchase PurchaseInfo
	{
		get;
		set;
	}

	public string OrderId 
	{
		get { return PurchaseInfo.orderId; }
	}

	public string ProductId 
	{
		get { return PurchaseInfo.productId; }
	}

	public string PurchaseToken 
	{
		get { return PurchaseInfo.purchaseToken; }
	}

	public long PurchaseTime 
	{
		get { return PurchaseInfo.purchaseTime; }
	}
	public string Signature 
	{
		get { return PurchaseInfo.signature; }
	}

	public string OriginalJson 
	{
		get { return PurchaseInfo.originalJson; }
	}

	public string PurchaseState 
	{
		get { return PurchaseInfo.purchaseState.ToString(); }
	}

	public string DeveloperPayload 
	{
		get { return PurchaseInfo.developerPayload; }
	}
}
