using System.Collections;
using System.Collections.Generic;
using Hyperbyte;
using UnityEngine;
 
namespace ScreenFrameWork {
    public class MainScreen : Screen
    {
        [SerializeField]
        private ContinnueButton continnueButton;
        private void Awake() {
            Application.targetFrameRate = 60;
        }


        override public  void Show(bool back, bool immediate)
        {
            base.Show(back, immediate);
            continnueButton.Show();

        }
    }
}
