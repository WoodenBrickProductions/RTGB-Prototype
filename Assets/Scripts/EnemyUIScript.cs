using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIScript : HealthBarScript
{
    // Start is called before the first frame update

    private Text enemyName;

    void Awake()
    {
        enemyName = GetComponentInChildren<Text>();
    }

    public void SetName(string name)
    {
        enemyName.text = name;
    }
}
