using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryPanel : MonoBehaviour
{
    static VictoryPanel main;

    [SerializeField]
    GameObject fon;
    [SerializeField]
    TextMeshProUGUI Text;

    private void Awake()
    {
        main ??= this;
    }

    struct Texts {
        public const string strVictory1 = "Player 1 (left) win";
        public const string strVictory2 = "Player 2 (right) win";
    }

    static public void SetVictory(int numAliance) {
        if (main.gameObject.activeSelf)
            return;

        main.gameObject.SetActive(true);

        switch (numAliance) {
            case 0: main.Text.text = Texts.strVictory1; break;
            case 1: main.Text.text = Texts.strVictory2; break;
            case 2: main.Text.text = "test"; break;
        }
    }

    static public void CloseWinPanel() {
        if (!main.gameObject.activeSelf)
            return;

        main.gameObject.SetActive(false);
    }
}
