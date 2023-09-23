using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills {
    public class Sprint : Skill, IPostActionSkill
    {
        public void PostActivate()
        {
            //45 процентов откатить ход
            int shance = Random.Range(0, 100);
            if (shance > 45)
                return;

            //¬озвращ€ем тик
            owner.lastMoveTick = owner.lastMoveTick - (50 / owner.Initiative);
            BattleField.Main.AlianceMoveNow = owner.allianceNum;
        }
    }
}
