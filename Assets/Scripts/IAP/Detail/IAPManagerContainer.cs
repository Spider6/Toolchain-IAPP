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

		[SerializeField]
		private List<IAPProductInput> idProductList;

		private IAPManager iapManager;

		public IAPManager IAPManager 
		{
			get { return iapManager; }
		}

		public void Initialize()
		{
			iapManager = null;
			iapManager = new IAPManager();
			iapManager.Initialize (debugData, storeSettings, GetIapSimulate(), Application.platform);
		}

		private List<IIAPProductData> GetIapSimulate()
		{
			IAPProductData[] arrayProduct = new IAPProductData[debugData.SimulateStore.IAPProducts.Count];
			debugData.SimulateStore.IAPProducts.CopyTo(arrayProduct);
			List<IAPProductData> iAPProducts = new List<IAPProductData>();
			iAPProducts.AddRange(arrayProduct);
			foreach(IAPProductInput product in idProductList)
			{
				int index = iAPProducts.FindIndex(p => p.BrainzProductId == product.BrainzProductId);
				if(product.IdProduct == string.Empty)
					iAPProducts.RemoveAt(index);
				else
					iAPProducts[index].IAPProductId = product.IdProduct;
			}
			Debug.Log("Result: " + iAPProducts.Count);
			return iAPProducts.ConvertAll(p => p as IIAPProductData);
		}
		
		private void OnDestroy()
		{
			if (iapManager != null)
				iapManager.Dispose();
		}
	}
}