using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MoralisUnity;
using MoralisUnity.Platform.Queries;
using MoralisUnity.Platform.Objects;



public class PlayerBalance : MoralisObject
{
    public float moon { get; set; }
    public float ren { get; set; }
    public float artifacts { get; set; }
    public float scrolls { get; set; }
    public float aether { get; set; }
    public float frost { get; set; }
    public float terra { get; set; }
    public float magma { get; set; }
    public float iron { get; set; }   


    public PlayerBalance() : base("PlayerBalance") {}
}


public class PlayerBalances : MonoBehaviour
{

    private MoralisQuery<PlayerBalance> _getAllItemsQuery;
    private MoralisLiveQueryCallbacks<PlayerBalance> _callbacks;

    // Start is called before the first frame update
    void Start()
    {

        GetItemsFromDB();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        public async void SubscribeToDatabaseEvents()
    {

        MoralisUser user = await Moralis.GetUserAsync();
        var playerAddress = user.authData["moralisEth"]["id"].ToString();

        Debug.Log("Player address: " + playerAddress);

        _getAllItemsQuery = await Moralis.GetClient().Query<PlayerBalance>();
        _getAllItemsQuery = _getAllItemsQuery.WhereEqualTo("ethAddress", playerAddress);

        _callbacks = new MoralisLiveQueryCallbacks<PlayerBalance>();
        _callbacks.OnConnectedEvent += (() => { Debug.Log("Connection Established."); });
        _callbacks.OnSubscribedEvent += ((requestId) => { Debug.Log($"Subscription {requestId} created."); });
        _callbacks.OnUnsubscribedEvent += ((requestId) => { Debug.Log($"Unsubscribed from {requestId}."); });
        _callbacks.OnCreateEvent += ((item, requestId) =>
        {
            Debug.Log("New item created on DB");
            //PopulateShopItem(item);
        });
        _callbacks.OnUpdateEvent += ((item, requestId) =>
        {
            Debug.Log("Item updated");
            //UpdateItem(item.objectId, item);
        });
        _callbacks.OnDeleteEvent += ((item, requestId) =>
        {
            Debug.Log("Item deleted from DB");
            //DeleteItem(item.objectId);
        });
        
        MoralisLiveQueryController.AddSubscription<PlayerBalance>("PlayerBalances", _getAllItemsQuery, _callbacks);
         GetItemsFromDB();
    }

        private async void GetItemsFromDB()
    {   
        IEnumerable<PlayerBalance> databaseItems = await _getAllItemsQuery.FindAsync();

        // stringify databaseItems then log

        string json = JsonUtility.ToJson(databaseItems);
        Debug.Log("Player balances:" + json);

        
        var databaseItemsList = databaseItems.ToList();
        if (!databaseItemsList.Any()) return;

        
    }
}
