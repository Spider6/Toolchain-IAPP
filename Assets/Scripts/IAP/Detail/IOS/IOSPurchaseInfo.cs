using UnityEngine;
using System.Collections;
using IAP;

namespace IAP.Detail
{
	#if UNITY_IOS
	public class IOSPurchaseInfo : IIOSPurchaseInfo
	{
		public StoreKitTransaction PurchaseInfo
		{
			get;
			set;
		}

		public string ProductId
		{
			get {return PurchaseInfo.productIdentifier;}
		}
		public string TransactionId 
		{
			get {return PurchaseInfo.transactionIdentifier;}
		}

		public string Base64EncodedTransactionReceipt 
		{
			get {return PurchaseInfo.base64EncodedTransactionReceipt;}
		}

		public int Quantity 
		{
			get {return PurchaseInfo.quantity;}
		}
	}
	#endif
}
