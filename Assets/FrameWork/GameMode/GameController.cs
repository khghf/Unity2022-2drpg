using System;
using UnityEngine;
namespace GFW.Gameplay
{
    public class GameController : MonoBehaviour
    {

        [HideInInspector]
        public GamePawn Target { get; private set; }//控制目标

        [HideInInspector]
        public GameHUD Hud { get; private set; }

        public event Action<GamePawn> OnControlledPawnChanged;
        
        public void SetPawn(GamePawn obj)
        {
            Target=obj;
            OnControlledPawnChanged?.Invoke(Target);
        }

        public void SetHud(GameHUD hud)
        {
            Hud=hud;
        }
    }
}
