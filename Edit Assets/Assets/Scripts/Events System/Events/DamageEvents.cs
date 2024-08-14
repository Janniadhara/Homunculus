using System;
using UnityEngine;

public class DamageEvents
{
    public event Action<GameObject, int> onDamageAnimal;
    public void DamageAnimal(GameObject animal, int damage)
    {
        if (onDamageAnimal != null)
        {
            onDamageAnimal(animal, damage);
        }
    }
    public event Action<GameObject> onAnimalInHitbox;
    public void AnimalInHitbox(GameObject animal)
    {
        if (onAnimalInHitbox != null)
        {
            onAnimalInHitbox(animal);
        }
    }
    public event Action<float> onDamagePlayer;
    public void DamagePlayer(float damage)
    {
        if (onDamagePlayer != null)
        {
            onDamagePlayer(damage);
        }
    }
    public event Action onPlayerInHitbox;
    public void PlayerInHitbox()
    {
        if (onPlayerInHitbox != null)
        {
            onPlayerInHitbox();
        }
    }
}
