using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    //幅と高さを定義
    public const int BOARD_WIDTH = 6;
    public const int BOARD_HEIGHT = 14;

    //Instantiate呼び出しに型情報が必要。ひな型となるぷよのPrefabを設定する
    //SerializedFieldでエディタから設定できる。[=default!]で必ず設定しなくてはならない
    [SerializeField] GameObject prefabPuyo = default!;

    //ゲーム世界に2次元配列としてメンバーを用意
    //ゲームオブジェクトの配列を保持
    int[,] _board = new int[BOARD_HEIGHT, BOARD_WIDTH];
    GameObject[,] _Puyo = new GameObject[BOARD_HEIGHT, BOARD_WIDTH];

    //Startで直接初期化しても良い
    private void ClearAll()
    {
        for (int y = 0; y < BOARD_HEIGHT; y++)
        {
            //ループを回す際は、x成分を内側にするほうがメモリキャッシュに乗りやすくなる
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                _board[y, x] = 0;

                if (_Puyo[y,x]!=null)
                {
                    Destroy(_Puyo[y, x]);
                }

                _Puyo[y, x] = null!;
            }
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        //Startで二次元配列を初期化
        ClearAll();

//        for (int y = 0; y < BOARD_HEIGHT; y++)
//       {
//           for (int x = 0; x < BOARD_WIDTH; x++)
//           {
//               Settle(new Vector2Int(x, y), Random.Range(1, 7));
//           }
//       }
    }

    public static bool IsValidated(Vector2Int pos)
    {
        //置こうとしている場所が盤面をはみ出していないか
        return 0 <= pos.x && pos.x < BOARD_WIDTH && 0 <= pos.y && pos.y < BOARD_HEIGHT;
    }

    public bool CanSettle(Vector2Int pos)
    {
        if (!IsValidated(pos))
        {
            return false;
        }

        //配列の値が埋まっていないか（0になっていないか）
        return 0 == _board[pos.y, pos.x];
    }

    //配列「_board」に値を設定するメソッドを用意
    public bool Settle(Vector2Int pos,int val)
    {
        //値設定の前に置くことができるかチェック
        if (!CanSettle(pos))
        {
            return false;
        }

        _board[pos.y, pos.x] = val;

        Debug.Assert(_Puyo[pos.y, pos.x] == null);
        //親の位置を取り込むために、その前に設定されている親の位置(transform.positionで値を足しこむ)
        Vector3 world_postion = transform.position + new Vector3(pos.x, pos.y, 0.0f);

        //Settleメソッドでゲームオブジェクトを生成するInstantiateを呼び出し(位置は整数の座標をそのまま浮動小数点にする)
        _Puyo[pos.y, pos.x] = Instantiate(prefabPuyo, world_postion, Quaternion.identity, transform);
    
        //ゲームオブジェクトの色を設定
        _Puyo[pos.y, pos.x].GetComponent<PuyoController>().SetPuyoType((PuyoType)val);

        return true;
    }
}
