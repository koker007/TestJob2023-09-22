using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills {
    public class DoubleFlower : PreActivateSkill, IActivateToTarget
    {
        [Header("Parameters")]
        [SerializeField]
        float CoofDamage = 0.5f;

        public void ActivateToTarget(Cell target)
        {
            //��������� �������� ����� �������?
            UnitRange ownRange = owner as UnitRange;

            //���� �����
            float damage = 0;
            if (ownRange != null) {
                damage = ownRange.RangeDamageBasic + Random.Range(0, ownRange.RangeDamageBasic);
            }
            else{
                damage = owner.damageBasic + Random.Range(0, owner.damagePlus);
            }

            damage *= owner.Count;

            //������� ������ ���� ����� ���������� ��� �����
            List<Cell> damageCells = GetListCells(target);

            //���� ���� ������ ����������� � ���� �������
            Unit[] units = owner.myCell.GetBattleField.GetAllUnits();

            foreach (Unit unit in units) {
                if (unit.allianceNum == owner.allianceNum)
                    continue;

                //��������� ������ �� ������� ���� �������� �� �� � ���� �����
                foreach (Cell cell in damageCells) {
                    if (cell != unit.myCell)
                        continue;

                    //���������
                    unit.Damage(damage, owner);
                    break;
                }
            }

            //��������� ���
            BattleField.Main.ActionIsDone();
        }

        public List<Cell> GetListCells(Cell target)
        {
            List<Cell> cells = new List<Cell>();

            cells.Add(target);
            cells.Add(target.GetNeigbour(Cell.Side.Left));
            cells.Add(target.GetNeigbour(Cell.Side.Right));
            cells.Add(target.GetNeigbour(Cell.Side.UpL));
            cells.Add(target.GetNeigbour(Cell.Side.UpR));
            cells.Add(target.GetNeigbour(Cell.Side.DownL));
            cells.Add(target.GetNeigbour(Cell.Side.DownR));

            //���� c ����� ���������
            if (target.Index.y + 2 < target.GetBattleField.cells.GetLength(1)) {
                Cell secondCenter = target.GetBattleField.cells[target.Index.x, target.Index.y + 2];

                cells.Add(secondCenter);
                cells.Add(secondCenter.GetNeigbour(Cell.Side.Left));
                cells.Add(secondCenter.GetNeigbour(Cell.Side.Right));
                cells.Add(secondCenter.GetNeigbour(Cell.Side.UpL));
                cells.Add(secondCenter.GetNeigbour(Cell.Side.UpR));
            }
            //����� ��������� �����
            else if (target.Index.y - 2 >= 0) {
                Cell secondCenter = target.GetBattleField.cells[target.Index.x, target.Index.y - 2];

                cells.Add(secondCenter);
                cells.Add(secondCenter.GetNeigbour(Cell.Side.Left));
                cells.Add(secondCenter.GetNeigbour(Cell.Side.Right));
                cells.Add(secondCenter.GetNeigbour(Cell.Side.DownL));
                cells.Add(secondCenter.GetNeigbour(Cell.Side.DownR));
            }

            //������ �������� ������ ������
            return cells;
        }
    }
}
