using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    const int cellsWidth = 8;
    const int cellsHeight = 8;
    public MirItemCell[,] Cells = new MirItemCell[cellsWidth, cellsHeight];
    public GameObject CellObject;
    public GameObject CellsLocation;

    void Awake()
    {
        GameManager.GameScene.Inventory = this;

        for (int y = 0; y < cellsHeight; y++)
        {
            for (int x = 0; x < cellsWidth; x++)
            {
                GameObject cell = Instantiate(CellObject, CellsLocation.transform);
                Cells[x, y] = cell.GetComponentInChildren<MirItemCell>();
                Cells[x, y].ItemSlot = 6 + (y * cellsHeight + x);
                Cells[x, y].GridType = MirGridType.Inventory;
                RectTransform rt = cell.GetComponent<RectTransform>();
                rt.localPosition = new Vector3(x * 44, -(y * 44), 0);
            }
        }
    }
}
