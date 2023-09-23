using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class Rebirth : Skill, IAfterDie
    {
        public void AfterDie(int countDead)
        {
            //Сперва ищем префаб аналогичного юнита
            Unit MyPrefab = null;

            if (owner is UnitArcher)
                MyPrefab = GameData.BattleFieldPrefabs.Main.unitArcher;
            else if (owner is UnitKnight)
                MyPrefab = GameData.BattleFieldPrefabs.Main.unitKnight;
            else if (owner is UnitSkeleton)
                MyPrefab = GameData.BattleFieldPrefabs.Main.unitSkeleton;
            else if (owner is UnitZombie)
                MyPrefab = GameData.BattleFieldPrefabs.Main.unitZombie;

            if (MyPrefab == null)
                return;

            //Если найден нужный префаб, возраждаем рядом трупы
            Cell cell;

            Cell L = owner.myCell.GetNeigbour(Cell.Side.Left);
            Cell R = owner.myCell.GetNeigbour(Cell.Side.Right);
            Cell UpL = owner.myCell.GetNeigbour(Cell.Side.UpL);
            Cell UpR = owner.myCell.GetNeigbour(Cell.Side.UpR);
            Cell DownL = owner.myCell.GetNeigbour(Cell.Side.DownL);
            Cell DownR = owner.myCell.GetNeigbour(Cell.Side.DownR);

            Unit UnitL = owner.myCell.GetBattleField.GetUnit(L);
            Unit UnitR = owner.myCell.GetBattleField.GetUnit(R);
            Unit UnitUpL = owner.myCell.GetBattleField.GetUnit(UpL);
            Unit UnitUpR = owner.myCell.GetBattleField.GetUnit(UpR);
            Unit UnitDownL = owner.myCell.GetBattleField.GetUnit(DownL);
            Unit UnitDownR = owner.myCell.GetBattleField.GetUnit(DownR);


            List<Cell> potencialCells = new List<Cell>();
            if (UnitL == null && L != null) potencialCells.Add(L);
            if (UnitR == null && R != null) potencialCells.Add(R);
            if (UnitUpL == null && UpL != null) potencialCells.Add(UpL);
            if (UnitUpR == null && UpR != null) potencialCells.Add(UpR);
            if (UnitDownL == null && DownL != null) potencialCells.Add(DownL);
            if (UnitDownR == null && DownR != null) potencialCells.Add(DownR);

            if (potencialCells.Count == 0)
                return;

            int index = Random.Range(0, potencialCells.Count);
            Cell targetCell = potencialCells[index];

            Unit reunit = owner.myCell.GetBattleField.CreateUnit(MyPrefab, targetCell, countDead, owner.allianceNum);

            if (reunit == null)
                return;
            //Необходимо найти и удалить этот скил
            for (int num = 0; num < reunit.skills.Count; num++) {
                if (reunit.skills[num] != null && reunit.skills[num] is Rebirth)
                {
                    reunit.DeleteSkill(reunit.skills[num]);
                    //Skill skill = reunit.skills[num];
                    //reunit.skills.Remove(skill);
                   // Destroy(skill.gameObject);

                    break;
                }


            }

            if(countActivate == 0)
                reunit.changeStats(1.0f - 0.3f);
            if (countActivate == 1)
                reunit.changeStats(1.0f - 0.6f);
            if (countActivate == 2)
                reunit.changeStats(1.0f - 0.9f);
            

            reunit.NextTick();

            countActivate++;
            if (countActivate >= 3)
                Destroy(gameObject);
        }

        bool IsMyPrefab<T>(Unit prefab) {
            if (owner is T && prefab is T)
                return true;

            return false;
        }
    }

}