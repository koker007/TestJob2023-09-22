using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;

public class SkillBuffer : MonoBehaviour
{
    static SkillBuffer main;

    RectTransform rect;

    private void Awake()
    {
        main ??= this;
        rect = gameObject.GetComponent<RectTransform>();
    }

    static public void Set(Skill skill) {
        skill.transform.parent = main.transform;

        RectTransform SkillRect = skill.GetComponent<RectTransform>();
        SkillRect.localPosition = new Vector3();
    }
}
