using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridAutoFit : MonoBehaviour
{
    public int columns = 3;
    public int rows = 2;

    private GridLayoutGroup grid;

    void Start()
    {
        grid = GetComponent<GridLayoutGroup>();
        UpdateCellSize();
    }

    void Update()
    {
        UpdateCellSize();
    }

    void UpdateCellSize()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;

        float cellWidth = (width - (grid.spacing.x * (columns - 1))) / columns;
        float cellHeight = (height - (grid.spacing.y * (rows - 1))) / rows;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
