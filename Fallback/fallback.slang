public struct VertexInput {
    public float3 PositionOs : POSITION;
    public float3 NormalOs : NORMAL;
    public float2 TexCoord : TEXCOORD0;
}
public struct PixelInput {
    public float3 PositionWs : POSITION;
    public float4 PositionPs : SV_Position;
    public float3 NormalWs : NORMAL;
    public float2 TexCoord : TEXCOORD0;
}

public struct CameraData {
    public matrix Projection;
    public matrix View;
    public float3 Position;
    public float3 Forward;

    public float4 Project(in float3 pos) {
        return mul(mul(float4(pos, 1.0), View), Projection);
    }
}
public uniform CameraData Camera;

PixelInput Vertex(VertexInput i) {
    PixelInput o; //TODO move most of this into pixel input constructor?
    o.PositionWs = i.PositionOs; //TODO transform these
    o.NormalWs = i.NormalOs;
    o.TexCoord = i.TexCoord;
    o.PositionPs = Camera.Project(o.PositionWs);
    return o;
}

float4 Fragment(PixelInput i) {
    return float4(1, 0, 1, 1);
}