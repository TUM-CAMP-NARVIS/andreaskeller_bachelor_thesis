using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrackingMessage : MessageBase
{
    public uint netId;
    public NetworkHash128 assetID;
    public Vector3 position;
    public Quaternion rotation;
    

    public override void Deserialize(NetworkReader reader)
    {
        netId = reader.ReadPackedUInt32();
        assetID = reader.ReadNetworkHash128();
        position = reader.ReadVector3();
        rotation = reader.ReadQuaternion();
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.WritePackedUInt32(netId);
        writer.Write(assetID);
        writer.Write(position);
        writer.Write(rotation);
    }
}
