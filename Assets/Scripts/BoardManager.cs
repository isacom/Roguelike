using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // ====== Tipos internos ======

    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    [Serializable]
    public class ObjectData
    {
        public int prefabIndex;
        public Vector2Int cell;
    }

    [Serializable]
    public class LevelData
    {
        public Vector2Int playerCell;
        public List<ObjectData> walls   = new List<ObjectData>();
        public List<ObjectData> foods   = new List<ObjectData>();
        public List<ObjectData> enemies = new List<ObjectData>();
        public int foodCount;
    }

    // ====== Campos de tablero / tiles ======

    private List<Vector2Int> m_EmptyCellsList;
    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public FoodObject[] FoodPrefab;
    public WallObject[] WallPrefab;
    public EnemyObject[] EnemyPrefab;
    public ExitCellObject ExitCellPrefab;

    public int foodCount;
    public int maxWalls;
    public int minWalls;
    public int enemyCount;

    // Datos actuales del nivel (lo que se guarda)
    public static LevelData currentLevelData = new LevelData();

    // Flag global para saber si, al entrar en la escena,
    // hay que cargar un nivel guardado o generar uno nuevo
    public static bool ShouldLoadSavedLevel = false;

    // ====== SaveSystem interno ======

    public static class SaveSystem
    {
        static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        public static void SaveLevel(LevelData data, string fileName, int food)
        {
            currentLevelData.foodCount = food;
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetPath(fileName), json);
            Debug.Log("Nivel guardado en " + GetPath(fileName));
        }

        public static LevelData LoadLevel(string fileName)
        {
            string path = GetPath(fileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning("No existe archivo: " + path);
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<LevelData>(json);
        }
    }

    // ====== API pública de guardado/carga ======

    public void SaveCurrentLevel(int food)
    {
        SaveSystem.SaveLevel(currentLevelData, "nivel1.json", food);
    }

    // Guardar la celda del jugador
    public void SpawnPlayer(Vector2Int playerCell)
    {
        currentLevelData.playerCell = playerCell;
    }

    /// <summary>
    /// Carga el nivel completo desde un archivo JSON.
    /// Limpia el tablero, reconstruye el suelo/bordes,
    /// instancia paredes, enemigos, comida y recoloca al jugador.
    /// </summary>
    public void LoadLevelFromFile(string fileName = "nivel1.json")
    {
        // Limpia el tablero actual (si hay algo)
        Clean();

        LevelData data = SaveSystem.LoadLevel(fileName);
        if (data == null)
            return;

        currentLevelData = data;

        // Reconstruir tablero base (suelo + bordes + salida)
        BuildBaseBoard();

        // 1) PAREDES
        foreach (var w in data.walls)
        {
            var newWall = Instantiate(WallPrefab[w.prefabIndex]);
            AddObject(newWall, w.cell);
        }

        // 2) ENEMIGOS
        foreach (var e in data.enemies)
        {
            var newEnemy = Instantiate(EnemyPrefab[e.prefabIndex]);
            AddObject(newEnemy, e.cell);
        }

        // 3) COMIDA
        foreach (var f in data.foods)
        {
            var newFood = Instantiate(FoodPrefab[f.prefabIndex]);
            AddObject(newFood, f.cell);
        }

        // 4) Contador de comida
        foodCount = data.foodCount;

        // 5) Recolocar al jugador
        if (GameManager.Instance != null && GameManager.Instance.PlayerController != null)
        {
            GameManager.Instance.PlayerController.Spawn(this, data.playerCell);
        }

        Debug.Log("Nivel cargado desde JSON");
    }

    // Wrapper por si quieres seguir usando el nombre antiguo
    public void LoadLevel()
    {
        LoadLevelFromFile("nivel1.json");
    }

    // ====== Inicialización de niveles nuevos ======

    /// <summary>
    /// Inicializa un nivel nuevo, generación procedural.
    /// </summary>
    public void Init()
    {
        // Reinicia los datos del nivel
        currentLevelData = new LevelData();

        BuildBaseBoard();

        GenerateWall();
        GenerateFood();
        GenerateEnemy();
    }

    /// <summary>
    /// Construye solo el tablero base (suelo, bordes, salida),
    /// rellena m_BoardData y m_EmptyCellsList.
    /// </summary>
    void BuildBaseBoard()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();
        m_BoardData = new CellData[Width, Height];

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;

                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        // Reservar (1,1) para el jugador
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));

        // Salida en esquina inferior derecha
        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);
    }

    // ====== Utilidades de tablero ======

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    // ====== Generadores aleatorios ======

    void GenerateFood()
    {
        for (int i = 0; i < foodCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0)
                break;

            int randomFood = Random.Range(0, FoodPrefab.Length);
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);

            FoodObject newFood = Instantiate(FoodPrefab[randomFood]);
            AddObject(newFood, coord);

            // Guardar en LevelData
            currentLevelData.foods.Add(new ObjectData
            {
                prefabIndex = randomFood,
                cell = coord
            });
        }
    }

    void GenerateWall()
    {
        int wallCount = Random.Range(minWalls, maxWalls);
        for (int i = 0; i < wallCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0)
                break;

            int randomWall = Random.Range(0, WallPrefab.Length);
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);

            WallObject newWall = Instantiate(WallPrefab[randomWall]);
            AddObject(newWall, coord);

            currentLevelData.walls.Add(new ObjectData
            {
                prefabIndex = randomWall,
                cell = coord
            });
        }
    }

    void GenerateEnemy()
    {
        for (int i = 0; i < enemyCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0)
                break;

            int randomEnemy = Random.Range(0, EnemyPrefab.Length);
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);

            EnemyObject newEnemy = Instantiate(EnemyPrefab[randomEnemy]);
            AddObject(newEnemy, coord);

            currentLevelData.enemies.Add(new ObjectData
            {
                prefabIndex = randomEnemy,
                cell = coord
            });
        }
    }

    // ====== Limpiar tablero ======

    public void Clean()
    {
        if (m_BoardData == null || m_Tilemap == null)
            return;

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }
}
