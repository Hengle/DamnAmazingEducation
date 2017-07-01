﻿using UnityEngine;
[RequireComponent(typeof(Renderer))]

public class TextureOffsetController : MonoBehaviour {

    public float xOffsetSpeed = 0.0f, yOffsetSpeed = 0.0f;
    public bool active = false;

    private Renderer _rend = null;
    private Vector2 _offset = Vector2.zero;

    void Start() {
        _rend = GetComponent<Renderer>();
    }

	void Update () {
        if (!active) return;
        Debug.Log(_offset);
        _offset += new Vector2(xOffsetSpeed, yOffsetSpeed);
        _rend.material.SetTextureOffset("_MainTex", _offset);
	}
}
