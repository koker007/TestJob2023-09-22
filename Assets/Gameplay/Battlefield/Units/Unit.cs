using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public abstract class Unit : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField]
    int Health = 10;
    int HealthNow = 10;
    [SerializeField]
    int moveMax = 3;
    [SerializeField]
    public int damageBasic = 3;
    [SerializeField]
    public int damagePlus = 0; //������� �����
    [SerializeField]
    int initiative = 4;
    [SerializeField]
    Skills.Skill[] skillPrefabs;
    public List<Skills.Skill> skills;
    [SerializeField]
    int count = 1;
    public int Count { get {return count; } }

    public int Initiative { get { return initiative; } }

    public int allianceNum = 0;
    public int lastMoveTick = 0;

    [Header("UI")]
    [SerializeField]
    Image Image;
    [SerializeField]
    TextMeshProUGUI unitCountText;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    RectTransform damageShake;
    float damageShakeTime = 0;
    float deleteTime = 0;
    [SerializeField]
    CanvasGroup canvasGroup;


    Cell cell;
    public Cell myCell { get { return cell; } }

    Stack<Cell> travelPath = new Stack<Cell>();
    Unit UnitTarget; //����

    [Header("Test")]
    [SerializeField]
    Vector2Int posNeed;

    public Unit(Cell cell) {
        this.cell = cell;
    }
    public void Inicialize(Cell cell, int count, int alliance)
    {
        this.cell = cell;
        this.count = count;
        this.allianceNum = alliance;

        HealthNow = Health;

        posNeed = cell.Index;

        lastMoveTick = cell.GetBattleField.TickNow;

        //������ ������� ����� ���� ���������
        RectTransform cellRect = cell.GetComponent<RectTransform>();
        RectTransform myRect = gameObject.GetComponent<RectTransform>();

        Vector3 posCell = cellRect.position;
        myRect.position = posCell;

        IniSkills();

        void IniSkills() {
            //������� ���������� ������ �� ������ ��������
            skills = new List<Skills.Skill>();

            foreach (Skills.Skill prefab in skillPrefabs) {
                if (prefab == null)
                    continue;

                Skills.Skill skill = Instantiate(prefab);
                skill.Inicialize(this);
                skills.Add(skill);

                //�������� �� ���� ��������
                SkillBuffer.Set(skill);
            }
        }
    }

    public void Attack(Unit target) {
        UnitTarget = target;
    }

    //������ ����������� � ��������� ������
    bool Move(Cell target) {
        //���� �����-�� �������� - ������
        if (cell.GetBattleField.IsAction)
            return false;

        //��������� ���� �� ������ ������
        Stack<Cell> bestPath = cell.GetBattleField.GetBestPath(cell, target, this);
        //���� ���� ������ ������������ ��������� �����������
        if (bestPath.Count > moveMax) {
            //�������� ������ �����
            Stack<Cell> pathBuffer = new Stack<Cell>();
            for (int moveNum = 0; moveNum < moveMax; moveNum++) {
                pathBuffer.Push(bestPath.Pop());
            }

            bestPath = new Stack<Cell>();
            for (int moveNum = 0; moveNum < pathBuffer.Count; moveNum++)
            {
                bestPath.Push(pathBuffer.Pop());
            }
        }

        travelPath = bestPath;

        //���� ������� �� ������ - �������
        if (travelPath.Count == 0) {
            return false;
        }

        NextTick();

        //�������� ������ ������
        cell.GetBattleField.ChangeMoveNextAliance();

        cell.GetBattleField.IsAction = true;

        //��������� ���� ���������� �����������
        MoveNextPath();

        return cell.GetBattleField.IsAction;

    }

    //��������� �����-�� ��������
    virtual public bool Action(Cell target) {
        bool isAct = false;

        //���� � ������� ����� ���� ���� �� ����������
        Unit enemy = cell.GetBattleField.GetEnemy(target);
        if (enemy)
            UnitTarget = enemy;

        //���� � ����
        if (Move(target))
            isAct = true;

        return isAct;

    }

    //������� ���� ����� �����
    public void Damage(float damage, Unit From) {

        float damageResult = damage;

        //��������� ���� �� ����� ������������ ������������ ������
        checkSkillDefence();

        //������� ����
        //������ ������� � ����� ��������
        float HealtSum = (count - 1) * Health + HealthNow;

        //�������� ��������
        HealtSum -= damageResult;

        int countOld = count;
        //������ ����� ���������� ������
        count = (int)(HealtSum / Health);
        count = count + 1;

        //� ������� ��������
        HealthNow = (int)(HealtSum % Health);

        int minusCount = 0;
        while (HealthNow < 0)
        {
            HealthNow += Health;
            minusCount++;
        }

        count -= minusCount;

        if (count < 0)
            count = 0;

        int dead = countOld - count;
        checkSkillAfterDie();

        startDamageShake();

        BattleTextPanel.SetMessage(gameObject.name + " Get " + damageResult + " From " + From.name);

        void checkSkillDefence() {
            foreach (Skills.Skill skill in skills) {
                if (skill == null)
                    continue;

                IDefenseSkill defenseSkill = skill as IDefenseSkill;

                if (defenseSkill == null)
                    continue;

                damageResult = defenseSkill.ChangeDamage(damageResult);
            }
        }

        void checkSkillAfterDie() {
            foreach (Skills.Skill skill in skills)
            {
                Skills.IAfterDie afterDieSkill = skill as Skills.IAfterDie;

                if (afterDieSkill == null)
                    continue;

                afterDieSkill.AfterDie(dead);
            }
        }
    }

    public void NextTick() {
        //������ ��� ����� ����� �������� ����� ��������
        lastMoveTick = lastMoveTick + (50 / Initiative);
    }

    public void Add(Unit addUnit) {
        //� ����� �� ����������
        if (count <= 0 || addUnit.count <= 0)
            return;

        //���������� ������ ���� ���������� ��� � ������ �� � ����
        if (isSameType(addUnit, this) && (travelPath != null && travelPath.Count == 0) && 
            (addUnit.travelPath != null && addUnit.travelPath.Count == 0)) 
            UnionStats();


        void UnionStats() {
            count += addUnit.count; 
            addUnit.count = 0;
        }
    }

    public void changeStats(float coof) {
        Health = (int)(Health * coof);
        HealthNow = (int)(HealthNow * coof);
        damageBasic = (int)(damageBasic * coof);
        damagePlus = (int)(damagePlus * coof);
        initiative = (int)(initiative * coof);

        if (Health == 0) Health = 1;
        if (HealthNow == 0) HealthNow = 1;
        if (damageBasic == 0) damageBasic = 1;
        if (damagePlus == 0) damagePlus = 1;
        if (initiative == 0) initiative = 1;

    }

    static public bool isSameType(Unit first, Unit second) {
        UnitArcher addArcher = first as UnitArcher;
        UnitKnight addKnight = first as UnitKnight;
        UnitSkeleton addSkeleton = first as UnitSkeleton;
        UnitZombie addZombie = first as UnitZombie;

        UnitArcher thisArcher = second as UnitArcher;
        UnitKnight thisKnight = second as UnitKnight;
        UnitSkeleton thisSkeleton = second as UnitSkeleton;
        UnitZombie thisZombie = second as UnitZombie;

        //���������� ������ ���� ���������� ���
        if (addArcher != null && thisArcher != null ||
            addKnight != null && thisKnight != null ||
            addSkeleton != null && thisSkeleton != null ||
            addZombie != null && thisZombie != null)
            return true;

        return false;
    }

    public void startDamageShake() {
        damageShakeTime = 1;

        Invoke("invokeDamageShake", 0.05f);
    }
    void invokeDamageShake()
    {
        if (damageShakeTime <= 0)
        {
            damageShake.localPosition = new Vector3();
            return;
        }

        Vector2 randomVector = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        randomVector.Normalize();

        randomVector *= damageShakeTime * 5;
        damageShake.localPosition = new Vector3(randomVector.x, randomVector.y);

        damageShakeTime -= Time.unscaledDeltaTime;

        Invoke("invokeDamageShake", 0.05f);
    }

    //������ ������� ����������� �������
    public void startDelete() {
        //������ ���� ������ ������� �� ������
        lastMoveTick = 99999;
        Invoke("invokeDelete", 0.05f);
    }

    void invokeDelete()
    {
        canvasGroup.alpha = 1 - deleteTime;

        if (canvasGroup.alpha <= 0)
        {
            Destroy(gameObject);
            return;
        }

        deleteTime += Time.unscaledDeltaTime;
        Invoke("invokeDelete", 0.05f);
    }

    void Update()
    {
        TestNeedPosition();
        updateUI();
    }

    void TestNeedPosition() {

        if (posNeed == cell.Index || //���� ������ ������� ��������� ������� �� ��� ��
            travelPath != null && travelPath.Count > 0) //����� ���� ���� ��� ���� �� ���� ��� ��
            return;


        //����� ������ �������
        //��������� ����
        if (Move(cell.GetBattleField.cells[posNeed.x, posNeed.y])) {

        }
        //�� �����-�� ������� ����������� �� ��������, ������
        else{
            posNeed = cell.Index;
        }


    }

    //���� �� �������
    void MoveNextPath() {

        if (travelPath.Count == 0)
            return;

        //��������� � �������
        cell.GetBattleField.IsAction = true;
        //����������� ��������� ������
        Cell next = travelPath.Pop();
        //���� ��� ��� ���������

        //���� ������ �� ��������� ��� �������� - ���������
        bool MoveEnd = false;
        if (!BattleField.isCorrectCellToMove(this, next))
        {
            travelPath = new Stack<Cell>();
            MoveEnd = true;
        }

        if (!MoveEnd)
        {
            cell = next;

            //���� ��� ��� ��������� ����
            //���������� ���� �����
            if(travelPath != null && travelPath.Count == 0)
                cell.GetBattleField.checkPostSkills();

            //������� �� ���� ����� ������ ��� � �������
            Invoke("MoveNextPath", 0.25f);
        }
        //���� �������� ����������� ��������� ���� �� ���� � ������� ���
        else {
            NearAttack();
            //���������� ���� �����
            cell.GetBattleField.checkPostSkills();
        }

        void NearAttack() {
            //��������� ���� �� ���� � �����
            if (UnitTarget == null)
                return;

            List<Cell> nearCells = new List<Cell>();
            nearCells.Add(cell.GetNeigbour(Cell.Side.Left));
            nearCells.Add(cell.GetNeigbour(Cell.Side.Right));
            nearCells.Add(cell.GetNeigbour(Cell.Side.UpL));
            nearCells.Add(cell.GetNeigbour(Cell.Side.UpR));
            nearCells.Add(cell.GetNeigbour(Cell.Side.DownL));
            nearCells.Add(cell.GetNeigbour(Cell.Side.DownR));

            foreach (Cell cell in nearCells)
            {
                if (UnitTarget == null ||
                    cell != UnitTarget.cell)
                    continue;

                //���� ����� ��������� ����
                float resultDamage = damageBasic + Random.Range(damagePlus, 0);
                resultDamage *= count;
                UnitTarget.Damage(resultDamage, this);

                //���� ����� �� �����
                foreach (Skills.Skill skill in skills) {
                    Skills.IAttackSkill attackSkill = skill as Skills.IAttackSkill;
                    if (attackSkill == null)
                        continue;

                    //����������
                    attackSkill.AttackSkill(UnitTarget);
                }

                //���� ������� ������� ���� �����
                UnitTarget = null;
            }
        }
    }

    public void DeleteSkill(Skills.Skill delete) {
        List<Skills.Skill> skillsListNew = new List<Skills.Skill>();

        foreach (Skills.Skill skill in skills) {
            if (skill == null ||
                skill == delete)
                continue;

            skillsListNew.Add(skill);
        }

        skills = skillsListNew;

        Destroy(delete.gameObject);
    }

    void updateUI() {
        CountUnitText();

        MoveToMyCell();
        FlashMoveUnit();

        void CountUnitText() {
            unitCountText.text = count.ToString();
        }

        //�������� �����������
        async Task MoveToMyCell() {
            //������ ������� ����� ���� ���������
            RectTransform cellRect = cell.GetComponent<RectTransform>();
            RectTransform myRect = gameObject.GetComponent<RectTransform>();

            Vector3 posNeed = cellRect.position;
            Vector3 posNow = myRect.position;
            Vector3 posNew = posNow + (posNeed - posNow)*Time.deltaTime * 4;

            myRect.position = posNew;

            canvas.sortingOrder = 2000 - (int)posNew.y/5 - (int)posNew.x/10;

            await Task.CompletedTask;
        }

        //������� ����� ������� ������ ������
        async Task FlashMoveUnit() {
            if (cell.GetBattleField.UnitMoving != this) {
                Image.color = new Color(1,1,1);
                return;
            }

            float impulse = (Time.unscaledTime * 2) % 1;
            impulse = 0.5f + impulse / 2;
            Color color = new Color(impulse, impulse, impulse);
            Image.color = color;
        }
    }
}
