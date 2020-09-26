using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutputMapGrid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor)
        {
            AstarPath path = GameObject.Find("A*").GetComponent<AstarPath>();
            if (path != null)
            {
                Scene scene = SceneManager.GetActiveScene();

                using (BinaryWriter writer = new BinaryWriter(File.Open(@".\Maps\" + scene.name + ".umap", FileMode.Create)))
                {
                    writer.Write(path.data.gridGraph.width);
                    writer.Write(path.data.gridGraph.depth);
                    for (int y = path.data.gridGraph.depth - 1; y >= 0; y--)
                        for (int x = 0; x < path.data.gridGraph.width; x++)
                        {
                            writer.Write(path.data.gridGraph.nodes[path.data.gridGraph.width * y + x].Walkable);
                            if (path.data.gridGraph.nodes[path.data.gridGraph.width * y + x].Walkable)
                            {
                                writer.Write(path.data.gridGraph.nodes[path.data.gridGraph.width * y + x].position.x / 1000f);
                                writer.Write(path.data.gridGraph.nodes[path.data.gridGraph.width * y + x].position.y / 1000f);
                                writer.Write(path.data.gridGraph.nodes[path.data.gridGraph.width * y + x].position.z / 1000f);
                                Debug.Log("saving x: " + path.data.gridGraph.nodes[path.data.gridGraph.width * y + x].position.x / 1000f + ", y: " + path.data.gridGraph.nodes[path.data.gridGraph.width * y + x].position.y / 1000f + ", z: " + path.data.gridGraph.nodes[path.data.gridGraph.width * y + x].position.z / 1000f);
                            }
                        }
                }
            }

        }
    }
}
