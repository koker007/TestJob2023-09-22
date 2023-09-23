using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameData
{
    public class BattleFieldPrefabs : MonoBehaviour
    {
        static BattleFieldPrefabs main;
        static public BattleFieldPrefabs Main { get{ return main;} }

        [SerializeField]
        public Cell CellGO;

        [Header("Units")]
        [SerializeField]
        public UnitKnight unitKnight;
        [SerializeField]
        public UnitArcher unitArcher;
        [SerializeField]
        public UnitSkeleton unitSkeleton;
        [SerializeField]
        public UnitZombie unitZombie;

        private void Awake()
        {
            main ??= this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
