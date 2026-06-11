using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GFW.Gameplay
{
    public class GameScene : Singleton<GameScene>
    {
        private GamePawn pawn;
        public GamePawn Pawn => pawn;
        private GameController controller;
        public GameController Controller => controller;
        private GameHUD hud;
        public GameHUD Hud => hud;

        private MonoBehaviour camera;
        public MonoBehaviour Camera => camera;
        //private void Awake()
        //{
        //    Init(this.pawn, this.controller, this.hud);
        //}

        public void Init(GameController controller,GamePawn pawn,MonoBehaviour camera,GameHUD hud)
        {
            if (controller!=null)
            {
                this.controller = controller;
            }
            if (pawn!=null)
            {
                this.pawn = pawn;
                this.controller.SetPawn(pawn);
            }
            if (camera!=null)
            {
                this .camera = camera;
            }
            if (hud!=null)
            {
                this.hud = hud;
                this.controller.SetHud(hud);
            }
            
        }
    }
}
