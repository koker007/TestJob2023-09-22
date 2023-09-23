using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public interface IActivateToTarget
    {

        //������������ � ��������� �����
        public void ActivateToTarget(Cell target);

        //�������� ������ ����� ���������� ��� �����
        public List<Cell> GetListCells(Cell target);
    }
}
