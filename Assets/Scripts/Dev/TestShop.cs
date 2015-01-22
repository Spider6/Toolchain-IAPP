using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestShop : MonoBehaviour 
{
	[SerializeField]
	private Text textBox;

	[SerializeField]
	private IAPManagerContainer iapManager;


	private IIAPPlatform CurrentIAPPlatform
	{
		get{ return iapManager.IAPPlatform; }
	}

	private void Start()
	{
		CurrentIAPPlatform.ProductListReceivedDelegate += OnProductListReceived;
		CurrentIAPPlatform.ProductListRequestFailedDelegate += OnProductListRequestFiled;
		CurrentIAPPlatform.PurchaseSuccessfulDelegate += OnPurchaseSuccessful;
		CurrentIAPPlatform.PurchaseFailedDelegate += OnPurchaseFailed;

		if(Application.platform == RuntimePlatform.Android)
		{
			(CurrentIAPPlatform as GoogleIAPPlatform).BillingSupportedDelegate += OnBillingSupported;
			(CurrentIAPPlatform as GoogleIAPPlatform).BillingNotSupportedSupportedDelegate += OnBillingNotSupported;
		}
	}

	public void OnProductListReceived(IAPPlatformID platformId)
	{
		textBox.text += "Product list received \n";
		foreach(IAPProduct product in CurrentIAPPlatform.Products)
		{
			textBox.text += "BrainzProductId: " + product.brainzProductId + "\n";
			textBox.text += "CurrencyCode: " + product.currencyCode + "\n";
			textBox.text += "CurrencySymbol: " + product.currencySymbol + "\n";
			textBox.text += "Description: " + product.description + "\n";
			textBox.text += "FormattedPrice: " + product.formattedPrice + "\n";
			textBox.text += "Price: " + product.price + "\n";
			textBox.text += "Title: " + product.title + "\n";
			textBox.text += "=========================================================================\n";
		}
	}

	public void OnBillingSupported()
	{
		textBox.text += "Billing Supported\n";
	}

	private void OnBillingNotSupported(string error)
	{
		textBox.text += "Billing No Supported Error: " + error + "\n";
	}

	private void OnProductListRequestFiled(IAPPlatformID platformId, string error)
	{
		textBox.text += "Product list request filed: Platform: " + platformId.ToString() + " Error: " + error;
	}

	public void RequestProducts()
	{
		textBox.text = string.Empty;
		CurrentIAPPlatform.RequestAllProductData(this);
	}

	public void PurchaseProduct()
	{
		iapManager.PurchaseProduct("PouchOfJade");
	}

	private void OnPurchaseSuccessful(string brainzProductId, int quantity, IAPPlatformID platformId, Hashtable transactionData)
	{
		textBox.text += "\n=============Product Purchase=============\n";
		textBox.text += "BrainzId: " + brainzProductId +  "\n";
		textBox.text += "Quantity: " + quantity +  "\n";
		textBox.text += "PlatformId: " + platformId +  "\n";
		textBox.text += "TransactionData:\n";
		foreach (DictionaryEntry pair in transactionData)
			textBox.text += "- " + pair.Key.ToString() + " : " + pair.Value.ToString() + "\n";

		textBox.text += "=================End Purchase===============\n";
	}

	private void OnPurchaseFailed(IAPPlatformID platformId, string error)
	{
		textBox.text += "Purchase product filed: Platform: " + platformId.ToString() + " Error: " + error;
	}
}
