using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAPManager : MonoBehaviour, IIAPManager 
{	
	public IIAPPlatform iaPPPlatform;
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
		SetIAPPPlatform (new List<IAPProductData>());
	}

	private void SetIAPPPlatform (List<IAPProductData> products)
	{
		#if UNITY_EDITOR
		iaPPPlatform = new EditorIAPPlatform (products, new List<IAPProduct>());
		#elif UNITY_IPHONE
		iaPPPlatform = new StoreKitPlatform(products);
		#elif UNITY_ANDROID
		iaPPPlatform = new GoogleIAPPlatform (products);
		#else
		iaPPPlatform = new DummyIAPPlatform(products);
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