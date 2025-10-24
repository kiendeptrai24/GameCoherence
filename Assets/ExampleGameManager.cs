using System.Collections.Generic;
using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;

public class ExampleGameManager : MonoBehaviour
{
    public CoherenceBridge Bridge;
    [SerializeField] CoherenceClientConnection simulatorConnection;
    [SerializeField] CoherenceClientConnection myConnection;
    [SerializeField] CoherenceClientConnection selectedConnection;
    [SerializeField] CoherenceClientConnection otherConnection;
    [SerializeField] CoherenceClientConnection allConnection;
    [SerializeField] IEnumerable<CoherenceClientConnection> otherConnections;
    [SerializeField] IEnumerable<CoherenceClientConnection> allConnections;
    [SerializeField] int clientCount;
    [SerializeField] ConnectionType connectionType;
    [SerializeField] bool isMine;
    [SerializeField] GameObject connectionGameObject;
    
    void Start()
    {

        // Raised whenever a new connection is made (including the local one).
        Bridge.ClientConnections.OnCreated += connection =>
        {
            Debug.Log($"Connection #{connection.ClientId} " +
                      $"of type {connection.Type} created.");
        };

        // Raised whenever a connection is destroyed.
        Bridge.ClientConnections.OnDestroyed += connection =>
        {
            Debug.Log($"Connection #{connection.ClientId} " +
                      $"of type {connection.Type} destroyed.");
        };

        // Raised when all initial connections have been synced.
        Bridge.ClientConnections.OnSynced += connectionManager =>
        {
            Debug.Log($"ClientConnections are now ready to be used.");
        };
        DontDestroyOnLoad(gameObject);
    }
    
    void Update()
    {
        // IMPORTANT: All of the connection retrieving calls may return null 
        // if the connection system was not turned on, not initialized yet,
        // or simply if the connection was not found.
        
        // Specifies how many clients are in this session (Room or World).
        int clientCount = Bridge.ClientConnections.ClientConnectionCount;
        
        // Returns connection objects for all connections in this session.
         allConnections
            = Bridge.ClientConnections.GetAll();

        // Returns all connections except for the local one.
         otherConnections
            = Bridge.ClientConnections.GetOther();

        // Returns connection object of the local user.
         myConnection
            = Bridge.ClientConnections.GetMine();

        // Returns connection object of the Simulator (if one is connected).
         simulatorConnection
            = Bridge.ClientConnections.GetSimulator();

        // Retrieves a connection by its ClientID.
         selectedConnection
            = Bridge.ClientConnections.Get(myConnection.ClientId);

        // Retrieves a connection by its EntityID (warning: requires
        // a connection Prefab with a CoherenceSync attached).
        if (myConnection.Sync != null)
        {
            selectedConnection
                = Bridge.ClientConnections.Get(myConnection.EntityId);
        }

        // Specifies if this is a Client or a Simulator connection.
         connectionType = selectedConnection.Type;

        // Specifies if this is a local connection (belonging to the
        // local user).
         isMine = selectedConnection.IsMyConnection;

        // Returns a GameObject associated with this connection.
        // Applicable only if connection Prefabs are used.
         connectionGameObject = selectedConnection.GameObject;
    }
}