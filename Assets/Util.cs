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
            case PieceType.None:
                break;
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
}
