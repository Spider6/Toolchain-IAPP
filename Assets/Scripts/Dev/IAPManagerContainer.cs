using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAPManagerContainer : MonoBehaviour 
{	
	[SerializeField]
	private DebugStoreData debugData;

	[SerializeField]
	private StoreSettings storeSettings;

	private IAPManager manager;
	
	public IIAPPlatform IAPPlatform
	{
		get{ return manager.IAPPlatform;}
	}
	
	public bool AreProductsLoaded
	{
		get { return manager.AreProductsLoaded; }
	}

	public bool PurchaseProduct(string brainzProductId)
	{
		return manager.PurchaseProduct(brainzProductId);
	}

	public void ConsumeProduct(string brainzProductId)
	{
		manager.ConsumeProduct(brainzProductId);
	}

	private void Awake()
	{
		manager = new IAPManager();
		manager.Initialize (debugData, storeSettings, debugData.SimulateStore.IAPProducts.ConvertAll(p => p as IIAPProductData), Application.platform);
	}
	
	private void OnDestroy()
	{
		if (manager != null)
			manager.Dispose();
	}
}