using UnityEngine;
using System.Collections;

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
		SetIAPPPlatform ();
	}

	private void SetIAPPPlatform ()
	{
		#if UNITY_EDITOR
		iaPPPlatform = new EditorIAPPlatform ();
		#elif UNITY_IPHONE
		iaPPPlatform = new StoreKitPlatform();
		#elif UNITY_ANDROID
		iaPPPlatform = new GoogleIAPPlatform ();
		#else
		iaPPPlatform = new DummyIAPPlatform();
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