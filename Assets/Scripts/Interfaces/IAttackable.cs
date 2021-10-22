using UnityEngine;

public interface IAttackable
  {
      bool GetAttacked(DamageSource damageSource); 
      void OnDeath(DamageSource damageSource);

      GameObject GetGameObject();
  }
