using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using IAP;

namespace IAP.Detail
{
	public class EditorIAPPlatform : DummyIAPPlatform
	{
		private float storeDebugDelayInSeconds;

		public EditorIAPPlatform (List<IIAPProductData> products, float timeOutToStore,
		                          List<IAPProduct> debugProducts, float storeDebugDelayInSeconds) : base(products, timeOutToStore)
		{
			dummyProducts = debugProducts;
			this.storeDebugDelayInSeconds = storeDebugDelayInSeconds;
		}

		public override void ConsumeProduct(string brainzProductId){}
		public override void Dispose(){}

		public override void PurchaseProduct(string brainzProductId, int quantity)
		{
			caller.StartCoroutine (PurchaseAsync(brainzProductId, quantity));
		}

		protected override void GetProductsDataFromStore ()
		{
			if(HasProducts)
				caller.StartCoroutine (ReceiveProductListAsync());
		}
		
		private IEnumerator ReceiveProductListAsync()
		{
			yield return new WaitForSeconds (storeDebugDelayInSeconds);
			foreach (IAPProduct product in Products)
			{
				SetCurrencyPrice(product.brainzProductId.ToString (), product.formattedPrice);
				CurrencyCode = product.currencyCode;
			}
			TurnOffTryToLoadProductsFlag ();
		    OnProductListReceived(PlatformId);
		}
		
		private IEnumerator PurchaseAsync(string brainzProductId, int quantity)
		{
			yield return new WaitForSeconds (storeDebugDelayInSeconds);

			Hashtable table = GetInfoPurchaseProduct (brainzProductId, quantity);
			OnPurchaseSuccessful(brainzProductId, quantity, PlatformId, table);
		}
	}
}