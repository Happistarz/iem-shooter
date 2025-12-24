using UnityEngine;
using UnityEngine.UI;

public class HealthUIComponent : MonoBehaviour
{
    public Image healthBarFill;

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBarFill && maxHealth > 0)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }
}