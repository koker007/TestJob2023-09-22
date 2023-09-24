using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class BattleField : MonoBehaviour
{
    static BattleField main;
    static public BattleField Main { get { return main; } }

    //����� ���������� ��������
    float timeLastAction = 0;
    public bool IsAction
    {
        get {
            //��������� �� ������ ����� ������ ��������
            if (timeLastAction + 1 > Time.unscaledTime)
                return true;
            return false;
        }
        set {
            //������� ��� ������ ��������
            if (value)
                timeLastAction = Time.unscaledTime;
            else timeLastAction = 0;
        }

    }

    Vector2Int size;
    public Vector2Int Size { get { return size; } }

    [SerializeField]
    Transform ParentCells;
    [SerializeField]
    Transform ParentUnits;

    public Cell[,] cells;

    int alianceMoveNow = 0;
    public int AlianceMoveNow { set { alianceMoveNow = value; } }

    int tickNow = 0;
    public int TickNow { get { return tickNow; } }

    Unit unitMoving;
    public Unit UnitMoving { get { return unitMoving; } }

    [SerializeField]
    public Skills.PreActivateSkill activateSkill;

    /// <summary>
    /// �������� ��� ��������� ������ �������� ��� ����������� ������� �����
    /// </summary>
    /// <param name="test"></param>
    /// <param name="cell"></param>
    /// <returns></returns>
    static public bool isCorrectCellToMove(Unit test, Cell cell)
    {
        //���� � ������ ��� ���� ����
        Unit[] units = cell.GetBattleField.GetAllUnits();

        bool isCorrectCell = true;
        foreach (Unit unit in units)
        {
            if (unit.myCell != cell)
                continue;

            //���� � ������ ���� ���� ������ ������� � �� ������ ����
            //if (unit.allianceNum == test.allianceNum && Unit.isSameType(unit, test) && !uncompatibleTest && !unit.isIncompatibleSkills())
            //    break;


            //� ���� �� ��� ������ ��� �� ��� ���
            //������ ������ ���� ����
            isCorrectCell = false;
            break;
        }

        return isCorrectCell;
    }
    /// <summary>
    /// ����� ��������� ����� ��������
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    static public int Distance(Cell first, Cell second)
    {
        Stack<Cell> path = main.GetBestPath(first, second);

        int distance = -1;
        if (path != null)
            distance = path.Count;
        //

        return distance;
    }


    public Stack<Cell> GetBestPath(Cell from, Cell to, Unit unit = null)
    {
        //��������� ���� �� ������ ������
        Stack<Cell> pathStack = new Stack<Cell>();

        //������ ��������� �����
        int[,] countMove = new int[size.x, size.y];
        //�������� ��� �������� �� ����������
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                countMove[x, y] = 99999;
            }
        }

        //���������� ������ � ���������� ��������� �����
        List<Cell> cellsNow = new List<Cell>();
        //�������� ������ �������� � �������
        cellsNow.Add(from);
        countMove[from.Index.x, from.Index.y] = 0; //��������� ������

        bool targetPathFound = false;
        for (int moveNow = 0; moveNow < 30 && !targetPathFound; moveNow++)
        {
            CalcCellsNext(moveNow);
        }

        //����� ��������� ���� ���������� ���������� ����� ���� �������� ������
        CalcPath();

        return pathStack;

        void CalcCellsNext(int moveNow)
        {
            //���� ������������� ������ ��� ���������� ��������
            List<Cell> cellNext = new List<Cell>();

            //��������� ������������ ���� ������� �����
            foreach (Cell cellTesting in cellsNow)
            {
                //���� �� ��� 6 ������
                Cell CellLeft = cellTesting.GetNeigbour(Cell.Side.Left);
                Cell CellRight = cellTesting.GetNeigbour(Cell.Side.Right);

                Cell CellUpLeft = cellTesting.GetNeigbour(Cell.Side.UpL);
                Cell CellUpRight = cellTesting.GetNeigbour(Cell.Side.UpR);

                Cell CellDownLeft = cellTesting.GetNeigbour(Cell.Side.DownL);
                Cell CellDownRight = cellTesting.GetNeigbour(Cell.Side.DownR);

                //��������� ������������� ������
                if (isCanMove(cellTesting, CellLeft)) cellNext.Add(CellLeft);
                if (isCanMove(cellTesting, CellRight)) cellNext.Add(CellRight);
                if (isCanMove(cellTesting, CellUpLeft)) cellNext.Add(CellUpLeft);
                if (isCanMove(cellTesting, CellUpRight)) cellNext.Add(CellUpRight);
                if (isCanMove(cellTesting, CellDownLeft)) cellNext.Add(CellDownLeft);
                if (isCanMove(cellTesting, CellDownRight)) cellNext.Add(CellDownRight);
            }

            //����� ����� �� ����������� �����
            cellsNow = cellNext;

            //���� ���� �� ��������� ����� ������� - ������� ��������
            foreach (Cell cell in cellsNow)
            {
                if (cell != to)
                    continue;

                targetPathFound = true;
                break;
            }

            //�������� ��� � ������ ����� �������������
            bool isCanMove(Cell from, Cell to)
            {

                //���� ������ �� ����������
                if (to == null)
                    return false;

                int countTo = countMove[to.Index.x, to.Index.y];

                //���� �� ��� ��� ������
                if (countMove[from.Index.x, from.Index.y] >= countMove[to.Index.x, to.Index.y])
                    return false;

                //���� ������ �� �������� ��� ����������� ������� �����
                //���� ����������� �����������
                if (unit != null && !isCorrectCellToMove(unit, to))
                {
                    return false;
                }

                //���� ������ ���������� � ���� ��� ����� ��� �� ������
                //�������� ������� ���� ����� ��� �� ��� ���������
                countMove[to.Index.x, to.Index.y] = moveNow + 1;

                return true;


            }
        }

        void CalcPath()
        {
            //��������� �������� �������� ���� �� ������� ������

            //����������� ������ ���� �������� ������������� �������
            Cell now = to;
            Cell next = null;
            //��������� �������� ����
            pathStack.Push(now);

            bool notHavePath = true;
            while (now != null)
            {
                //����������� ������ �� ����
                Cell Left = now.GetNeigbour(Cell.Side.Left);
                Cell Right = now.GetNeigbour(Cell.Side.Right);
                Cell UpL = now.GetNeigbour(Cell.Side.UpL);
                Cell UpR = now.GetNeigbour(Cell.Side.UpR);
                Cell DownL = now.GetNeigbour(Cell.Side.DownL);
                Cell DownR = now.GetNeigbour(Cell.Side.DownR);

                //��������� ������������� ��������
                List<Cell> GoodPath = new List<Cell>();
                if (isGoodPath(now, Left)) GoodPath.Add(Left);
                if (isGoodPath(now, Right)) GoodPath.Add(Right);
                if (isGoodPath(now, UpL)) GoodPath.Add(UpL);
                if (isGoodPath(now, UpR)) GoodPath.Add(UpR);
                if (isGoodPath(now, DownL)) GoodPath.Add(DownL);
                if (isGoodPath(now, DownR)) GoodPath.Add(DownR);

                //�������� ������
                next = GetBest(GoodPath);

                //���� ������ - �������
                if (next == null)
                    break;

                //���� ���� ������ ���� �� ��������� � ������ � ������������� �� ���� ��� �� ������� ��� �������
                now = next;

                //��������� ��� ��������� ������?
                if (countMove[now.Index.x, now.Index.y] == 0)
                {
                    //����� ���� ��������
                    notHavePath = false;
                    break;
                }

                //��������� ����� ���� � ������
                pathStack.Push(now);
            }

            //���� ������ ���� �� ��������
            if (notHavePath)
            {
                pathStack = new Stack<Cell>();
                return;
            }

            bool isGoodPath(Cell from, Cell to)
            {

                if (to == null)
                    return false;

                //���� ���� �� �������������
                if (countMove[from.Index.x, from.Index.y] <= countMove[to.Index.x, to.Index.y])
                    return false;

                return true;
            }

            Cell GetBest(List<Cell> goodList)
            {
                Cell best = null;

                foreach (Cell test in goodList)
                {
                    //���� ������� ���� ����
                    if (best == null)
                    {
                        best = test;
                        continue;
                    }

                    //���� ������ ���� �������� �� ������, ��������
                    if (countMove[best.Index.x, best.Index.y] > countMove[test.Index.x, test.Index.y])
                    {
                        best = test;
                    }
                }

                return best;
            }
        }
    }

    public void ClickToCell(Cell clicked)
    {
        if (Cell.EnterCell != clicked && IsAction)
            return;

        //���� ������� �����������, ����� ��
        if (activateSkill)
        {
            Skills.IActivateToTarget skill = activateSkill as Skills.IActivateToTarget;
            skill.ActivateToTarget(clicked);
            activateSkill = null;

            ActionIsDone();
            return;
        }

        UnitMoving.Action(clicked);
    }

    public Unit GetUnit(Cell from)
    {
        if (from == null)
            return null;

        //����������� ���� ������
        Unit[] units = GetAllUnits();

        //��������� ���� �� ���� �� ������
        Unit result = null;
        foreach (Unit unit in units)
        {
            if (unit.myCell != from)
                continue;

            //����� �����
            result = unit;
            break;
        }

        return result;
    }
    public Unit GetEnemy(Cell from)
    {
        if (from == null)
            return null;

        //��������� ���� �� �� ������
        Unit targetEnemy = null;
        Unit unit = GetUnit(from);
        if (unit != null &&
            unit.allianceNum != unitMoving.allianceNum)
        {
            targetEnemy = unit;
        }

        return targetEnemy;
    }

    public Unit CreateUnit(Unit prefab, Cell target, int count, int aliance)
    {
        if (prefab == null || target == null)
            return null;

        Unit unit = Instantiate(prefab, ParentUnits);
        unit.Inicialize(target, count, aliance);

        return unit;
    }
    public void ChangeMoveNextAliance()
    {
        Unit[] units = GetAllUnits();

        int[] armyCountInAliance = new int[10];

        foreach (Unit unit in units)
        {
            armyCountInAliance[unit.allianceNum]++;
        }

        //�������� ���������� �� ������ � ���� ���� �����
        int alianseMoveNew = alianceMoveNow;

        for (int x = 1; x < armyCountInAliance.Length; x++)
        {
            int alianseTest = x + alianceMoveNow;

            if (alianseTest >= armyCountInAliance.Length)
                alianseTest -= armyCountInAliance.Length;

            if (armyCountInAliance[alianseTest] == 0)
                continue;

            //���� ����� �� ����, ��� ��� ��� �����
            alianseMoveNew = alianseTest;
            break;

        }

        if (alianceMoveNow != alianseMoveNew)
            alianceMoveNow = alianseMoveNew;

    }

    //���������� ���
    public void ActionIsDone()
    {
        unitMoving.NextTick();
        ChangeMoveNextAliance();

        //���������� ���� �����
        checkPostSkills();
    }

    public void checkPostSkills()
    {
        Debug.Log("checkPostSkills " + Time.unscaledTime);
        //��������� ������������ �����
        foreach (Skills.Skill skill in unitMoving.skills)
        {
            if (skill == null)
                continue;

            Skills.IPostActionSkill postActionSkill = skill as Skills.IPostActionSkill;
            if (postActionSkill == null)
                continue;

            //����������
            postActionSkill.PostActivate();
        }
    }

    public Unit[] GetAllUnits()
    {
        return ParentUnits.GetComponentsInChildren<Unit>();
    }

    private void Awake()
    {
        main ??= this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateBattlefield();
        CreateUnits();
    }

    void CreateBattlefield() {
        size = new Vector2Int(10, 8);

        cells = new Cell[size.x, size.y];

        Cell prefabCell = BattleFieldPrefabs.Main.CellGO;
        RectTransform PrefabRect = prefabCell.GetComponent<RectTransform>();
        float correctSizeX = PrefabRect.sizeDelta.x * 0.98f;
        float correctSizeY = PrefabRect.sizeDelta.y * 0.73f;
        float halfOffsetX = correctSizeX / 2;

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++)
            {
                cells[x, y] = Instantiate(prefabCell, ParentCells);
                cells[x, y].Inicialize(this, new Vector2Int(x,y));
                RectTransform rect = cells[x, y].GetComponent<RectTransform>();

                float OffsetX = halfOffsetX;
                if (y % 2 == 0)
                    OffsetX = 0;

                rect.localPosition = new Vector3(correctSizeX * x - OffsetX, correctSizeY * y);
            }
        }

        
    }
    void CreateUnits() {
        //������� ������ ��� ��� � �������
        int XMax = cells.GetLength(0) - 1;

        //������� �������
        int knightMax = 60;
        int knightInStack = knightMax / 3;

        UnitKnight unitKnight1 = Instantiate(GameData.BattleFieldPrefabs.Main.unitKnight, ParentUnits);
        UnitKnight unitKnight2 = Instantiate(GameData.BattleFieldPrefabs.Main.unitKnight, ParentUnits);
        UnitKnight unitKnight3 = Instantiate(GameData.BattleFieldPrefabs.Main.unitKnight, ParentUnits);

        unitKnight1.Inicialize(cells[0, 4], knightInStack, 0);
        unitKnight2.Inicialize(cells[1, 3], knightInStack, 0);
        unitKnight3.Inicialize(cells[0, 2], knightInStack, 0);

        
        //������� ��������
        UnitArcher unitArcher = Instantiate(GameData.BattleFieldPrefabs.Main.unitArcher, ParentUnits);

        int archerCount = 40;
        unitArcher.Inicialize(cells[0, 3], archerCount, 0);

        //������� ��������
        int SceletonCount = 200;
        int SceletonInStack = SceletonCount / 4;

        UnitSkeleton unitSkeleton1 = Instantiate(GameData.BattleFieldPrefabs.Main.unitSkeleton, ParentUnits);
        UnitSkeleton unitSkeleton2 = Instantiate(GameData.BattleFieldPrefabs.Main.unitSkeleton, ParentUnits);
        UnitSkeleton unitSkeleton3 = Instantiate(GameData.BattleFieldPrefabs.Main.unitSkeleton, ParentUnits);
        UnitSkeleton unitSkeleton4 = Instantiate(GameData.BattleFieldPrefabs.Main.unitSkeleton, ParentUnits);

        unitSkeleton1.Inicialize(cells[XMax, 1], SceletonInStack, 1);
        unitSkeleton2.Inicialize(cells[XMax, 2], SceletonInStack, 1);
        unitSkeleton3.Inicialize(cells[XMax, 4], SceletonInStack, 1);
        unitSkeleton4.Inicialize(cells[XMax, 5], SceletonInStack, 1);

        //������� �����
        UnitZombie unitZombie = Instantiate(GameData.BattleFieldPrefabs.Main.unitZombie, ParentUnits);

        int zombieCount = 1;
        unitZombie.Inicialize(cells[XMax, 3], zombieCount, 1);

        //������� ������
        UnitDeath unitDeath = Instantiate(GameData.BattleFieldPrefabs.Main.unitDeath, ParentUnits);

        int deathCount = 1;
        unitDeath.Inicialize(cells[XMax, 0], deathCount, 1);

        //������� ������
        UnitDog unitDog = Instantiate(GameData.BattleFieldPrefabs.Main.unitDog, ParentUnits);

        int dogCount = 1;
        unitDog.Inicialize(cells[1, 0], dogCount, 0);

    }



    // Update is called once per frame
    void Update()
    {
        Gameplay();
    }

    void Gameplay() {
        VisualizeSkillZone();
        VisualizeWinPanel();

        //��������� ����� �� ���������� ������
        checkUnitsUnion();
        checkDeleteZero();

        MovePlayers();

        void MovePlayers()
        {
            //���� ����-�� ���� � ��������
            //�������
            if (IsAction)
                return;

            //��������� ��� ��� ��������� �������� ������� �������
            Unit[] unitList = GetAllUnits();

            Unit selectNew = null;

            //���� �� �������� ����
            bool isHaveTickUnit = false;

            foreach (Unit unit in unitList)
            {

                UnitZombie unitZombie = unit as UnitZombie;
                if (unitZombie != null)
                {
                    bool test = false;
                }

                //������� ������ �� ����� ����� �����
                int iniciative = unit.Initiative;
                if (iniciative == 0)
                    iniciative = 1;

                int tickTime = unit.lastMoveTick + (50 / iniciative);

                //���� ����� �� ������ ���������� �����
                if (tickNow < tickTime)
                    continue;

                //����� ��� �� �����
                isHaveTickUnit = true;

                //���� ��� ���� ����� ������� � ��� ���� ������ ��� �� �������
                if (unit.allianceNum == alianceMoveNow && selectNew == null) {
                    //�������� ����� � �������� ��������
                    unitMoving = unit;
                    selectNew = unit;
                }
            }

            //��������� ��� 
            //���� �������� ����� ����
            if (!isHaveTickUnit)
                tickNow++;
            //���� �������� ����� ���� �� �� �������� ��� ������, ������ ������
            else if (selectNew == null) {
                ChangeMoveNextAliance();
            }
            
        }


        //�������� ����������� ������ ���� �����
        void checkUnitsUnion() {
            Unit[] units = GetAllUnits();

            foreach(Unit unit in units) {
                if (unitMoving == null)
                    break;

                if (unit == unitMoving ||
                    unit.myCell != unitMoving.myCell ||
                    unit.allianceNum != unitMoving.allianceNum)
                    continue;

                //�������� ����������
                unit.Add(unitMoving);
            }
        }
        void checkDeleteZero() {
            //���� ������� ����� ����� �������
            Unit[] units = GetAllUnits();

            foreach (Unit unit in units) {
                if (unit.Count > 0)
                    continue;

                unit.startDelete();
            }
        }

        //������������ ���� �������� ��������� �����
        void VisualizeSkillZone() {
            if (!activateSkill)
                return;

            Skills.IActivateToTarget ActivateSkill = activateSkill as Skills.IActivateToTarget;
            if (!activateSkill)
                return;

            //�������� ���� ��������� �����
            List<Cell> zone = ActivateSkill.GetListCells(Cell.EnterCell);

            //��������� ��������� ����
            foreach (Cell cell in zone) {
                if(cell)
                    cell.SelectedWithSkill();
            }

        }

        void VisualizeWinPanel() {
            //�������� ������� �������
            Unit[] units = GetAllUnits();

            int countAlianse = 0;
            int indexAliande = 0;
            for (int numAniance = 0; numAniance < 10; numAniance++) {
                //���� ������� ����� ������
                foreach (Unit unit in units) {
                    if (unit.allianceNum != numAniance)
                        continue;

                    countAlianse++;
                    //���������� ������ ���������� �������
                    indexAliande = numAniance;
                    break;
                }

                //���� ��� ������ 1 ������� ������ �� �����
                if (countAlianse > 1)
                    break;
            }


            if (countAlianse > 1) {
                VictoryPanel.CloseWinPanel();
                return;
            }


            //���� ����� ������ ������� ��� ��� ����
            //��������� ������
            VictoryPanel.SetVictory(indexAliande);
            IsAction = this;
        }
    }
}
