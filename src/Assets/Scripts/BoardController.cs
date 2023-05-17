using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    //���ƍ������`
    public const int BOARD_WIDTH = 6;
    public const int BOARD_HEIGHT = 14;

    //Instantiate�Ăяo���Ɍ^��񂪕K�v�B�ЂȌ^�ƂȂ�Ղ��Prefab��ݒ肷��
    //SerializedField�ŃG�f�B�^����ݒ�ł���B[=default!]�ŕK���ݒ肵�Ȃ��Ă͂Ȃ�Ȃ�
    [SerializeField] GameObject prefabPuyo = default!;

    //�Q�[�����E��2�����z��Ƃ��ă����o�[��p��
    //�Q�[���I�u�W�F�N�g�̔z���ێ�
    int[,] _board = new int[BOARD_HEIGHT, BOARD_WIDTH];
    GameObject[,] _Puyo = new GameObject[BOARD_HEIGHT, BOARD_WIDTH];

    //Start�Œ��ڏ��������Ă��ǂ�
    private void ClearAll()
    {
        for (int y = 0; y < BOARD_HEIGHT; y++)
        {
            //���[�v���񂷍ۂ́Ax����������ɂ���ق����������L���b�V���ɏ��₷���Ȃ�
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

    //�z��u_board�v�ɒl��ݒ肷�郁�\�b�h��p��
    public bool Settle(Vector2Int pos,int val)
    {
        //�l�ݒ�̑O�ɒu�����Ƃ��ł��邩�`�F�b�N
        if (!CanSettle(pos))
        {
            return false;
        }

        _board[pos.y, pos.x] = val;

        Debug.Assert(_Puyo[pos.y, pos.x] == null);
        //�e�̈ʒu����荞�ނ��߂ɁA���̑O�ɐݒ肳��Ă���e�̈ʒu(transform.position�Œl�𑫂�����)
        Vector3 world_postion = transform.position + new Vector3(pos.x, pos.y, 0.0f);

        //Settle���\�b�h�ŃQ�[���I�u�W�F�N�g�𐶐�����Instantiate���Ăяo��(�ʒu�͐����̍��W�����̂܂ܕ��������_�ɂ���)
        _Puyo[pos.y, pos.x] = Instantiate(prefabPuyo, world_postion, Quaternion.identity, transform);
    
        //�Q�[���I�u�W�F�N�g�̐F��ݒ�
        _Puyo[pos.y, pos.x].GetComponent<PuyoController>().SetPuyoType((PuyoType)val);

        return true;
    }
}
