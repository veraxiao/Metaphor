using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using Google.Maps.Coord;
//using Google.Maps.Event;
//using Google.Maps.Examples.Shared;
//using Google.Maps.Examples;

public class UDPMessenger : MonoBehaviour
{
    // Start is called before the first frame update
    Thread thread;
    public string rfidData;
    public VR_Tag rfidTag = new VR_Tag();

    //public Player player;

    void Start()
    {
        //player = GameObject.Find("Player").GetComponent<Player>();
        //if (player == null)
        //Debug.Log("Cannot find player");

        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.IsBackground = true;
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {
        thread.Abort();
    }

    private async void ThreadMethod()
    {
        UdpClient udpSend = new UdpClient();
        //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11001);//IPEndPoint remoteIpEndPoint = null;
        //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.2.103"), 11001);
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("10.20.96.150"), 11001);
        string msg = "StartReading";
        byte[] packet = Encoding.UTF8.GetBytes(msg);
        await udpSend.SendAsync(packet, packet.Length, ep);

        //Creat a UdpClient object and specify 11000 as receive port
        UdpClient udpReceive = new UdpClient(11000);

        //Specify remote host machine, allowing all data sent by any IP port
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);//IPEndPoint remoteIpEndPoint = null;

        Debug.Log("Waiting for a client...");

        while (true)
        {
            //Listen for data under asynchronous mode and transfer them into UTP8-encoded string
            var result = await udpReceive.ReceiveAsync();
            rfidData = Encoding.UTF8.GetString(result.Buffer);

            this.OnRecieve(rfidData);

        }
    }

    private void OnRecieve(string rfidData)
    {
        Debug.Log("JSON Raw Data:" + rfidData);
        if(rfidData.Contains("EpcName"))
            rfidTag = rfidTag.ParsedFromJSON(rfidData);
        Debug.Log("Parsed Tag EPC:" + rfidTag.EpcName + " Lat: " + rfidTag.Latitude + " Lng:" + rfidTag.Longitude + " URL:" + rfidTag.BundleUrl + " Name:" + rfidTag.AssetName);
        //if (rfidData.Contains("EPC:0011")) player.playerRelativeOffset = new LatLng(-19.594984, -11.325444);
        //else if (rfidData.Contains("EPC:0012")) player.playerRelativeOffset = new LatLng(0, 0);

        //The latitude and longtitude should be contained in the rfidData;

    }
}
