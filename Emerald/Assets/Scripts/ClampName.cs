using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClampName : MonoBehaviour
{
    private Camera _camera;
    public TMP_Text nameLabel;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = _camera.WorldToScreenPoint(transform.position);
        nameLabel.transform.position = pos;
    }
}
