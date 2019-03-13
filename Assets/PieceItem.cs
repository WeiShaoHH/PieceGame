using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class PieceItem : MonoBehaviour
{
    public bool beLoaded = false;
    public int vol;
    public int col;
    public PieceType MyType { get; private set; }
    // Use this for initialization
    void Start()
    {

    }
    public void OnPointClick()
    {
        if (beLoaded)
        {
            return;
        }
        switch (GameManager.Instance.GamePieceType)
        {
            case PieceType.White:
                MyType = PieceType.White;
                GameManager.Instance.GamePieceType = PieceType.Black;
                break;
            case PieceType.Black:
                MyType = PieceType.Black;
                GameManager.Instance.GamePieceType = PieceType.White;
                break;

        }
        Util.SetItemType(this);
        beLoaded = true;
        GameManager.Instance.CalcSuccess(this);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
