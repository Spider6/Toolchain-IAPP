using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using IAP;

namespace IAP.Detail
{
	public class DummyIAPProduct
	{
		public DummyIAPProduct (string brainzProductId)
		{
			BrainzProductId = brainzProductId;
		}

		public string BrainzProductId { get; private set; }
		public string IAPProductId { get{return BrainzProductId;} }
		public string OrderId { get {return "rqeoiffaksjldj8490324";} }
		public string PurchaseToken { get {return "98tuoigji4jtiojfkasjdpoifad989jfadofu90eie";} }
	}

	public class DummyIAPPlatform : IAPPlatformBase
	{
		protected DummyIAPProduct lastTransactionData;	
		protected List<IAPProduct> dummyProducts;

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

		public override void ValidatePedingPurchases (){}
		public override void ConsumeProduct(string brainzProductId) {}

		public DummyIAPPlatform (List<IIAPProductData> products, float timeOutToStore) : base(products, timeOutToStore)
		{
			dummyProducts = new List<IAPProduct>();
		}

		public override void PurchaseProduct(string brainzProductId, int quantity)
		{
			Hashtable table = GetInfoPurchaseProduct (brainzProductId, quantity);
			OnPurchaseSuccessful(brainzProductId, quantity, PlatformId, table);
		}

		public override void Dispose()
		{
			dummyProducts.Clear();
		}
		
		public override Hashtable GetLastTransactionData()
		{
			Hashtable transactionData = new Hashtable();
			transactionData.Add("productIdentifier", lastTransactionData.IAPProductId);
			transactionData.Add("transactionIdentifier", lastTransactionData.OrderId);
			transactionData.Add("base64EncodedTransactionReceipt", lastTransactionData.PurchaseToken);
			transactionData.Add("quantity", 1);
			
			IAPProduct product = Products.Find(p => BrainzProductIdToIAPProductId(p.brainzProductId) == lastTransactionData.IAPProductId);
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

		protected Hashtable GetInfoPurchaseProduct (string brainzProductId, int quantity)
		{
			DummyIAPProduct data = GetDummyIAPProduct (brainzProductId);
			lastTransactionData = data;
			Hashtable table = new Hashtable();
			table.Add ("orderId" , data.OrderId);
			table.Add ("brainzProductId" , data.BrainzProductId);
			table.Add ("productId" , data.IAPProductId);
			table.Add ("purchaseToken" , data.PurchaseToken);
			table.Add ("pack", brainzProductId);
			table.Add ("receipt", data.OrderId);
			return table;
		}

		private DummyIAPProduct GetDummyIAPProduct (string brainzProductId)
		{
			return new DummyIAPProduct (brainzProductId);
		}
	}
}