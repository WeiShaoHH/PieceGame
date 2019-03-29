using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//单例

    public PlayerType curPlayerType = PlayerType.Me;//玩家类型

    public PieceType gamePieceType = PieceType.Black;//当前能下的棋子类型

    public Transform piecePrefab;//棋子预置

    public float volVelocity;//行方向偏移量

    public float colVelocity;//列方向偏移量

    public int totalVolCount;//需要生成的总行数

    public int totalColCount;//需要生成的总列数

    public static PieceItem[,] pieceItemArray = new PieceItem[19, 19];//棋盘上的所有子

    private List<PieceItem> whiteItems = new List<PieceItem>();//临时白棋总数

    private List<PieceItem> blackItems = new List<PieceItem>();//临时黑棋总数

    private List<PieceItem> tempItems = new List<PieceItem>();//当前方向上的所有棋子

    public Point PlayerSetPoint;//玩家当前下的点

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        GameData.GameOver = false;
        GameData.Winner = PieceType.Black;
        gamePieceType = PieceType.Black;
        curPlayerType = PlayerType.Me;
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
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
    /// 每当下完一步棋之后开始检测是否赢了
    /// </summary>
    /// <param name="pieceItem">当前下的棋</param>
    public void CalcSuccess(PieceItem pieceItem)
    {
        if (pieceItem.MyType == PieceType.Black)
        {
            PlayerSetPoint = new Point(pieceItem.vol, pieceItem.col);
        }
        for (int i = 0; i < 4; i++)
        {
            CheckSuccess(pieceItem, (CheckType)i);
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
        tempItems.Clear();
        whiteItems.Clear();
        blackItems.Clear();
        PieceItem calcTemp = null;  //根据当前下的子来得到一个和当前子形成一次函数的子，也可以叫目标子
        int tempVol = 0, tempCol = 0; //目标子的行列数
        switch (checkType)
        {
            case CheckType.Col:
                {
                    #region 纵列
                    tempVol = pieceItem.vol;    //计算纵列保证【行】一致
                    if (pieceItem.col < totalColCount - 1)   //当前子不是最【上】边的子
                    {
                        tempCol = pieceItem.col + 1;    //目标子的列数 = 当前子的列数 + 1
                    }
                    else
                    {
                        tempCol = pieceItem.col - 1;    //目标子的列数 = 当前子的列数 - 1
                    }
                    #endregion
                }
                break;
            case CheckType.Row:
                {
                    #region 横行
                    tempCol = pieceItem.col;    //计算横行保证【列】一致
                    if (pieceItem.vol < totalVolCount - 1)   //当前子不是最【右】边的子
                    {
                        tempVol = pieceItem.vol + 1;    //目标子的行数 = 当前子的行数 + 1
                    }
                    else
                    {
                        tempVol = pieceItem.vol - 1;    //目标子的行数 = 当前子的列数 - 1
                    }
                    #endregion
                }
                break;
            case CheckType.AddMid:
                {
                    #region 斜向【上】
                    //【斜向上】行判断当前子是否时右上角那个
                    if (pieceItem.vol < totalVolCount - 1 && pieceItem.col < totalColCount - 1)
                    {
                        tempVol = pieceItem.vol + 1;    //目标子的行数 = 当前子的行数 + 1
                        tempCol = pieceItem.col + 1;    //目标子的列数 = 当前子的列数 + 1
                    }
                    else
                    {
                        //如果右上角溢出就找到自己左下方的那个作为目标子
                        tempVol = pieceItem.vol - 1;    //目标子的行数 = 当前子的行数 - 1
                        tempCol = pieceItem.col - 1;    //目标子的列数 = 当前子的列数 - 1
                        if (tempCol < 0 || tempVol < 0)//左上角  右下角 
                        {
                            return;
                        }
                    }
                    #endregion
                }
                break;
            case CheckType.DecMid:
                {
                    #region  斜向【下】
                    //计算斜向下行，如果当前子不是右下角的那个子
                    if (pieceItem.vol < totalVolCount - 1 && pieceItem.col > 0)
                    {
                        //右下方找目标子
                        tempVol = pieceItem.vol + 1;
                        tempCol = pieceItem.col - 1;
                    }
                    else
                    {
                        //左上方找目标子
                        tempVol = pieceItem.vol - 1;
                        tempCol = pieceItem.col + 1;
                        bool overflowCol = tempCol > (totalColCount - 1);
                        bool overflowVol = tempVol < 0;
                        if (overflowCol || overflowVol)//右上角   左下角
                        {
                            return;
                        }
                    }
                    #endregion
                }
                break;
            default:
                break;
        }
        calcTemp = pieceItemArray[tempVol, tempCol];    //根据计算的vol,col得到目标子实体
        //y1=k * x1 + b
        //y2=k * x2 + b      -----根据下边的公式计算该条线上的子是否满足   (((y1 - y2) * j) - ((x1 - x2) * i) + (x1 * y2) - (x2 * y1)) == 0
        int x1 = pieceItem.vol;
        int y1 = pieceItem.col;
        int x2 = calcTemp.vol;
        int y2 = calcTemp.col;
        for (int i = 0; i < totalColCount; i++)
        {
            for (int j = 0; j < totalVolCount; j++)
            {
                bool h = (((y1 - y2) * j) - ((x1 - x2) * i) + (x1 * y2) - (x2 * y1)) == 0 ? true : false;   //点是否满足求出来的一次函数的方程
                if (h)
                {
                    PieceItem item = pieceItemArray[j, i];
                    if (pieceItemArray[j, i].beLoaded)
                    {
                        if (item.MyType == PieceType.White)
                        {
                            whiteItems.Add(item);
                        }
                        else
                        {
                            blackItems.Add(item);
                        }
                    }
                    tempItems.Add(item);
                }
            }
        }
        Check(checkType, PieceType.Black, blackItems);
        Check(checkType, PieceType.White, whiteItems);
    }

    /// <summary>
    /// 将满足函数的黑白子加入到各自的计算列表中进行计算
    /// </summary>
    /// <param name="checkType"></param>
    /// <param name="pieceType"></param>
    /// <param name="tempItems"></param>
    private void Check(CheckType checkType, PieceType pieceType, List<PieceItem> tempItems)
    {
        if (tempItems.Count < 5)
        {
            return;
        }
        int tempcount = 1;//当前计算成功的棋子数量
        for (int i = 0; i < tempItems.Count - 1; i++)
        {
            if (tempItems[i].beLoaded && tempItems[i + 1].beLoaded)
            {
                if (tempItems[i].MyType == tempItems[i + 1].MyType)     //判断前后两个棋子类型是否一致
                {
                    int checkCol = 0, checkVol = 0;     //根据当前检测类型来判断行和列的增量大小
                    switch (checkType)
                    {
                        case CheckType.Col:
                            checkCol = 1;   //检测列：列是逐渐变化 1 的，行不变
                            checkVol = 0;
                            break;
                        case CheckType.Row:
                            checkCol = 0;
                            checkVol = 1;   //检测行：行是逐渐变化 1 的，列不变
                            break;
                        case CheckType.AddMid:
                        case CheckType.DecMid:
                            checkCol = checkVol = 1;   //检测斜向：行和列都会变化1
                            break;
                        default:
                            break;
                    }
                    int col = Mathf.Abs(tempItems[i].col - tempItems[i + 1].col);   //当前棋子和下一个棋子【列】的差值  计算这两个值的目的是为了比较是否时相邻的
                    int vol = Mathf.Abs(tempItems[i].vol - tempItems[i + 1].vol);   //当前棋子和下一个棋子【行】的差值  避免出现 黑黑黑黑   黑 也会算进去的情况
                    if (col == checkCol && vol == checkVol)
                    {
                        tempcount++;
                    }
                }
            }
        }
        if (tempcount >= 5)
        {
            //switch (checkType)
            //{
            //    case CheckType.Col:
            //        Util.LogFormat("【列={0}棋胜】", pieceType == PieceType.Black ? "黑" : "白");
            //        break;
            //    case CheckType.Row:
            //        Util.LogFormat("【行={0}棋胜】", pieceType == PieceType.Black ? "黑" : "白");
            //        break;
            //    case CheckType.AddMid:
            //        Util.LogFormat("【斜上={0}棋胜】", pieceType == PieceType.Black ? "黑" : "白");
            //        break;
            //    case CheckType.DecMid:
            //        Util.LogFormat("【斜下={0}棋胜】", pieceType == PieceType.Black ? "黑" : "白");
            //        break;
            //    default:
            //        break;
            //}
            GameData.GameOver = true;
            GameData.Winner = pieceType;
            Util.LogErrorFormat("游戏结束！获胜玩家是：{0}", pieceType == PieceType.Black ? "黑手" : "白手");
        }
    }
}
