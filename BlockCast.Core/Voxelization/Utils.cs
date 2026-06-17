using System.Numerics;

class Utils
{
    public static float SignedArea(Vector3 a, Vector3 b, Vector3 origin)
    {
        a -= origin;
        b -= origin;

        return a.X * b.Z - a.Z * b.X;
    }

    private static float SignedArea(Vector3 a, Vector3 b)
    {
        return a.X * b.Z - a.Z * b.X;
    }

    public static bool IsPointInTriangleXZ(Vector3 a, Vector3 b, Vector3 c, Vector3 point)
    {
        float ab = SignedArea(a, b, point);
        float bc = SignedArea(b, c, point);
        float ca = SignedArea(c, a, point);

        bool hasNeg = ab < 0 || bc < 0 || ca < 0;
        bool hasPos = ab > 0 || bc > 0 || ca > 0;

        return !(hasNeg && hasPos);
    }
}