using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class BattleField : MonoBehaviour
{
    static BattleField main;
    static public BattleField Main { get { return main; } }

    //Время последнего действия
    float timeLastAction = 0;
    public bool IsAction
    {
        get {
            //Проиходит ли сейчас какое нибудь действие
            if (timeLastAction + 1 > Time.unscaledTime)
                return true;
            return false;
        }
        set {
            //Сказать что сейчас действие
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
    /// Проверка что следующая ячейка пригодна для перемещения данного юнита
    /// </summary>
    /// <param name="test"></param>
    /// <param name="cell"></param>
    /// <returns></returns>
    static public bool isCorrectCellToMove(Unit test, Cell cell)
    {
        //Если в ячейке уже есть юнит
        Unit[] units = cell.GetBattleField.GetAllUnits();

        bool isCorrectCell = true;
        foreach (Unit unit in units)
        {
            if (unit.myCell != cell)
                continue;

            //Если в ячейке есть юнит нашего альянса и он нашего типа
            //if (unit.allianceNum == test.allianceNum && Unit.isSameType(unit, test) && !uncompatibleTest && !unit.isIncompatibleSkills())
            //    break;


            //у него не наш альянс или не наш тип
            //Значит нельзя туда идти
            isCorrectCell = false;
            break;
        }

        return isCorrectCell;
    }
    /// <summary>
    /// Найти дистанцию между ячейками
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
        //Вычисляем путь до нужной ячейки
        Stack<Cell> pathStack = new Stack<Cell>();

        //Массив дальности ячеек
        int[,] countMove = new int[size.x, size.y];
        //Заменяем все значения на невыгодные
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                countMove[x, y] = 99999;
            }
        }

        //Перебираем ячейки в количестве доступных ходов
        List<Cell> cellsNow = new List<Cell>();
        //Начинаем отсчет маршрута с текущей
        cellsNow.Add(from);
        countMove[from.Index.x, from.Index.y] = 0; //Стартовая ячейка

        bool targetPathFound = false;
        for (int moveNow = 0; moveNow < 30 && !targetPathFound; moveNow++)
        {
            CalcCellsNext(moveNow);
        }

        //Когда возможные пути просчитаны необходимо найти один наиболее верный
        CalcPath();

        return pathStack;

        void CalcCellsNext(int moveNow)
        {
            //Ищем потенциальные ячейки для следующего движения
            List<Cell> cellNext = new List<Cell>();

            //проверяем относительно всех текущих ячеек
            foreach (Cell cellTesting in cellsNow)
            {
                //идем на все 6 сторон
                Cell CellLeft = cellTesting.GetNeigbour(Cell.Side.Left);
                Cell CellRight = cellTesting.GetNeigbour(Cell.Side.Right);

                Cell CellUpLeft = cellTesting.GetNeigbour(Cell.Side.UpL);
                Cell CellUpRight = cellTesting.GetNeigbour(Cell.Side.UpR);

                Cell CellDownLeft = cellTesting.GetNeigbour(Cell.Side.DownL);
                Cell CellDownRight = cellTesting.GetNeigbour(Cell.Side.DownR);

                //Добавляем потенциальные ячейки
                if (isCanMove(cellTesting, CellLeft)) cellNext.Add(CellLeft);
                if (isCanMove(cellTesting, CellRight)) cellNext.Add(CellRight);
                if (isCanMove(cellTesting, CellUpLeft)) cellNext.Add(CellUpLeft);
                if (isCanMove(cellTesting, CellUpRight)) cellNext.Add(CellUpRight);
                if (isCanMove(cellTesting, CellDownLeft)) cellNext.Add(CellDownLeft);
                if (isCanMove(cellTesting, CellDownRight)) cellNext.Add(CellDownRight);
            }

            //Далее ходим от добавленных ячеек
            cellsNow = cellNext;

            //Если одна из следующих ячеек целевая - маршрут построен
            foreach (Cell cell in cellsNow)
            {
                if (cell != to)
                    continue;

                targetPathFound = true;
                break;
            }

            //Проверка что в ячейку можно переместиться
            bool isCanMove(Cell from, Cell to)
            {

                //Если ячейка не существует
                if (to == null)
                    return false;

                int countTo = countMove[to.Index.x, to.Index.y];

                //Если мы там уже ходили
                if (countMove[from.Index.x, from.Index.y] >= countMove[to.Index.x, to.Index.y])
                    return false;

                //Если ячейка не пригодна для перемещения данного юнита
                //Если учитываются препятствия
                if (unit != null && !isCorrectCellToMove(unit, to))
                {
                    return false;
                }

                //Если ячейка существует и если еще через нее не ходили
                //отмечаем сколько надо ходов что до нее добраться
                countMove[to.Index.x, to.Index.y] = moveNow + 1;

                return true;


            }
        }

        void CalcPath()
        {
            //Вычисляем наиболее короткий путь до целевой ячейки

            //Анализируем вокруг цели наиболее перспективный маршрут
            Cell now = to;
            Cell next = null;
            //Добавляем конечную цель
            pathStack.Push(now);

            bool notHavePath = true;
            while (now != null)
            {
                //Анализируем ячейки по круг
                Cell Left = now.GetNeigbour(Cell.Side.Left);
                Cell Right = now.GetNeigbour(Cell.Side.Right);
                Cell UpL = now.GetNeigbour(Cell.Side.UpL);
                Cell UpR = now.GetNeigbour(Cell.Side.UpR);
                Cell DownL = now.GetNeigbour(Cell.Side.DownL);
                Cell DownR = now.GetNeigbour(Cell.Side.DownR);

                //Добавляем перспективные маршруты
                List<Cell> GoodPath = new List<Cell>();
                if (isGoodPath(now, Left)) GoodPath.Add(Left);
                if (isGoodPath(now, Right)) GoodPath.Add(Right);
                if (isGoodPath(now, UpL)) GoodPath.Add(UpL);
                if (isGoodPath(now, UpR)) GoodPath.Add(UpR);
                if (isGoodPath(now, DownL)) GoodPath.Add(DownL);
                if (isGoodPath(now, DownR)) GoodPath.Add(DownR);

                //ВЫбираем лучший
                next = GetBest(GoodPath);

                //идти некуда - выходим
                if (next == null)
                    break;

                //Если путь дальше есть то добавляем в список и переключаемся на него как на текущий для анализа
                now = next;

                //Проверяем это стартовая ячейка?
                if (countMove[now.Index.x, now.Index.y] == 0)
                {
                    //Поиск пути закончен
                    notHavePath = false;
                    break;
                }

                //Добавляем точку пути в список
                pathStack.Push(now);
            }

            //Если расчет пути не завершен
            if (notHavePath)
            {
                pathStack = new Stack<Cell>();
                return;
            }

            bool isGoodPath(Cell from, Cell to)
            {

                if (to == null)
                    return false;

                //Если путь не перспективный
                if (countMove[from.Index.x, from.Index.y] <= countMove[to.Index.x, to.Index.y])
                    return false;

                return true;
            }

            Cell GetBest(List<Cell> goodList)
            {
                Cell best = null;

                foreach (Cell test in goodList)
                {
                    //Если лучшего пока нету
                    if (best == null)
                    {
                        best = test;
                        continue;
                    }

                    //Если лучший путь оказался не лучшим, заменяем
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

        //Если выбрана способность, ходим ею
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

        //Вытаскиваем всех юнитов
        Unit[] units = GetAllUnits();

        //Проверяем есть ли юнит на ячейке
        Unit result = null;
        foreach (Unit unit in units)
        {
            if (unit.myCell != from)
                continue;

            //Нашли врага
            result = unit;
            break;
        }

        return result;
    }
    public Unit GetEnemy(Cell from)
    {
        if (from == null)
            return null;

        //Проверяем враг ли на ячейке
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

        //Выбираем следующего по списку у кого есть армия
        int alianseMoveNew = alianceMoveNow;

        for (int x = 1; x < armyCountInAliance.Length; x++)
        {
            int alianseTest = x + alianceMoveNow;

            if (alianseTest >= armyCountInAliance.Length)
                alianseTest -= armyCountInAliance.Length;

            if (armyCountInAliance[alianseTest] == 0)
                continue;

            //Если армия не ноль, это тот кто нужен
            alianseMoveNew = alianseTest;
            break;

        }

        if (alianceMoveNow != alianseMoveNew)
            alianceMoveNow = alianseMoveNew;

    }

    //Пропустить ход
    public void ActionIsDone()
    {
        unitMoving.NextTick();
        ChangeMoveNextAliance();

        //Активируем пост скилы
        checkPostSkills();
    }

    public void checkPostSkills()
    {
        Debug.Log("checkPostSkills " + Time.unscaledTime);
        //Проверяем послеходовые скилы
        foreach (Skills.Skill skill in unitMoving.skills)
        {
            if (skill == null)
                continue;

            Skills.IPostActionSkill postActionSkill = skill as Skills.IPostActionSkill;
            if (postActionSkill == null)
                continue;

            //Активируем
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
        //Создаем юнитов так как в задании
        int XMax = cells.GetLength(0) - 1;

        //Создаем рыцарей
        int knightMax = 60;
        int knightInStack = knightMax / 3;

        UnitKnight unitKnight1 = Instantiate(GameData.BattleFieldPrefabs.Main.unitKnight, ParentUnits);
        UnitKnight unitKnight2 = Instantiate(GameData.BattleFieldPrefabs.Main.unitKnight, ParentUnits);
        UnitKnight unitKnight3 = Instantiate(GameData.BattleFieldPrefabs.Main.unitKnight, ParentUnits);

        unitKnight1.Inicialize(cells[0, 4], knightInStack, 0);
        unitKnight2.Inicialize(cells[1, 3], knightInStack, 0);
        unitKnight3.Inicialize(cells[0, 2], knightInStack, 0);

        
        //Создаем лучников
        UnitArcher unitArcher = Instantiate(GameData.BattleFieldPrefabs.Main.unitArcher, ParentUnits);

        int archerCount = 40;
        unitArcher.Inicialize(cells[0, 3], archerCount, 0);

        //Создаем скелетов
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

        //Создаем зомби
        UnitZombie unitZombie = Instantiate(GameData.BattleFieldPrefabs.Main.unitZombie, ParentUnits);

        int zombieCount = 1;
        unitZombie.Inicialize(cells[XMax, 3], zombieCount, 1);

        //Создаем смерть
        UnitDeath unitDeath = Instantiate(GameData.BattleFieldPrefabs.Main.unitDeath, ParentUnits);

        int deathCount = 1;
        unitDeath.Inicialize(cells[XMax, 0], deathCount, 1);

        //Создаем песеля
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

        //Проверяем можно ли объединить воинов
        checkUnitsUnion();
        checkDeleteZero();

        MovePlayers();

        void MovePlayers()
        {
            //Если како-то юнит в действии
            //Пропуск
            if (IsAction)
                return;

            //Проверяем что все персонажи текущего альянса сходили
            Unit[] unitList = GetAllUnits();

            Unit selectNew = null;

            //Есть ли тикающий юнит
            bool isHaveTickUnit = false;

            foreach (Unit unit in unitList)
            {

                UnitZombie unitZombie = unit as UnitZombie;
                if (unitZombie != null)
                {
                    bool test = false;
                }

                //Считаем пришло ли время этого юнита
                int iniciative = unit.Initiative;
                if (iniciative == 0)
                    iniciative = 1;

                int tickTime = unit.lastMoveTick + (50 / iniciative);

                //Если время не пришло пропускаем юнита
                if (tickNow < tickTime)
                    continue;

                //Новый тик не нужен
                isHaveTickUnit = true;

                //Если это юнит этого альянса и кем надо ходить еще не выбрали
                if (unit.allianceNum == alianceMoveNow && selectNew == null) {
                    //Выбираем юнита в качестве ходящего
                    unitMoving = unit;
                    selectNew = unit;
                }
            }

            //Проверяем тик 
            //Если тикующие юниты есть
            if (!isHaveTickUnit)
                tickNow++;
            //Если тикующие юниты есть но не выбранно кем ходить, меняем альянс
            else if (selectNew == null) {
                ChangeMoveNextAliance();
            }
            
        }


        //Проверка объединения одного типа воина
        void checkUnitsUnion() {
            Unit[] units = GetAllUnits();

            foreach(Unit unit in units) {
                if (unitMoving == null)
                    break;

                if (unit == unitMoving ||
                    unit.myCell != unitMoving.myCell ||
                    unit.allianceNum != unitMoving.allianceNum)
                    continue;

                //Пытаемся объединить
                unit.Add(unitMoving);
            }
        }
        void checkDeleteZero() {
            //ищем мертвую армию чтобы удалить
            Unit[] units = GetAllUnits();

            foreach (Unit unit in units) {
                if (unit.Count > 0)
                    continue;

                unit.startDelete();
            }
        }

        //Визуализация зоны действия активного скила
        void VisualizeSkillZone() {
            if (!activateSkill)
                return;

            Skills.IActivateToTarget ActivateSkill = activateSkill as Skills.IActivateToTarget;
            if (!activateSkill)
                return;

            //Получаем зону выделения скила
            List<Cell> zone = ActivateSkill.GetListCells(Cell.EnterCell);

            //Обновляем выделение зоны
            foreach (Cell cell in zone) {
                if(cell)
                    cell.SelectedWithSkill();
            }

        }

        void VisualizeWinPanel() {
            //Провеяем сколько фракций
            Unit[] units = GetAllUnits();

            int countAlianse = 0;
            int indexAliande = 0;
            for (int numAniance = 0; numAniance < 10; numAniance++) {
                //ищем фракцию этого номера
                foreach (Unit unit in units) {
                    if (unit.allianceNum != numAniance)
                        continue;

                    countAlianse++;
                    //Запоминаем индекс найденного альянса
                    indexAliande = numAniance;
                    break;
                }

                //Если уже больше 1 фракции победы не будет
                if (countAlianse > 1)
                    break;
            }


            if (countAlianse > 1) {
                VictoryPanel.CloseWinPanel();
                return;
            }


            //Если после поиска фракция все еще одна
            //Запускаем победу
            VictoryPanel.SetVictory(indexAliande);
            IsAction = this;
        }
    }
}
