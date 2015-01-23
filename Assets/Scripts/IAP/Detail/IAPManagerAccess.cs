using UnityEngine;
using System.Collections;
using IAP;

namespace IAP.Detail
{
	public class IAPManagerAccess : MonoBehaviour, IIAPManagerAccess
	{
		[SerializeField]
		private IAPManagerContainer iapManagerContainer;

		public IIAPManager IAPManager 
		{
			get {return iapManagerContainer.IAPManager;}
		}

		public static IIAPManagerAccess Instance
		{
			get;
			private set;
		}

		private void Awake()
		{
			if(Instance == null)
				Instance = this;
		}
	}
}