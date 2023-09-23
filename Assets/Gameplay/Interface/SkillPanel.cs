using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Skills;

public class SkillPanel : MonoBehaviour
{
    static SkillPanel main;

    Unit select;

    private void Awake()
    {
        main ??= this;
    }

    public void Update()
    {
        UpdatePanel();
    }
    async Task UpdatePanel() {
        BattleField battleField = BattleField.Main;
        
        //Изменения не требуются
        if (select == battleField.UnitMoving)
            return;

        select = battleField.UnitMoving;

        Clear();
        Draw();


        await Task.CompletedTask;

        void Clear() {
            Skill[] skills = gameObject.GetComponentsInChildren<Skill>();

            foreach (Skill skill in skills) {
                SkillBuffer.Set(skill);
            }
        }
        void Draw() {
            int num = 0;
            foreach (Skill skill in select.skills) {
                if (skill == null)
                    continue;

                skill.transform.parent = transform;
                RectTransform rect = skill.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(0, -rect.sizeDelta.y/2, 0);

                rect.pivot = new Vector2(-num, 0.5f);

                num++;
            }
        }
    }
}
