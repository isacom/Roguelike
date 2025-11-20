using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile[] ObstacleTile;
    public int MaxHealth = 3;

    private int m_HealthPoint;
    private Tile m_OriginalTile;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        m_HealthPoint = MaxHealth;

        m_OriginalTile = GameManager.Instance.BoardManager.GetCellTile(cell);

        Tile chosenTile;

        if (ObstacleTile != null && ObstacleTile.Length > 0)
        {
            int index = Random.Range(0, ObstacleTile.Length);
            chosenTile = ObstacleTile[index];
        }
        else
        {
            Debug.LogWarning("WallObject: ObstacleTile está vacío, no hay tiles de muro asignados.");
            chosenTile = m_OriginalTile;
        }

        GameManager.Instance.BoardManager.SetCellTile(cell, chosenTile);
    }

    public override bool PlayerWantsToEnter()
    {
        m_HealthPoint -= 1;

        if (m_HealthPoint > 0)
        {
            return false;
        }

        GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_OriginalTile);
        Destroy(gameObject);
        return true;
    }
}