using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAPManager : MonoBehaviour, IIAPManager 
{	
	[SerializeField]
	private DebugData debugData;

	private IIAPPlatform iaPPPlatform;
	public IIAPPlatform IAPPlatform
	{
		get{ return iaPPPlatform;}
	}
	
	public bool AreProductsLoaded
	{
		get { return IAPPlatform.Products.Count > 0; }
	}

	private void Awake()
	{
		SetIAPPPlatform (debugData.SimulateStore.IAPProducts.ConvertAll(p => p as IIAPProductData), debugData.TimeOutToStore);
	}

	private void SetIAPPPlatform (List<IIAPProductData> products, float timeOutToStore)
	{

		#if UNITY_EDITOR
		iaPPPlatform = new EditorIAPPlatform (products, timeOutToStore, debugData.DebugProducts, debugData.StoreDebugDelayInSeconds);
		#elif UNITY_IPHONE
		iaPPPlatform = new StoreKitPlatform(products, timeOutToStore);
		#elif UNITY_ANDROID
		iaPPPlatform = new GoogleIAPPlatform (products, timeOutToStore, new IAPGoogleConnector(), debugData.GooglePublicKey);
		#else
		iaPPPlatform = new DummyIAPPlatform(products, timeOutToStore);
		#endif
	}
	
	private void OnDestroy()
	{
		if (IAPPlatform != null)
			IAPPlatform.Dispose();
	}
	
	public bool PurchaseProduct(IAPProductID id)
	{
		Debug.Log("Trying to  purchase of " + id);
		if (IAPPlatform.CanMakePayments)
			return CanToPurchaseProduct (id);
		else
		{
			Debug.Log("Purchase of " + id + " failed because purchases are forbidden on this device.");
			return false;
		}
	}

	private bool CanToPurchaseProduct (IAPProductID id)
	{
		if (AreProductsLoaded) 
		{
			IAPPlatform.PurchaseProduct (id, 1);
			return true;
		}
		else 
		{
			Debug.Log ("Purchase of " + id + " failed because product list is not loaded yet.");
			return false;
		}
	}

	public void ConsumeProduct(IAPProductID id)
	{
		IAPPlatform.ConsumeProduct(id);
	}
}