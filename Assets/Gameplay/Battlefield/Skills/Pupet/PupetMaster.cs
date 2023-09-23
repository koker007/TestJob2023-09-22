using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class PupetMaster : Skill, IAttackSkill
    {
        [Header("PupetMaster")]
        [SerializeField]
        PupetDoll pupetDoll;

        public void AttackSkill(Unit target)
        {
            //Проверяем что у цели нет активной марионетки
            foreach (Skill skill in target.skills) {
                if (skill != null && 
                    skill is PupetDoll)
                    return;
            }

            //Создаем марионетку
            PupetDoll skillDoll = Instantiate(pupetDoll);
            skillDoll.Inicialize(target);
            skillDoll.iniPupetDoll(target.allianceNum, owner.allianceNum);
            target.skills.Add(skillDoll);

            //Спрятать от глаз подальше
            SkillBuffer.Set(skillDoll);
        }
    }
}
