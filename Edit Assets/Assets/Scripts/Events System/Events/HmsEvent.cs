using System;

public class HmsEvent
{
    public event Action<float, float> onChangeHealth;
    public void ChangeHealth(float healthGain, float maxHealth)
    {
        if (onChangeHealth != null)
        {
            onChangeHealth(healthGain, maxHealth);
        }
    }
    public event Action<float, float> onChangeMana;
    public void ChangeMana(float manaGain, float maxMana)
    {
        if (onChangeMana != null)
        {
            onChangeMana(manaGain, maxMana);
        }
    }
    public event Action<float, float> onChangeStamina;
    public void ChangeStamina(float staminaGain, float maxStamina)
    {
        if (onChangeStamina != null)
        {
            onChangeStamina(staminaGain, maxStamina);
        }
    }
}
