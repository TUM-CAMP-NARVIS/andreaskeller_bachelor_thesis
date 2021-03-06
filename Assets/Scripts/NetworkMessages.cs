﻿using Mirror;
using UnityEngine;
public struct TrackedObjectMessage : IMessageBase
{
    public enum Type { ViveTracker, ViveController, HMD };
    public short id;
    public Type type;
    public Vector3 position;
    public Quaternion rotation;


    public TrackedObjectMessage(short id, Type type, Vector3 position, Quaternion rotation)
    {
        this.id = id;
        this.type = type;
        this.position = position;
        this.rotation = rotation;
    }

    public void Deserialize(NetworkReader reader)
    {
        id = reader.ReadInt16();
        type = (Type)reader.ReadInt16();
        position = reader.ReadVector3();
        rotation = reader.ReadQuaternion();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteInt16(id);
        writer.WriteInt16((short)type);
        writer.WriteVector3(position);
        writer.WriteQuaternion(rotation);
    }
}

public struct InputVisualizationMessage : IMessageBase
{
    public bool enableHeadCursor;


    public InputVisualizationMessage(bool enableHeadCursor)
    {
        this.enableHeadCursor = enableHeadCursor;
    }

    public void Deserialize(NetworkReader reader)
    {
        enableHeadCursor = reader.ReadBoolean();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteBoolean(enableHeadCursor);
    }
}

public struct TumorPositionMessage : IMessageBase
{
    public Vector3 position;


    public TumorPositionMessage(Vector3 position)
    {
        this.position = position;
    }

    public void Deserialize(NetworkReader reader)
    {
        position = reader.ReadVector3();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteVector3(position);
    }
}

public struct StudyManagerMessage: IMessageBase
{
    public StudyManager.State state;
    public int currentTrial;
    public int totalTrials;
    public float buttonPressAmount;

    public StudyManagerMessage(StudyManager.State state, int currentTrial, int totalTrials, float buttonPressAmount)
    {
        this.state = state;
        this.currentTrial = currentTrial;
        this.totalTrials = totalTrials;
        this.buttonPressAmount = buttonPressAmount;
    }

    public void Deserialize(NetworkReader reader)
    {
        state = (StudyManager.State)reader.ReadInt16();
        currentTrial = reader.ReadInt32();
        totalTrials = reader.ReadInt32();
        buttonPressAmount = reader.ReadSingle();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteInt16((short)state);
        writer.WriteInt32(currentTrial);
        writer.WriteInt32(totalTrials);
        writer.WriteSingle(buttonPressAmount);
    }
}

public struct VisualizationMethodMessage: IMessageBase
{
    
    public int matInside;
    public int matSkinWindow;
    public bool hasWindow;
    public bool front;



    public VisualizationMethodMessage(int matInside, int matSkinWindow, bool hasWindow, bool front)
    {
        
        this.matInside = matInside;
        this.matSkinWindow = matSkinWindow;
        this.hasWindow = hasWindow;
        this.front = front;
    }

    public void Deserialize(NetworkReader reader)
    {
        matInside = reader.ReadInt32();
        matSkinWindow = reader.ReadInt32();
        hasWindow = reader.ReadBoolean();
        front = reader.ReadBoolean();
        
        
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteInt32(matInside);
        writer.WriteInt32(matSkinWindow);
        writer.WriteBoolean(hasWindow);
        writer.WriteBoolean(front);
    }
}

public struct SceneStateMessage : IMessageBase
{
    public PhantomManager.Status status;
    public bool skinEnabled, windowEnabled;
    public float a, b, g, wC, wA, wD, focusSize;
    public float hatchIntensity, hatchUVScale;
    public bool hatchTriPlanar, hatchInverted;

    public Color chromaFarColor, chromaCloseColor;
    public float chromaLerpDist;

    public int window_mat;


    public SceneStateMessage(PhantomManager.State phantomState)
    {
        status = phantomState.status;
        skinEnabled = phantomState.skinEnabled;
        windowEnabled = phantomState.windowEnabled;
        var bichl = phantomState.bichlmeierState;
        a = bichl.alpha;
        b = bichl.beta;
        g = bichl.gamma;
        wC = bichl.weightCurv;
        wA = bichl.weightAngle;
        wD = bichl.weightDistance;
        focusSize = bichl.focusSize;
        var hatch = phantomState.hatchingState;
        hatchIntensity = hatch.intensity;
        hatchUVScale = hatch.uvscale;
        hatchTriPlanar = hatch.isTriPlanar;
        hatchInverted = hatch.isInverted;

        chromaFarColor = phantomState.chromaFarColor;
        chromaCloseColor = phantomState.chromaCloseColor;
        chromaLerpDist = phantomState.chromaLerpDist;

        window_mat = phantomState.window_mat;
    }

    public void Deserialize(NetworkReader reader)
    {
        status = (PhantomManager.Status) reader.ReadUInt16();
        skinEnabled = reader.ReadBoolean();
        windowEnabled = reader.ReadBoolean();
        a = reader.ReadSingle();
        b = reader.ReadSingle();
        g = reader.ReadSingle();
        wC = reader.ReadSingle();
        wA = reader.ReadSingle();
        wD = reader.ReadSingle();
        focusSize = reader.ReadSingle();
        hatchIntensity = reader.ReadSingle();
        hatchUVScale = reader.ReadSingle();
        hatchTriPlanar = reader.ReadBoolean();
        hatchInverted = reader.ReadBoolean();
        chromaFarColor = reader.ReadColor();
        chromaCloseColor = reader.ReadColor();
        chromaLerpDist = reader.ReadSingle();
        window_mat = reader.ReadInt32();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteUInt16((ushort) status);
        writer.WriteBoolean(skinEnabled);
        writer.WriteBoolean(windowEnabled);
        writer.WriteSingle(a);
        writer.WriteSingle(b);
        writer.WriteSingle(g);
        writer.WriteSingle(wC);
        writer.WriteSingle(wA);
        writer.WriteSingle(wD);
        writer.WriteSingle(focusSize);
        writer.WriteSingle(hatchIntensity);
        writer.WriteSingle(hatchUVScale);
        writer.WriteBoolean(hatchTriPlanar);
        writer.WriteBoolean(hatchInverted);
        writer.WriteColor(chromaFarColor);
        writer.WriteColor(chromaCloseColor);
        writer.WriteSingle(chromaLerpDist);
        writer.WriteInt32(window_mat);

    }
}

public struct HoloLensPositionMessage : IMessageBase
{
    public Vector3 position;
    public Quaternion rotation;
    

    public HoloLensPositionMessage(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
        
    }

    public void Deserialize(NetworkReader reader)
    {
        position = reader.ReadVector3();
        rotation = reader.ReadQuaternion();
        

    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteVector3(position);
        writer.WriteQuaternion(rotation);
        
    }
}