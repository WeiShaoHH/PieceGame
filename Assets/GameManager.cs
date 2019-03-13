using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerType curPlayerType = PlayerType.Me;
    public PieceType GamePieceType = PieceType.None;
    public Transform piecePrefab;
    public float volVelocity;
    public float colVelocity;
    public int totalVolCount;
    public int totalColCount;
    public PieceItem[,] pieceItemArray = new PieceItem[19, 19];
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < totalColCount; i++)
        {
            for (int j = 0; j < totalVolCount; j++)
            {
                Transform piece = Instantiate(piecePrefab, transform);
                piece.name = "行：" + j + "--列：" + i;
                piece.localPosition = new Vector2(i * volVelocity, j * colVelocity);
                PieceItem pieceItem = piece.gameObject.GetComponent<PieceItem>();
                pieceItem.col = i;
                pieceItem.vol = j;
                pieceItemArray[j, i] = pieceItem;
            }
        }

    }
    public void CalcSuccess(PieceItem pieceItem)
    {
        int col_blackSuccessCount = 0;
        int vol_blackSuccessCount = 0;
        int midTop_blackSuccessCount = 0;

        int col_whiteSuccessCount = 0;
        int vol_whiteSuccessCount = 0;
        int midTop_whiteSuccessCount = 0;
        PieceItem calcTemp = null;
        if (pieceItem.vol < totalVolCount - 1 && pieceItem.col < totalColCount - 1)
        {
            calcTemp = pieceItemArray[pieceItem.vol + 1, pieceItem.col + 1];
        }
        else
        {
            calcTemp = pieceItemArray[pieceItem.vol - 1, pieceItem.col - 1];
        }
        int x1 = pieceItem.vol;
        int y1 = pieceItem.col;
        int x2 = calcTemp.vol;
        int y2 = calcTemp.col;
        Debug.LogFormat("x1({0},{1})--x2({2},{3})", x1, y1, x2, y2);
        List<PieceItem> tempItems = new List<PieceItem>();
        for (int i = 0; i < totalColCount; i++)
        {
            for (int j = 0; j < totalVolCount; j++)
            {
                if (pieceItemArray[j, i].beLoaded)
                {
                    bool h = (((y1 - y2) * j) - ((x1 - x2) * i) + (x1 * y2) - (x2 * y1)) == 0 ? true : false;
                    if (h)
                    {
                        if (tempItems.Count > 0)
                        {
                            if (tempItems[tempItems.Count - 1].MyType == pieceItemArray[j, i].MyType)
                            {
                                tempItems.Add(pieceItemArray[j, i]);
                                Debug.LogFormat("加入{0},(行：{1},{2})", pieceItemArray[j, i].MyType, j, i);
                            }
                        }
                        else
                        {
                            tempItems.Add(pieceItemArray[j, i]);
                            Debug.LogFormat("加入{0},(行：{1},{2})", pieceItemArray[j, i].MyType, j, i);
                        }

                    }
                }

            }
        }
        if (tempItems.Count >= 5)
        {
            for (int i = 0; i < tempItems.Count - 1; i++)
            {
                if (tempItems[i].beLoaded && tempItems[i + 1].beLoaded)
                {
                    if (tempItems[i].MyType == tempItems[i + 1].MyType)
                    {
                        switch (tempItems[i].MyType)
                        {
                            case PieceType.None:
                                break;
                            case PieceType.White:
                                midTop_whiteSuccessCount++;
                                break;
                            case PieceType.Black:
                                midTop_blackSuccessCount++;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        midTop_whiteSuccessCount = 0;
                        midTop_blackSuccessCount = 0;
                        continue;
                    }
                }
                else
                {
                    midTop_whiteSuccessCount = 0;
                    midTop_blackSuccessCount = 0;
                    continue;
                }
            }
            if (midTop_blackSuccessCount == 5)
            {
                Debug.Log("【斜=黑棋胜】");
                midTop_blackSuccessCount = 0;

            }
            if (midTop_whiteSuccessCount == 5)
            {
                Debug.Log("【斜=白棋胜】");
                vol_whiteSuccessCount = 0;

            }
        }
        for (int i = 0; i < totalColCount; i++)
        {
            for (int j = 0; j < totalVolCount - 1; j++)
            {
                if (i == pieceItem.col)//一列
                {
                    //看行
                    PieceItem temp1 = pieceItemArray[j, i];
                    PieceItem temp2 = pieceItemArray[j + 1, i];
                    if (temp1.beLoaded && temp2.beLoaded)
                    {
                        if (temp1.MyType == temp2.MyType)
                        {
                            switch (temp1.MyType)
                            {
                                case PieceType.None:
                                    break;
                                case PieceType.White:
                                    col_whiteSuccessCount++;
                                    break;
                                case PieceType.Black:
                                    col_blackSuccessCount++;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            col_whiteSuccessCount = 0;
                            col_blackSuccessCount = 0;
                            continue;
                        }
                    }
                    else
                    {
                        col_whiteSuccessCount = 0;
                        col_blackSuccessCount = 0;
                        continue;
                    }
                }
                if (col_blackSuccessCount == 4)
                {
                    Debug.Log("【行=黑棋胜】");
                    col_blackSuccessCount = 0;
                    break;
                }
                if (col_whiteSuccessCount == 4)
                {
                    Debug.Log("【行=白棋胜】");
                    col_whiteSuccessCount = 0;
                    break;
                }
            }
        }
        for (int i = 0; i < totalColCount - 1; i++)
        {
            for (int j = 0; j < totalVolCount; j++)
            {
                if (j == pieceItem.vol)//一行
                {
                    //看列
                    PieceItem temp1 = pieceItemArray[j, i];
                    PieceItem temp2 = pieceItemArray[j, i + 1];
                    if (temp1.beLoaded && temp2.beLoaded)
                    {
                        if (temp1.MyType == temp2.MyType)
                        {
                            switch (temp1.MyType)
                            {
                                case PieceType.None:
                                    break;
                                case PieceType.White:
                                    vol_whiteSuccessCount++;
                                    break;
                                case PieceType.Black:
                                    vol_blackSuccessCount++;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            vol_blackSuccessCount = 0;
                            vol_whiteSuccessCount = 0;
                            continue;
                        }
                    }
                    else
                    {
                        vol_blackSuccessCount = 0;
                        vol_whiteSuccessCount = 0;
                        continue;
                    }
                }
                if (vol_blackSuccessCount == 4)
                {
                    Debug.Log("【列=黑棋胜】");
                    vol_blackSuccessCount = 0;
                    break;
                }
                if (vol_whiteSuccessCount == 4)
                {
                    Debug.Log("【列=白棋胜】");
                    vol_whiteSuccessCount = 0;
                    break;
                }
            }
        }

    }
}
