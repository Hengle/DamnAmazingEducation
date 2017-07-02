﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{

    [Header("WallShape")]
    [SerializeField]
    float WallWidth = 10;
    [SerializeField]
    float WallDepth = 10;

    [Header("ThrusterSettings")]
    [SerializeField]
    Thruster ThrusterPrefab;
    [SerializeField]
    uint NumThrusters = 3;

    public List<Thruster> LeftThrusters;
    public List<Thruster> RightThrusters;

    [Header("Movement")]
    [SerializeField]
    float MovementMultiplier = 1;
    [SerializeField]
    float MovementDampener = 1;
    private float _movement = 0;

    [Header("Upgrading")]
    [SerializeField]
    float UpgradeMultiplier = 10;
    [SerializeField]
    float UpgradeTime = 10;
    private float _upgradeTimer = 0;
    public float UpgradeTimer
    {
        get
        {
            return UpgradeTime - _upgradeTimer;
        }
    }
    private uint _upgradeLevel = 0;
    [SerializeField]
    GameObject UpgradePrefab;

    [Header("EndGame")]
    [SerializeField]
    float FieldWidth;
    [SerializeField]
    float MaxFieldWidth;
    [SerializeField]
    Explosion ExplosionPrefab;
    public bool LeftWon = false;
    public bool RightWon = false;

    private CameraController _camera;

	// Use this for initialization
	void Start ()
    {
        float PosOffsetDelta = WallWidth / (NumThrusters);

        LeftThrusters = new List<Thruster>();
        for (int i = 0; i < NumThrusters; i++)
        {
            var instance = Instantiate(ThrusterPrefab, 
                new Vector3(-WallWidth*0.5f+PosOffsetDelta*0.5f+i*PosOffsetDelta, transform.position.y, -WallDepth*0.5f), 
                Quaternion.Euler(new Vector3(0, 0, 0)), this.transform);
            Vector3 scale = instance.transform.localScale;
            scale.z = -scale.z;
            instance.transform.localScale = scale;
            instance.ThrustParticles.transform.rotation = Quaternion.Euler(0, 180, 0);
            LeftThrusters.Add(instance);
        }

        RightThrusters = new List<Thruster>();
        for (int i = 0; i < NumThrusters; i++)
        {
            var instance = Instantiate(ThrusterPrefab, 
                new Vector3(-WallWidth * 0.5f + PosOffsetDelta *0.5f+i*PosOffsetDelta, transform.position.y, WallDepth*0.5f), 
                Quaternion.Euler(new Vector3(0, 0, 0)), this.transform);
            RightThrusters.Add(instance);
        }

        _camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
	}
	
	void Update ()
    {
        //Move Wall
        float leftVolume = 0;
        float rightVolume = 0;

        float wallForce = 0;
        for (int i = 0; i < NumThrusters; i++)
        {
            wallForce += LeftThrusters[i].Thrust;
            leftVolume += LeftThrusters[i].Thrust;

            wallForce -= RightThrusters[i].Thrust;
            rightVolume += RightThrusters[i].Thrust;
        }
        float moveMult = MovementMultiplier + UpgradeMultiplier * _upgradeLevel * _upgradeLevel;

        _movement += wallForce * moveMult * Time.deltaTime;
        _movement -= MovementDampener * _movement * Time.deltaTime;

        if(Mathf.Abs(transform.position.z)<MaxFieldWidth)
        {
            transform.position += new Vector3(0, 0, _movement) * Time.deltaTime;
        }

        GlobalSoundManager.instance.SetThrusterVolume(true, leftVolume * 2);
        GlobalSoundManager.instance.SetThrusterVolume(false, rightVolume * 2);
        _camera.AddScreenShake((leftVolume + rightVolume) * 0.4f, 2, 0.3f);

        //Upgrade Thrusters
        _upgradeTimer += Time.deltaTime;
        if(_upgradeTimer > UpgradeTime && _upgradeLevel <= 2)
        {
            _upgradeLevel++;
            for (int i = 0; i < NumThrusters; i++)
            {
                LeftThrusters[i].UpgradeLevel = _upgradeLevel;
                Instantiate(UpgradePrefab, LeftThrusters[i].transform.position-new Vector3(0, 0, 1), Quaternion.identity);
                RightThrusters[i].UpgradeLevel = _upgradeLevel;
                Instantiate(UpgradePrefab, RightThrusters[i].transform.position + new Vector3(0, 0, 1), Quaternion.identity);
                if(_upgradeLevel == 1)
                {
                    GlobalSoundManager.instance.PlayClip(GlobalSounds.Thruster1, SourcePosition.Center, 1);
                }
                else if(_upgradeLevel == 2)
                {
                    GlobalSoundManager.instance.PlayClip(GlobalSounds.Thruster2, SourcePosition.Center, 1);
                }
            }
            _upgradeTimer = 0;
        }

        //end state
        if(transform.position.z > FieldWidth)
        {
            //left player wins
            LeftWon = true;
        }
        else if(transform.position.z < -FieldWidth)
        {
            //right player wins
            RightWon = true;
        }

        if(LeftWon)
        {
            foreach(var thruster in RightThrusters)
            {
                Instantiate(ExplosionPrefab, thruster.transform.position, Quaternion.identity);
            }
        }
        else if(RightWon)
        {
            foreach(var thruster in LeftThrusters)
            {
                Instantiate(ExplosionPrefab, thruster.transform.position, Quaternion.identity);
            }
        }
	}
}
