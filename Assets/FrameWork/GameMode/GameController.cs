using System;
using UnityEngine;
namespace GFW.Gameplay
{
    public class GameController : MonoBehaviour
    {

        [HideInInspector]
        public GamePawn target { get; private set; }//控制目标

        [HideInInspector]
        public GameHUD hud { get; private set; }

        public event Action<GamePawn> onControlledPawnChanged;
        

        protected virtual void OnControlledPawnChanged()
        {
            onControlledPawnChanged?.Invoke(target);
        }
        public void SetPawn(GamePawn obj)
        {
            if (!ReferenceEquals(target, obj))
            {
                target=obj;
                OnControlledPawnChanged();
            }
        }

        public void SetHud(GameHUD hud)
        {
            this.hud=hud;
        }
    }
}
