using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class PupetDoll : Skill, IPostActionSkill
    {
        int alianceBasic = 0;

        public void iniPupetDoll(int alianceBasic, int alianceNeed) {
            this.alianceBasic = alianceBasic;
            owner.allianceNum = alianceNeed;
        }

        public void PostActivate()
        {
            //���������� ��� �������
            owner.allianceNum = alianceBasic;

            //����������� �������
            owner.DeleteSkill(this);
        }
    }
}
