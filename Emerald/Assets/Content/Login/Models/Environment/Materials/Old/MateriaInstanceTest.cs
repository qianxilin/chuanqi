using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MateriaInstanceTest : MonoBehaviour
{
	public GameObject go;
	public Color color;
	public Material material;
    // Start is called before the first frame update
    void Start()
    {
        go = this.gameObject;
	material = go.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.color = color;
    }
}
