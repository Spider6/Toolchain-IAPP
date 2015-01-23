using UnityEngine;
using System.Collections;
using IAP;

namespace IAP.Detail
{
	public class StoreSettings : ScriptableObject, IStoreSettings
	{
		[SerializeField]
		private string googlePublicKey;

		[SerializeField]
		private float timeOutToStore;

		public string GooglePublicKey 
		{
			get { return googlePublicKey; }
		}
		
		public float TimeOutToStore 
		{
			get { return timeOutToStore; }
		}
	}
}