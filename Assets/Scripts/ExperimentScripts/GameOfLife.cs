using UnityEngine;
using UnityEngine.UI;

public class GameOfLife : MonoBehaviour {
    [SerializeField] int width = 100;
    [SerializeField] int height = 60;
    public float updateInterval = 0.5f;
    public Color aliveColor = Color.white;
    public Color deadColor = Color.black;

    private int[,] grid;
    private int[,] nextGrid;
    private Texture2D texture;
    private RawImage rawImage;
    private float timer;

    void Start() {
        rawImage = GetComponent<RawImage>();

        // Setup texture
        texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point; // pixel style
        rawImage.texture = texture;

        grid = new int[width, height];
        nextGrid = new int[width, height];

        // Random initial state
        for ( int x = 0; x < width; x++ ) {
            for ( int y = 0; y < height; y++ ) {
                grid[x, y] = Random.value > 0.8f ? 1 : 0;
            }
        }

        DrawGrid();
    }

    void Update() {
        timer += Time.deltaTime;
        if ( timer >= updateInterval ) {
            timer = 0f;
            Step();
        }
    }

    void Step() {
        for ( int x = 0; x < width; x++ ) {
            for ( int y = 0; y < height; y++ ) {
                int alive = CountAlive(x, y);
                if ( grid[x, y] == 1 ) {
                    nextGrid[x, y] = (alive == 2 || alive == 3) ? 1 : 0;
                }
                else {
                    nextGrid[x, y] = (alive == 3) ? 1 : 0;
                }
            }
        }

        // Swap
        var temp = grid;
        grid = nextGrid;
        nextGrid = temp;

        DrawGrid();
    }

    int CountAlive( int x, int y ) {
        int count = 0;
        for ( int dx = -1; dx <= 1; dx++ ) {
            for ( int dy = -1; dy <= 1; dy++ ) {
                if ( dx == 0 && dy == 0 ) continue;
                int nx = (x + dx + width) % width;
                int ny = (y + dy + height) % height;
                count += grid[nx, ny];
            }
        }
        return count;
    }

    void DrawGrid() {
        for ( int x = 0; x < width; x++ ) {
            for ( int y = 0; y < height; y++ ) {
                texture.SetPixel(x, y, grid[x, y] == 1 ? aliveColor : deadColor);
            }
        }
        texture.Apply();
    }
}