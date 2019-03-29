using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public static class Util
{
    public static void SetItemType(this PieceItem pieceItem)
    {
        Image itemImg = pieceItem.GetComponent<Image>();
        switch (pieceItem.MyType)
        {
            case PieceType.White:
                itemImg.color = Color.white;
                break;
            case PieceType.Black:
                itemImg.color = Color.black;
                break;
            default:
                break;
        }
    }

    public static void Log(string s)
    {
        Debug.LogFormat("<color=green>【{0}】</color>", s);
    }

    public static void LogFormat(string format, params object[] args)
    {
        Debug.LogFormat("<color=yellow>" + format + "</color>", args);
    }

    public static void LogError(string s)
    {
        Debug.LogFormat("<color=red>【{0}】</color>", s);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        Debug.LogFormat("<color=red>" + format + "</color>", args);
    }
}
