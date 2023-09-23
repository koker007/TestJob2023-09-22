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
            //��������� ��� � ���� ��� �������� ����������
            foreach (Skill skill in target.skills) {
                if (skill != null && 
                    skill is PupetDoll)
                    return;
            }

            //������� ����������
            PupetDoll skillDoll = Instantiate(pupetDoll);
            skillDoll.Inicialize(target);
            skillDoll.iniPupetDoll(target.allianceNum, owner.allianceNum);
            target.skills.Add(skillDoll);

            //�������� �� ���� ��������
            SkillBuffer.Set(skillDoll);
        }
    }
}
