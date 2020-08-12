using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cell
{
    public bool walkable;
    public Vector3 position;
}

public class MirScene : MonoBehaviour
{
    private int Width;
    private int Height;
    public Cell[,] Cells;

    public GameObject UserObject;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.CurrentScene = this;

        string fileName = Path.Combine(Directory.GetCurrentDirectory(), "Maps", gameObject.scene.name + ".map");
        if (File.Exists(fileName))
        {
            byte[] fileBytes = File.ReadAllBytes(fileName);
            int offSet = 0;
            Width = BitConverter.ToInt32(fileBytes, offSet);
            offSet += 4;
            Height = BitConverter.ToInt32(fileBytes, offSet);
            offSet += 4;
            Cells = new Cell[Width, Height];

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    if (!BitConverter.ToBoolean(fileBytes, offSet))
                    {
                        offSet++;
                        Cells[x, y] = new Cell() { walkable = false }; //Can Fire Over.
                        continue;
                    }

                    Cells[x, y] = new Cell() { walkable = true };
                    Cells[x, y].position = new Vector3();

                    offSet++;
                    Cells[x, y].position.x = BitConverter.ToSingle(fileBytes, offSet);

                    offSet += 4;
                    Cells[x, y].position.y = BitConverter.ToSingle(fileBytes, offSet);

                    offSet += 4;
                    Cells[x, y].position.z = BitConverter.ToSingle(fileBytes, offSet);

                    offSet += 4;
                }
        }
    }
}
