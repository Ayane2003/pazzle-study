using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct FallData
{
    public readonly int X { get; }
    public readonly int Y { get; }
    public readonly int Dest { get; }//�������

    public FallData(int x, int y, int dest)
    {
        X = x;
        Y = y;
        Dest = dest;
    }
}
public class BoardController : MonoBehaviour
{
    //���ƍ������`
    public const int FALL_FRAME_PER_CELL = 5;//�P�ʃZ��������̗����t���[����
    public const int BOARD_WIDTH = 6;
    public const int BOARD_HEIGHT = 14;

    //Instantiate�Ăяo���Ɍ^��񂪕K�v�B�ЂȌ^�ƂȂ�Ղ��Prefab��ݒ肷��
    //SerializedField�ŃG�f�B�^����ݒ�ł���B[=default!]�ŕK���ݒ肵�Ȃ��Ă͂Ȃ�Ȃ�
    [SerializeField] GameObject prefabPuyo = default!;

    //�Q�[�����E��2�����z��Ƃ��ă����o�[��p��
    //�Q�[���I�u�W�F�N�g�̔z���ێ�
    int[,] _board = new int[BOARD_HEIGHT, BOARD_WIDTH];
    GameObject[,] _Puyos = new GameObject[BOARD_HEIGHT, BOARD_WIDTH];

    List<FallData> _falls = new();
    int _fallFrames = 0;

    List<Vector2Int> _erases = new();
    int _eraseFrames = 0;
    //Start�Œ��ڏ��������Ă��ǂ�
    private void ClearAll()
    {
        for (int y = 0; y < BOARD_HEIGHT; y++)
        {
            //���[�v���񂷍ۂ́Ax����������ɂ���ق����������L���b�V���ɏ��₷���Ȃ�
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                _board[y, x] = 0;

                if (_Puyos[y, x] != null)
                {
                    Destroy(_Puyos[y, x]);
                }

                _Puyos[y, x] = null!;
            }
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        //Start�œ񎟌��z���������
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
        //�u�����Ƃ��Ă���ꏊ���Ֆʂ��͂ݏo���Ă��Ȃ���
        return 0 <= pos.x && pos.x < BOARD_WIDTH && 0 <= pos.y && pos.y < BOARD_HEIGHT;
    }

    public bool CanSettle(Vector2Int pos)
    {
        if (!IsValidated(pos))
        {
            return false;
        }

        //�z��̒l�����܂��Ă��Ȃ����i0�ɂȂ��Ă��Ȃ����j
        return 0 == _board[pos.y, pos.x];
    }

    public bool CheckFall()
    {
        _falls.Clear();
        _fallFrames = 0;

        int[] dsts = new int[BOARD_WIDTH];
        for (int y = 0; y < BOARD_HEIGHT; y++)
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                if (_board[y, x] == 0) continue;

                int dst = dsts[x];
                dsts[x] = y + 1;

                if (y == 0) continue;
                if (_board[y - 1, x] != 0) continue;

                _falls.Add(new FallData(x, y, dst));

                _board[dst, x] = _board[y, x];
                _board[y, x] = 0;
                _Puyos[dst, x] = _Puyos[y, x];
                _Puyos[y, x] = null;

                dsts[x] = dst + 1;
            }
        }

        return _falls.Count != 0;
    }

    public bool Fall()
    {
        _fallFrames++;

        float dy = _fallFrames / (float)FALL_FRAME_PER_CELL;
        int di = (int)dy;

        for (int i = _falls.Count - 1; 0 <= i; i--)
        {
            FallData f = _falls[i];

            Vector3 pos = _Puyos[f.Dest, f.X].transform.localPosition;
            pos.y = f.Y - dy;

            if (f.Y <= f.Dest + di)
            {
                pos.y = f.Dest;
                _falls.RemoveAt(i);
            }
            _Puyos[f.Dest, f.X].transform.localPosition = pos;
        }

        return _falls.Count != 0;
    }

    static readonly Vector2Int[] search_tbl = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    public bool CheckErase()
    {
        _eraseFrames = 0;
        _erases.Clear();

        uint[] isChecked = new uint[BOARD_HEIGHT];

        List<Vector2Int> add_list = new();
        for (int y = 0; y < BOARD_HEIGHT; y++)
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                if ((isChecked[y] & (1u << x)) != 0) continue;

                isChecked[y] |= (1u << x);

                int type = _board[y, x];
                if (type == 0) continue;

                System.Action<Vector2Int> get_connection = null;
                get_connection = (pos) =>
                {
                    add_list.Add(pos);//�폜�Ώۂɂ���

                    foreach (Vector2Int d in search_tbl)
                    {
                        Vector2Int target = pos + d;
                        if (target.x < 0 || BOARD_WIDTH <= target.x || target.y < 0 || BOARD_HEIGHT <= target.y) continue;//�͈͊O

                        if (_board[target.y, target.x] != type) continue;//�F�Ⴂ

                        if ((isChecked[target.y] & (1u << target.x)) != 0) continue;

                        isChecked[target.y] |= (1u << target.x);
                        get_connection(target);
                    }
                };

                add_list.Clear();
                get_connection(new Vector2Int(x, y));

                if (4 <= add_list.Count)
                {
                    _erases.AddRange(add_list);
                }
            }
        }

        return _erases.Count != 0;
    }
    //�z��u_board�v�ɒl��ݒ肷�郁�\�b�h��p��
    public bool Settle(Vector2Int pos, int val)
    {
        //�l�ݒ�̑O�ɒu�����Ƃ��ł��邩�`�F�b�N
        if (!CanSettle(pos))
        {
            return false;
        }

        _board[pos.y, pos.x] = val;

        Debug.Assert(_Puyos[pos.y, pos.x] == null);
        //�e�̈ʒu����荞�ނ��߂ɁA���̑O�ɐݒ肳��Ă���e�̈ʒu(transform.position�Œl�𑫂�����)
        Vector3 world_postion = transform.position + new Vector3(pos.x, pos.y, 0.0f);

        //Settle���\�b�h�ŃQ�[���I�u�W�F�N�g�𐶐�����Instantiate���Ăяo��(�ʒu�͐����̍��W�����̂܂ܕ��������_�ɂ���)
        _Puyos[pos.y, pos.x] = Instantiate(prefabPuyo, world_postion, Quaternion.identity, transform);

        //�Q�[���I�u�W�F�N�g�̐F��ݒ�
        _Puyos[pos.y, pos.x].GetComponent<PuyoController>().SetPuyoType((PuyoType)val);

        return true;
    }

    public bool Erase()
    {
        _eraseFrames++;

        float t = _eraseFrames * Time.deltaTime;
        t = 1.0f - 10.0f * ((t - 0.1f) * (t - 0.1f) - 0.1f * 0.1f);

        if (t <= 0.0f)
        {

            foreach (Vector2Int d in _erases)
            {
                Destroy(_Puyos[d.y, d.x]);
                _Puyos[d.y, d.x] = null;
                _board[d.y, d.x] = 0;
            }

            return false;
        }

        foreach (Vector2Int d in _erases)
        {
            _Puyos[d.y, d.x].transform.localScale = Vector3.one * t;
        }

        return true;
    }
}