using UnityEngine;

public class TurnManager
{
    public bool IsPaused { get; private set; }
    public event System.Action OnTick;
    private int m_TurnCount;
    
    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }
    public TurnManager()
    {
        m_TurnCount = 1;
    }

    public void Tick()
    {
        if (IsPaused)
            return;
        m_TurnCount += 1;
        Debug.Log("Current turn count : " + m_TurnCount);
        OnTick?.Invoke();
    }
}
