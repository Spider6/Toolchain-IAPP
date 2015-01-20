using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class IABUIManager : MonoBehaviourGUI
{
	#if UNITY_ANDROID
	void OnGUI()
	{
		beginColumn();
		
		if( GUILayout.Button( "Initialize IAB" ) )
		{
			var key = "7cfb1b0ec10d4e6b9d29c2312e073ac7";
			key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyI70qk86LJd/obggjgfETOccmXs5BmFmppuSad2EQ0CkJuysTb9QsvPj2d/vYxWQKdtx/fEIdG9AO6nyJ8q1u1fuZdRPT1ImZOxT9bAYNJuqWXGKdxcSKQPQ0gcNQ+1FA5z7+GwDv43QqJrhftANe4Td/nIRxex9FE7Eb0FK4XDRFn1nDNCwf02jAmKfz2wUumsKUdspfzOR0ZCo6A659nzg4/86tB0GdZ6rM9XfNviKRNRlMe4gkcIcbY2yXR5bIH71GNjWrQ+UwJNpJK9AlYyWt9YfPrXV9lGSaTaJV7FpXRoeMjsV3O3GrP+NfFkLjKjSo+0zcu5cVrh8MeRPrwIDAQAB";
			GoogleIAB.init( key );
		}
		
		
		if( GUILayout.Button( "Query Inventory" ) )
		{
			// enter all the available skus from the Play Developer Console in this array so that item information can be fetched for them
			//var skus = new string[] { "com.prime31.testproduct", "android.test.purchased", "com.prime31.managedproduct", "com.prime31.testsubscription" };
			var skus = new string[] {"com.gamevilusa.markofthedragon.android.google.global.normal.500ambers","com.gamevilusa.markofthedragon.android.google.global.normal.1100ambers","com.gamevilusa.markofthedragon.android.google.global.normal.2400ambers"};
			GoogleIAB.queryInventory( skus );
		}
		
		
		if( GUILayout.Button( "Are subscriptions supported?" ) )
		{
			Debug.Log( "subscriptions supported: " + GoogleIAB.areSubscriptionsSupported() );
		}
		
		
		if( GUILayout.Button( "Purchase Test Product" ) )
		{
			GoogleIAB.purchaseProduct( "com.gamevilusa.markofthedragon.android.google.global.normal.500ambers" );
		}
		
		
		if( GUILayout.Button( "Consume Test Purchase" ) )
		{
			GoogleIAB.consumeProduct( "android.test.purchased" );
		}
		
		
		if( GUILayout.Button( "Test Unavailable Item" ) )
		{
			GoogleIAB.purchaseProduct( "com.gamevilusa.markofthedragon.android.google.global.normal.500ambers" );
		}
		
		
		endColumn( true );
		
		
		if( GUILayout.Button( "Purchase Real Product" ) )
		{
			GoogleIAB.purchaseProduct( "com.gamevilusa.markofthedragon.android.google.global.normal.500ambers", "payload that gets stored and returned" );
		}
		
		
		if( GUILayout.Button( "Purchase Real Subscription" ) )
		{
			GoogleIAB.purchaseProduct( "com.prime31.testsubscription", "subscription payload" );
		}
		
		
		if( GUILayout.Button( "Consume Real Purchase" ) )
		{
			GoogleIAB.consumeProduct( "com.prime31.testproduct" );
		}
		
		
		if( GUILayout.Button( "Enable High Details Logs" ) )
		{
			GoogleIAB.enableLogging( true );
		}
		
		
		if( GUILayout.Button( "Consume Multiple Purchases" ) )
		{
			var skus = new string[] { "com.prime31.testproduct", "android.test.purchased" };
			GoogleIAB.consumeProducts( skus );
		}
		
		endColumn();
	}
	#endif
}
