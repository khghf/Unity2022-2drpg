using Spine.Unity;
using UnityEngine;
[CreateAssetMenu(fileName ="New Spine Anims",menuName ="Character/SpineAnims")]
public class SpineAnims : ScriptableObject
{
    public AnimationReferenceAsset Idle ;
    public AnimationReferenceAsset Run ;
    public AnimationReferenceAsset CommonAttack;
}
