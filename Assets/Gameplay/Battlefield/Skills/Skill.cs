using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    //База пассивного скила
    public class Skill : MonoBehaviour
    {
        [SerializeField]
        Image image;
        [SerializeField]
        Button button;

        
        [SerializeField]
        public bool Isincompatible = false; //Можно ли соединяться с обладателем этой способности?


        //Владелец скила
        protected Unit owner;

        [Header("Test")]
        [SerializeField]
        protected int countActivate = 0;

        public void Inicialize(Unit owner) {
            this.owner = owner;

            IniButton();

            void IniButton() {
                IActivateSkill activateSkill = this as IActivateSkill;
                IActivateToTarget activateToTarget = this as IActivateToTarget;

                if (activateSkill == null && activateToTarget == null)
                    SetInteractable();
                else SetNonInteractable();

                void SetInteractable() {
                    button.interactable = false;

                    image.color = new Color(1, 1, 1, 0.8f);
                }
                void SetNonInteractable() {
                    button.interactable = true;
                    image.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
                }
            }
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