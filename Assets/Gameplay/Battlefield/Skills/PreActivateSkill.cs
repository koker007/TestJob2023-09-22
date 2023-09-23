using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills {
    public class PreActivateSkill : Skill
    {
        public void StartPreActivate()
        {
            if (BattleField.Main.activateSkill != this)
                BattleField.Main.activateSkill = this;
            else BattleField.Main.activateSkill = null;
        }
    }
}
