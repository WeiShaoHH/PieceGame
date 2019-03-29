public enum PieceType
{
    White,
    Black
}
public enum PlayerType
{
    Me,
    Other,
    AI
}
public enum CheckType
{
    Col,//列

    Row,//行

    AddMid,//斜向上

    DecMid,//斜向下
}

public enum PieceAIType
{
    Dash_one,//眠一

    Live_one,//活一

    Dash_two,//眠二

    Live_Two,//活二

    Dash_Three,//眠三

    Live_Three,//活三

    Dash_Four,//眠四

    Live_Four,//活四

    Five//连五
}