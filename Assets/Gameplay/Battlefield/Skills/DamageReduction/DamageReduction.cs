using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class DamageReduction : Skill, IDefenseSkill
    {
        public float ChangeDamage(float damage) {
            float damageResult = damage;

            if (countActivate == 0)
                damageResult *= (1.0f - 0.99f);
            else if (countActivate == 1)
                damageResult *= (1.0f - 0.66f);
            else if (countActivate == 2)
                damageResult *= (1.0f - 0.33f);
            else {
                Destroy(gameObject,0.1f);
            }

            countActivate++;

            return damageResult;
        }
    }
}
