using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� �������
public class UnitRange : Unit
{
    [Header("UnitRange")]
    [SerializeField]
    int DistRangeStart; //���������, ����������� ��������� �����
    [SerializeField]
    int DistRangePlus; //��������� ����� ����������� � ���������
    [SerializeField]
    public int RangeDamageBasic;
    [SerializeField]
    public int RangeDamagePlus;
    public UnitRange(Cell cell): base(cell)
    {

    }

    override public bool Action(Cell target)
    {
        bool isAct = false;

        Unit enemy = myCell.GetBattleField.GetEnemy(target);
        //���� � ��������� ������ ����� ���, ����� ������ ���� �� ����
        if (!enemy)
            return base.Action(target);

        //���� ���� ����, ����� ���� ������ ��������� �� ����
        int dist = BattleField.Distance(myCell, enemy.myCell);

        //���� ��������� ������ ��� �����������, � ������ ��� ������������ - ���� �� ����
        if (dist < DistRangeStart || dist >= DistRangeStart + DistRangePlus)
            return base.Action(target);

        //������� ���� �� �������� �������
        float damage = RangeDamageBasic + Random.Range(0, RangeDamagePlus);
        damage *= Count;

        enemy.Damage(damage, this);
        myCell.GetBattleField.ActionIsDone();

        return isAct;

    }
}
