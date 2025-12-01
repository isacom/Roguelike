using UnityEngine;

public class FoodObject : CellObject
{
    [HideInInspector] public int prefabIndex;
    public int AmountGranted = 10;
    private AudioManager sm;

    private void Awake()
    {
        sm = FindObjectOfType<AudioManager>();
    }
    public override void PlayerEntered()
    {
        Destroy(gameObject);
    
        //increase food
        GameManager.Instance.ChangeFood(AmountGranted);
        if (AmountGranted == 10)
        {
            sm.PlaySoda();
        }
        else if (AmountGranted == 5)
        {
            sm.PlayFruit();
        }
    }
}