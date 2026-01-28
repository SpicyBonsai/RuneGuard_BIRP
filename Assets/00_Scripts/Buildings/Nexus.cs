using UnityEngine;

public class Nexus : MonoBehaviour
{
    public int health = 10;
    public bool IsAlive => health > 0;
    
    public void TakeDamage()
    {
        health--;
    }
}
