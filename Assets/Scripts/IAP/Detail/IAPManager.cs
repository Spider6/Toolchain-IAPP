using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IAP;

namespace IAP.Detail
{
	public class IAPManager : IIAPManager
	{
		private IIAPPlatform iaPPPlatform;

		public IIAPPlatform IAPPlatform
		{
			get{ return iaPPPlatform;}
		}
		
		public bool AreProductsLoaded
		{
			get { return IAPPlatform.Products.Count > 0; }
		}

		public void Initialize(IDebugStoreData debugData, IStoreSettings storeSettings, List<IIAPProductData> iapProductList, RuntimePlatform currentPlatform)
		{
			SetIAPPPlatform (iapProductList, currentPlatform, debugData, storeSettings);
		}
		
		private void SetIAPPPlatform (List<IIAPProductData> products, RuntimePlatform currentPlatform, IDebugStoreData debugData, IStoreSettings storeSettings)
		{
			Debug.Log("Current Platform: " + currentPlatform.ToString());
			switch(currentPlatform)
			{
			case RuntimePlatform.Android: 
				iaPPPlatform = new GoogleIAPPlatform (products, storeSettings.TimeOutToStore, new IAPGoogleConnector(), storeSettings.GooglePublicKey);
				break;
				
			case RuntimePlatform.IPhonePlayer:
				iaPPPlatform = new IOSPlatform(products, storeSettings.TimeOutToStore, new IAPIOSConnector());
				break;
				
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				iaPPPlatform = new EditorIAPPlatform (products, storeSettings.TimeOutToStore, debugData.DebugProducts, debugData.StoreDebugDelayInSeconds);
				break;
				
			default:
				iaPPPlatform = new DummyIAPPlatform(products, storeSettings.TimeOutToStore);
				break;
			}
		}
		
		public void Dispose()
		{
			if (IAPPlatform != null)
				IAPPlatform.Dispose();
		}
		
		public bool PurchaseProduct(string brainzProductId)
		{
			Debug.Log("Trying to  purchase of " + brainzProductId);
			if (IAPPlatform.CanMakePayments)
				return CanToPurchaseProduct (brainzProductId);
			else
			{
				Debug.Log("Purchase of " + brainzProductId + " failed because purchases are forbidden on this device.");
				return false;
			}
		}
		
		public void ConsumeProduct(string brainzProductId)
		{
			IAPPlatform.ConsumeProduct(brainzProductId);
		}
		
		private bool CanToPurchaseProduct (string brainzProductId)
		{
			if (AreProductsLoaded) 
			{
				IAPPlatform.PurchaseProduct (brainzProductId, 1);
				return true;
			}
			else 
			{
				Debug.Log ("Purchase of " + brainzProductId + " failed because product list is not loaded yet.");
				return false;
			}
		}
	}
}