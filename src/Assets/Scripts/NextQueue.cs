using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextQueue
{
   private enum Constants
    {
        PUYO_TIPE_MAX=4,
        PUYO_NEXT_HISTORIES=2,
    };

    Queue<Vector2Int> _nexts = new();

    Vector2Int CreateNext()
    {
        return new Vector2Int(
            Random.Range(0, (int)Constants.PUYO_TIPE_MAX) + 1,
            Random.Range(0, (int)Constants.PUYO_TIPE_MAX) + 1);
    }

    public void Initialize()
    {
        for(int t=0;t<(int)Constants.PUYO_NEXT_HISTORIES;t++)
        {
            _nexts.Enqueue(CreateNext());
        }
    }

    public Vector2Int Update()
    {
        Vector2Int next = _nexts.Dequeue();
        _nexts.Enqueue(CreateNext());

        return next;
    }

    //�L���[�ɓo�^����Ă���v�f�����ԂɃR�[���o�b�N�֐��ŌĂяo��
    public void Each(System.Action<int,Vector2Int>cb)
    {
        int idx = 0;
        foreach(Vector2Int n in _nexts)
        {
            cb(idx++, n);
        }
    }
}
