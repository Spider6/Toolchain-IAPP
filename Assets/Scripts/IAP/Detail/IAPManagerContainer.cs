using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IAP;

namespace IAP.Detail
{
	public class IAPManagerContainer : MonoBehaviour 
	{	
		[SerializeField]
		private DebugStoreData debugData;

		[SerializeField]
		private StoreSettings storeSettings;

		private IAPManager iapManager;

		public IAPManager IAPManager 
		{
			get { return iapManager; }
		}

		private void Awake()
		{
			iapManager = new IAPManager();
			iapManager.Initialize (debugData, storeSettings, debugData.SimulateStore.IAPProducts.ConvertAll(p => p as IIAPProductData), Application.platform);
		}
		
		private void OnDestroy()
		{
			if (iapManager != null)
				iapManager.Dispose();
		}
	}
}