﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public struct ScreenShakeData
    {
        public float Strength;
        public float Speed;
        public float Duration;
        public float GlobalTimer;
        public float ShakeTimer;
        public Vector2 Offset;
        public Vector2 SpringForce;
        public bool DecayOverTime;
    }

    private Vector3 _basePos;
    private List<ScreenShakeData> _shakeQueue;
    // Update is called once per frame
    void Start()
    {
        _shakeQueue = new List<ScreenShakeData>();
        _basePos = transform.position;
    }

	void Update () {
        for (int i = 0; i < _shakeQueue.Count; ++i)
        {
            ScreenShakeData data = _shakeQueue[i];
            data.GlobalTimer += Time.deltaTime;
            data.ShakeTimer += Time.deltaTime;

            if (data.ShakeTimer >= 0.05f / data.Speed)
            {
                data.ShakeTimer = 0;
                float strength;

                if (data.DecayOverTime)
                {
                    strength = data.Strength * (1.0f - (data.GlobalTimer / data.Duration));
                }
                else
                {
                    strength = data.Strength;
                }

                data.SpringForce = new Vector2(Random.Range(-strength, strength), Random.Range(-strength, strength));
            }

            data.Offset += data.SpringForce * Time.deltaTime;
            data.Offset -= (data.Offset / 2.0f) * Time.deltaTime * data.Speed * 2.0f;
            data.SpringForce -= (data.SpringForce / 2.0f) * Time.deltaTime * data.Speed * 2.0f;

            _shakeQueue[i] = data;

            transform.position = _basePos;
            Vector3 t = new Vector3(data.Offset.x, data.Offset.y, 0);
            transform.Translate(t);
        }

        for (int i = 0; i < _shakeQueue.Count; ++i)
        {
            ScreenShakeData data = _shakeQueue[i];

            if (data.GlobalTimer > data.Duration)
            {
                _shakeQueue.Remove(_shakeQueue[i]);
            }
        }


        if (_shakeQueue.Count == 0)
        {
            if ((transform.position - _basePos).magnitude > 0.05f)
            {
                transform.Translate(((_basePos - transform.position) / 2.0f) * Time.deltaTime * 20.0f, Space.World);
            }
        }
    }

    public void AddScreenShake(float strength, float speed, float duration, bool decay = false)
    {
        ScreenShakeData data;
        data.Strength = strength;
        data.Speed = speed;
        data.Duration = duration;
        data.GlobalTimer = 0;
        data.ShakeTimer = 0;
        data.DecayOverTime = decay;
        data.Offset = Vector2.zero;
        data.SpringForce = new Vector2(Random.Range(-strength, strength), Random.Range(-strength, strength));
        _shakeQueue.Add(data);
    }
}
