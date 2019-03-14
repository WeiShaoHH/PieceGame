using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CheckType
{
    Col,//列
    Row,//行
    AddMid,//斜向上
    DecMid,//斜向下
}
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
    private List<PieceItem> checkItems = new List<PieceItem>();
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
    /// <summary>
    /// 判断胜负
    /// 采用y=kx+b的形式检测，把行列看作一个一次函数第一象限，
    /// 落下某一个子之后遍历该子的斜上、斜下、纵列、纵行四个一次函数，判断是否满足相邻一起共5个，满足则胜利
    /// </summary>
    /// <param name="pieceItem">当前落的子</param>
    /// <param name="checkType">检测类型</param>
    private void CheckSuccess(PieceItem pieceItem, CheckType checkType)
    {
        checkItems.Clear();
        int blackSuccessCount = 1;
        int whiteSuccessCount = 1;
        PieceItem calcTemp = null;
        int tempVol = 0, tempCol = 0;
        switch (checkType)
        {
            case CheckType.Col:
                {
                    tempVol = pieceItem.vol;
                    if (pieceItem.col < totalColCount - 1)
                    {
                        tempCol = pieceItem.col + 1;
                    }
                    else
                    {
                        tempCol = pieceItem.col - 1;
                    }
                }
                break;
            case CheckType.Row:
                {
                    tempCol = pieceItem.col;
                    if (pieceItem.vol < totalVolCount - 1)
                    {
                        tempVol = pieceItem.vol + 1;
                    }
                    else
                    {
                        tempVol = pieceItem.vol - 1;
                    }
                }
                break;
            case CheckType.AddMid:
                {
                    if (pieceItem.vol < totalVolCount - 1 && pieceItem.col < totalColCount - 1)
                    {
                        tempVol = pieceItem.vol + 1;
                        tempCol = pieceItem.col + 1;
                    }
                    else
                    {
                        tempVol = pieceItem.vol - 1;
                        tempCol = pieceItem.col - 1;
                    }
                }
                break;
            case CheckType.DecMid:
                {
                    if (pieceItem.vol < totalVolCount - 1 && pieceItem.col > 0)
                    {
                        tempVol = pieceItem.vol + 1;
                        tempCol = pieceItem.col - 1;
                    }
                    else
                    {
                        tempVol = pieceItem.vol - 1;
                        tempCol = pieceItem.col + 1;
                    }
                }
                break;
            default:
                break;
        }
        calcTemp = pieceItemArray[tempVol, tempCol];
        int x1 = pieceItem.vol;
        int y1 = pieceItem.col;
        int x2 = calcTemp.vol;
        int y2 = calcTemp.col;
        List<PieceItem> tempItems = new List<PieceItem>();
        for (int i = 0; i < totalColCount; i++)
        {
            for (int j = 0; j < totalVolCount; j++)
            {
                if (pieceItemArray[j, i].beLoaded)
                {
                    bool h = (((y1 - y2) * j) - ((x1 - x2) * i) + (x1 * y2) - (x2 * y1)) == 0 ? true : false;//点是否满足求出来的一次函数的方程
                    if (h)
                    {
                        if (tempItems.Count > 0)
                        {
                            if (tempItems[tempItems.Count - 1].MyType == pieceItemArray[j, i].MyType)
                            {
                                tempItems.Add(pieceItemArray[j, i]);
                            }
                        }
                        else
                        {
                            tempItems.Add(pieceItemArray[j, i]);
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
                        int col = 0, vol = 0;
                        int checkCol = 0, checkVol = 0;
                        switch (checkType)
                        {
                            case CheckType.Col:
                                checkCol = 1;
                                checkVol = 0;
                                break;
                            case CheckType.Row:
                                checkCol = 0;
                                checkVol = 1;
                                break;
                            case CheckType.AddMid:
                                checkCol = checkVol = 1;
                                break;
                            case CheckType.DecMid:
                                checkCol = checkVol = 1;
                                break;
                            default:
                                break;
                        }
                        col = Mathf.Abs(tempItems[i].col - tempItems[i + 1].col);
                        vol = Mathf.Abs(tempItems[i].vol - tempItems[i + 1].vol);
                        if (col == checkCol && vol == checkVol)
                        {
                            switch (tempItems[i].MyType)
                            {
                                case PieceType.None:
                                    break;
                                case PieceType.White:
                                    whiteSuccessCount++;
                                    break;
                                case PieceType.Black:
                                    blackSuccessCount++;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            if (blackSuccessCount >= 5 || whiteSuccessCount >= 5)
            {
                switch (checkType)
                {
                    case CheckType.Col:
                        Debug.LogFormat("【列={0}棋胜】", pieceItem.MyType == PieceType.Black ? "黑" : "白");
                        break;
                    case CheckType.Row:
                        Debug.LogFormat("【行={0}棋胜】", pieceItem.MyType == PieceType.Black ? "黑" : "白");
                        break;
                    case CheckType.AddMid:
                        Debug.LogFormat("【斜上={0}棋胜】", pieceItem.MyType == PieceType.Black ? "黑" : "白");
                        break;
                    case CheckType.DecMid:
                        Debug.LogFormat("【斜下={0}棋胜】", pieceItem.MyType == PieceType.Black ? "黑" : "白");
                        break;
                    default:
                        break;
                }
                whiteSuccessCount = 1;
                blackSuccessCount = 1;
            }
        }
    }
    public void CalcSuccess(PieceItem pieceItem)
    {
        for (int i = 0; i < 4; i++)
        {
            CheckSuccess(pieceItem, (CheckType)i);
        }
        #region oldVersion

        //int col_blackSuccessCount = 0;
        //int vol_blackSuccessCount = 0;
        //int midTop_blackSuccessCount = 0;

        //int col_whiteSuccessCount = 0;
        //int vol_whiteSuccessCount = 0;
        //int midTop_whiteSuccessCount = 0;
        //PieceItem calcTemp = null;
        //if (pieceItem.vol < totalVolCount - 1 && pieceItem.col < totalColCount - 1)
        //{
        //    calcTemp = pieceItemArray[pieceItem.vol + 1, pieceItem.col + 1];
        //}
        //else
        //{
        //    calcTemp = pieceItemArray[pieceItem.vol - 1, pieceItem.col - 1];
        //}
        //int x1 = pieceItem.vol;
        //int y1 = pieceItem.col;
        //int x2 = calcTemp.vol;
        //int y2 = calcTemp.col;
        ////Debug.LogFormat("x1({0},{1})--x2({2},{3})", x1, y1, x2, y2);

        //for (int i = 0; i < totalColCount; i++)
        //{
        //    for (int j = 0; j < totalVolCount - 1; j++)
        //    {
        //        if (i == pieceItem.col)//一列
        //        {
        //            //看行
        //            PieceItem temp1 = pieceItemArray[j, i];
        //            PieceItem temp2 = pieceItemArray[j + 1, i];
        //            if (temp1.beLoaded && temp2.beLoaded)
        //            {
        //                if (temp1.MyType == temp2.MyType)
        //                {
        //                    switch (temp1.MyType)
        //                    {
        //                        case PieceType.None:
        //                            break;
        //                        case PieceType.White:
        //                            col_whiteSuccessCount++;
        //                            break;
        //                        case PieceType.Black:
        //                            col_blackSuccessCount++;
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }
        //                else
        //                {
        //                    col_whiteSuccessCount = 0;
        //                    col_blackSuccessCount = 0;
        //                    continue;
        //                }
        //            }
        //            else
        //            {
        //                col_whiteSuccessCount = 0;
        //                col_blackSuccessCount = 0;
        //                continue;
        //            }
        //        }
        //        if (col_blackSuccessCount >= 4)
        //        {
        //            Debug.Log("【行=黑棋胜】");
        //            col_blackSuccessCount = 0;
        //            break;
        //        }
        //        if (col_whiteSuccessCount >= 4)
        //        {
        //            Debug.Log("【行=白棋胜】");
        //            col_whiteSuccessCount = 0;
        //            break;
        //        }
        //    }
        //}
        //for (int i = 0; i < totalColCount - 1; i++)
        //{
        //    for (int j = 0; j < totalVolCount; j++)
        //    {
        //        if (j == pieceItem.vol)//一行
        //        {
        //            //看列
        //            PieceItem temp1 = pieceItemArray[j, i];
        //            PieceItem temp2 = pieceItemArray[j, i + 1];
        //            if (temp1.beLoaded && temp2.beLoaded)
        //            {
        //                if (temp1.MyType == temp2.MyType)
        //                {
        //                    switch (temp1.MyType)
        //                    {
        //                        case PieceType.None:
        //                            break;
        //                        case PieceType.White:
        //                            vol_whiteSuccessCount++;
        //                            break;
        //                        case PieceType.Black:
        //                            vol_blackSuccessCount++;
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }
        //                else
        //                {
        //                    vol_blackSuccessCount = 0;
        //                    vol_whiteSuccessCount = 0;
        //                    continue;
        //                }
        //            }
        //            else
        //            {
        //                vol_blackSuccessCount = 0;
        //                vol_whiteSuccessCount = 0;
        //                continue;
        //            }
        //        }
        //        if (vol_blackSuccessCount >= 4)
        //        {
        //            Debug.Log("【列=黑棋胜】");
        //            vol_blackSuccessCount = 0;
        //            break;
        //        }
        //        if (vol_whiteSuccessCount >= 4)
        //        {
        //            Debug.Log("【列=白棋胜】");
        //            vol_whiteSuccessCount = 0;
        //            break;
        //        }
        //    }
        //}

        #endregion
    }
}
