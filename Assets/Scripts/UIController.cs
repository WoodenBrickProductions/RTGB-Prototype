using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController uiController;
    // Start is called before the first frame update

    private HealthBarScript _playerHealthBar;
    private EnemyUIScript _enemyHealthBar;
    private PlayerController _playerController;
    private UnitController _currentEnemy;
    
    private void Awake()
    {
        uiController = this;
        _playerHealthBar = GetComponentInChildren<HealthBarScript>();
        _enemyHealthBar = GetComponentInChildren<EnemyUIScript>();
        _enemyHealthBar.gameObject.SetActive(false);
    }

    private void Start()
    {
        _playerController = PlayerController._playerController;
        _playerHealthBar.SetMaxHealth(_playerController.GetUnitStats().maxHealth);
    }

    public void SetHealth(UnitController unitController)
    {
        switch (unitController.tag)
        {
            case "Player":
            {
                _playerHealthBar.SetHealth(unitController.GetUnitStats().currentHealth);
            }
                break;
            case "Enemy":
            {
                if (_currentEnemy != unitController)
                {
                    _enemyHealthBar.gameObject.SetActive(true);
                    _enemyHealthBar.SetName(unitController.name);
                    _currentEnemy = unitController;
                    _enemyHealthBar.SetMaxHealth(_currentEnemy.GetUnitStats().maxHealth);
                }
                _enemyHealthBar.SetHealth(unitController.GetUnitStats().currentHealth);
            }
                break;
        }
    }

    
    
    public void OnEnemyKilled(UnitController unitController)
    {
        _enemyHealthBar.gameObject.SetActive(false);
    }
}
