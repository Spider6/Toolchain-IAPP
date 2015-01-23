using UnityEngine;
using System.Collections;
using IAP.Detail;

namespace IAP
{
	public interface IIAPManagerAccess 
	{
		IIAPManager IAPManager {get;}
	}

	public static class IAPDataManager
	{
		public static IIAPManagerAccess Instance
		{
			get{ return IAPManagerAccess.Instance; }
		}
	}
}
