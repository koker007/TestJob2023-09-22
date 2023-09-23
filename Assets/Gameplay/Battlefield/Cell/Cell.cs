using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    static Cell enterCell;
    static public Cell EnterCell { get { return enterCell; } }

    //Поле в котором находится ячейка
    BattleField battleField;
    public BattleField GetBattleField { get { return battleField; } }
    Vector2Int index;
    public Vector2Int Index { get { return index; } }

    [SerializeField]
    Image EnterMouseImage;
    bool isEnterMouse;
    [SerializeField]
    Image SkillSelectImage;
    float SkillSelectLastTime;

    public void Inicialize(BattleField battleField, Vector2Int index)
    {
        this.battleField = battleField;
        this.index = index;
    }

    //Стороны гекса
    public enum Side
    {
        Left = 1,
        Right = 2,
        UpL = 3,
        UpR = 4,
        DownL = 5,
        DownR = 6,
    }

    //получить соседа текущей ячейки
    public Cell GetNeigbour(Side side)
    {

        Cell result = null;

        switch (side)
        {
            case Side.Left: Left(); break;
            case Side.Right: Right(); break;
            case Side.UpL: UpLeft(); break;
            case Side.UpR: UpRight(); break;
            case Side.DownL: DownLeft(); break;
            case Side.DownR: DownRight(); break;
        }

        return result;

        void Left()
        {
            Vector2Int indexNeed = new Vector2Int(index.x - 1, index.y);

            if (indexNeed.x < 0)
                return;

            result = battleField.cells[indexNeed.x, indexNeed.y];

        }
        void Right()
        {
            Vector2Int indexNeed = new Vector2Int(index.x + 1, index.y);

            if (indexNeed.x >= battleField.cells.GetLength(0))
                return;

            result = battleField.cells[indexNeed.x, indexNeed.y];
        }
        void UpLeft()
        {

            //Проверяем четность Y
            bool parity = false;
            if (index.y % 2 == 0)
                parity = true;

            //вычисляем нужный индекс
            Vector2Int indexNeed = new Vector2Int(index.x, index.y + 1);

            //Если нечетное то отнимаем X
            if (!parity)
                indexNeed.x--;

            //проверка границ
            if (indexNeed.x < 0 ||
                indexNeed.y >= battleField.cells.GetLength(1))
                return;

            result = battleField.cells[indexNeed.x, indexNeed.y];
        }
        void UpRight()
        {
            //Проверяем четность Y
            bool parity = false;
            if (index.y % 2 == 0)
                parity = true;

            //вычисляем нужный индекс
            Vector2Int indexNeed = new Vector2Int(index.x, index.y + 1);

            //Если четное
            if (parity)
                indexNeed.x++;

            //проверка границ
            if (indexNeed.x >= battleField.cells.GetLength(0) ||
                indexNeed.y >= battleField.cells.GetLength(1))
                return;

            result = battleField.cells[indexNeed.x, indexNeed.y];
        }
        void DownLeft()
        {
            //Проверяем четность Y
            bool parity = false;
            if (index.y % 2 == 0)
                parity = true;

            //вычисляем нужный индекс
            Vector2Int indexNeed = new Vector2Int(index.x, index.y - 1);

            //Если нечетное
            if (!parity)
                indexNeed.x--;

            //проверка границ
            if (indexNeed.x < 0 ||
                indexNeed.y < 0)
                return;

            result = battleField.cells[indexNeed.x, indexNeed.y];
        }
        void DownRight()
        {
            //Проверяем четность Y
            bool parity = false;
            if (index.y % 2 == 0)
                parity = true;

            //вычисляем нужный индекс
            Vector2Int indexNeed = new Vector2Int(index.x, index.y - 1);

            //Если нечетное
            if (parity)
                indexNeed.x++;

            //проверка границ
            if (indexNeed.x >= battleField.cells.GetLength(0) ||
                indexNeed.y < 0)
                return;

            result = battleField.cells[indexNeed.x, indexNeed.y];
        }
    }
    public void SelectedWithSkill()
    {
        SkillSelectLastTime = Time.unscaledTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateEnterImage();
        updateSelectSkill();
    }

    void updateEnterImage() {
        //Если какойто юнит ходит или это не наша ячейка
        if (enterCell != this || battleField.IsAction)
        {
            EnterMouseImage.gameObject.SetActive(false);
        }
        else {
            EnterMouseImage.gameObject.SetActive(true);
        }

    }

    void updateSelectSkill() {

        if (SkillSelectLastTime == Time.unscaledTime)
            SkillSelectImage.gameObject.SetActive(true);
        else SkillSelectImage.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enterCell = this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        battleField.ClickToCell(this);
    }
}
