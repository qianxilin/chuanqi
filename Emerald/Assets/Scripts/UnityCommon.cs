using UnityEngine;

public static class ClientFunctions
{   
    public static Vector2 VectorMove(Vector2 p, MirDirection d, int i)
    {
        Vector2 newp = new Vector2(p.x, p.y);
        switch (d)
        {
            case MirDirection.Up:
                newp += Vector2.down * i;
                break;
            case MirDirection.UpRight:
                newp += Vector2.down * i + Vector2.right * i;
                break;
            case MirDirection.Right:
                newp += Vector2.right * i;
                break;
            case MirDirection.DownRight:
                newp += Vector2.up * i + Vector2.right * i;
                break;
            case MirDirection.Down:
                newp += Vector2.up * i;
                break;
            case MirDirection.DownLeft:
                newp += Vector2.up * i + Vector2.left * i;
                break;
            case MirDirection.Left:
                newp += Vector2.left * i;
                break;
            case MirDirection.UpLeft:
                newp += Vector2.down * i + Vector2.left * i;
                break;
        }
        return newp;
    }
    public static Vector2 Back(Vector2 p, MirDirection direction, int i)
    {
        MirDirection backdirection = (MirDirection)(((int)direction + 4) % 8);
        return VectorMove(p, backdirection, i);
    }
    public static Quaternion GetRotation(MirDirection direction)
    {
        switch (direction)
        {
            case MirDirection.UpRight:
                return Quaternion.AngleAxis(45, Vector3.up);
            case MirDirection.Right:
                return Quaternion.AngleAxis(90, Vector3.up);
            case MirDirection.DownRight:
                return Quaternion.AngleAxis(135, Vector3.up);
            case MirDirection.Down:
                return Quaternion.AngleAxis(180, Vector3.up);
            case MirDirection.DownLeft:
                return Quaternion.AngleAxis(225, Vector3.up);
            case MirDirection.Left:
                return Quaternion.AngleAxis(270, Vector3.up);
            case MirDirection.UpLeft:
                return Quaternion.AngleAxis(315, Vector3.up);
            default:
                return new Quaternion(0, 0, 0, 0);
        }
    }    
}
