using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Юнит дальник
public class UnitRange : Unit
{
    [Header("UnitRange")]
    [SerializeField]
    int DistRangeStart; //начальная, минимальная дальность атаки
    [SerializeField]
    int DistRangePlus; //дистанция атаки добавленная к начальной
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
        //Если в указанной ячейке врага нет, тогда просто идет по базе
        if (!enemy)
            return base.Action(target);

        //Если враг есть, тогда надо узнать растояние до него
        int dist = BattleField.Distance(myCell, enemy.myCell);

        //Если дистанция маньше чем минимальная, и больше чем максимальная - идет по базе
        if (dist < DistRangeStart || dist >= DistRangeStart + DistRangePlus)
            return base.Action(target);

        //наносим урон по дальнему шаблону
        float damage = RangeDamageBasic + Random.Range(0, RangeDamagePlus);
        damage *= Count;

        enemy.Damage(damage, this);
        myCell.GetBattleField.ActionIsDone();

        return isAct;

    }
}
