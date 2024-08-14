using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingWeapon : MonoBehaviour
{
    [SerializeField] private Transform WeaponHolsterSlot;
    [SerializeField] private Transform WeaponHandSlot;
    [SerializeField] private BoxCollider HitBox;
    [SerializeField] private List<GameObject> enemiesHittedList;
    public GameObject CurrentWeapon;

    private void OnEnable()
    {
        EventsManager.Instance.playerStateEvent.onBeginAttack += StartAttack;
        EventsManager.Instance.playerStateEvent.onEndAttack += EndAttack;
    }
    private void OnDisable()
    {
        EventsManager.Instance.playerStateEvent.onBeginAttack -= StartAttack;
        EventsManager.Instance.playerStateEvent.onEndAttack -= EndAttack;
    }
    void Start()
    {
        HitBox.enabled = false;
        enemiesHittedList = new List<GameObject>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            HitEnemy(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            HitEnemy(other.gameObject);
        }
    }
    private void HitEnemy(GameObject enemy)
    {
        if(!enemiesHittedList.Contains(enemy))
        {
            enemiesHittedList.Add(enemy);
            EventsManager.Instance.damageEvents.AnimalInHitbox(enemy);
            Debug.Log(enemy + " hitted");
        }
        else
        {
            Debug.Log(enemy + " already hitted");
        }
    }
    void StartAttack()
    {
        HitBox.enabled = true;
    }
    void EndAttack()
    {
        enemiesHittedList.Clear();
        HitBox.enabled = false;
    }
}
