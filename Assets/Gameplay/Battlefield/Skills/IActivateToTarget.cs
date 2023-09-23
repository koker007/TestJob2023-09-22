using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public interface IActivateToTarget
    {

        //Активировать в указанном месте
        public void ActivateToTarget(Cell target);

        //Получить список ячеек попадающих под атаку
        public List<Cell> GetListCells(Cell target);
    }
}
