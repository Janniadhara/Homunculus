using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AggressiveMobAttackEnemy : MonoBehaviour
{
    [SerializeField] private BoxCollider weaponHitbox;
    [SerializeField] private List<GameObject> enemiesHittedList;
    public float damage { private get; set; }

    void Start()
    {
        enemiesHittedList = new List<GameObject>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventsManager.Instance.damageEvents.DamagePlayer(damage);
            gameObject.SetActive(false);
        }
    }
    private void HitEnemy(GameObject enemy)
    {
        if (!enemiesHittedList.Contains(enemy))
        {
            enemiesHittedList.Add(enemy);
            Debug.Log(enemy + " hitted");
        }
        else
        {
            Debug.Log(enemy + " already hitted");
        }
    }
}
