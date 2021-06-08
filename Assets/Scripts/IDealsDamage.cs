using UnityEngine;

public interface IDealsDamage
{
    bool Attack(IAttackable target);
    void OnTargetObjectKilled(IAttackable target);
    
    GameObject GetGameObject();

}