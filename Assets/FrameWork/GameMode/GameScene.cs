using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GFW.Gameplay
{
    public class GameScene : Singleton<GameScene>
    {
        private GamePawn pawn;
        public GamePawn Pawn => pawn;
        [SerializeField]
        private GameController controller;
        public GameController Controller => controller;
        [SerializeField]
        private GameHUD hud;
        public GameHUD Hud => hud;

        [SerializeField]
        private Camera gameCamera;
        public Camera GameCamera => gameCamera;

        //private void Awake()
        //{
        //    Init(this.pawn, this.controller, this.hud);
        //}

        public void Init(GameController controller,GamePawn pawn, Camera camera,GameHUD hud)
        {

            this.controller = controller;
            this.pawn = pawn;
            this.gameCamera = camera;
            this.hud = hud;
            this.controller?.SetPawn(pawn);
            this.controller?.SetHud(hud);
        }
    }
}
