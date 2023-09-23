using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleTextPanel : MonoBehaviour
{
    static BattleTextPanel main;

    [SerializeField]
    TextMeshProUGUI[] texts;

    

    private void Awake()
    {
        main ??= this;
        Clear();
    }

    static public void SetMessage(string message) {
        //Перебираем тексты
        for (int num = main.texts.Length - 1; num >= 0; num--) {
            if (num > 0)
                main.texts[num].text = main.texts[num - 1].text;
            else main.texts[num].text = message;
        }
    }

    static void Clear() {
        //Перебираем тексты
        for (int num = 0; num < main.texts.Length; num++)
            main.texts[num].text = string.Empty;
    }
}
