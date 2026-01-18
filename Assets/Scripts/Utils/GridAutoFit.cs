using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridAutoFit : MonoBehaviour {
    [SerializeField] int columns = 3;
    [SerializeField] int rows = 2;

    [SerializeField] bool updateInGame;
    RectTransform rt;

    private GridLayoutGroup grid;

    void Start() {
        grid = GetComponent<GridLayoutGroup>();
        rt = GetComponent<RectTransform>();
        UpdateCellSize();
    }

    void Update() {
        if ( updateInGame ) UpdateCellSize();
    }

    void UpdateCellSize() {
        if ( updateInGame ) rt = GetComponent<RectTransform>();

        float width = rt.rect.width;
        float height = rt.rect.height;

        float cellWidth = (width - (grid.spacing.x * (columns - 1))) / columns;
        float cellHeight = (height - (grid.spacing.y * (rows - 1))) / rows;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
