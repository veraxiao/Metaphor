using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class BundleWebLoader : MonoBehaviour
{
    public string bundleUrl;
    //public string assetName = "";
    public UDPMessenger rfidMessenger;
    //public List<GameObject> loadedAsset;
    public List<String> readTags = new List<string>();
    public UnityWebRequest webBundle;
    public AssetBundle remoteAssetBundle;
    //public Regex rx = new Regex(@"EPC:(\S+)"); //Regular expression to match RFID tag's EPC in each RFID data.
    //public MatchCollection tagEPC;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Load());
        rfidMessenger = GameObject.Find("UDPMessenger").GetComponent<UDPMessenger>();
        if (rfidMessenger == null)
        {
            Debug.LogError("Failed to find RFID Messenger!");
        }

        bundleUrl = ""; //The actual asset bundle url will be contained from RFID data.
        //loaderCalled = false;
    }

    void Update()
    {
        //tagEPC = rx.Matches(rfidMessenger.rfidData);   //Extract RFID tag EPC in each data
        //foreach (Match tag in tagEPC)
        //{
        //Debug.Log(tag);
        if (readTags.Contains(rfidMessenger.rfidTag.EpcName) == false && rfidMessenger.rfidTag.AssetName != "")
        {

            if (bundleUrl == "" && rfidMessenger.rfidTag.BundleUrl != null)
            {
                bundleUrl = rfidMessenger.rfidTag.BundleUrl;
                //Debug.Log("BundleURL set to " + bundleUrl);
            }
            if (bundleUrl != "")
            {
                StartCoroutine(Load(rfidMessenger.rfidTag.AssetName));
                readTags.Add(rfidMessenger.rfidTag.EpcName);
            }
        }

        //}

    }

    public IEnumerator Load(string assetName)
    {
        //Load dynamic web asset from remote web server, according to given asset name.
        if (webBundle == null)
        {
            webBundle = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
            yield return webBundle.SendWebRequest();
            remoteAssetBundle = (webBundle.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        }

        if (remoteAssetBundle == null)
        {
            Debug.LogError("Failed to download AssetBundle!");
            yield break;
        }

        GameObject loadedAsset = remoteAssetBundle.LoadAsset<GameObject>(assetName);
        Instantiate(loadedAsset);

        //remoteAssetBundle.Unload(false);

    }

}
