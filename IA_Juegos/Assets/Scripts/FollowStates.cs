using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ActionState
{
    AttackEnemy = 0,
    CaptureBase = 1,
    Defend = 2,
    GoHealing = 3,
    GoEnemyBase = 4,
    Heal = 5
}

public class FollowStates : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] public AgentNPC lookAt;
    [SerializeField] public Vector3 offset;
    [Header("Logic")]
    private Camera cam;
    public RawImage img;

    private Texture[] myTextures = new Texture[6];

    private void Start()
    {
        cam = Camera.main;
        myTextures[0] = Resources.Load("AttackEnemy") as Texture;
        myTextures[1] = Resources.Load("CaptureBase") as Texture;
        myTextures[2] = Resources.Load("Defend") as Texture;
        myTextures[3] = Resources.Load("GoHealing") as Texture;
        myTextures[4] = Resources.Load("GoToEnemyBase") as Texture;
        myTextures[5] = Resources.Load("Heal") as Texture;
    }

    private void Update()
    {
        transform.position = cam.WorldToScreenPoint(lookAt.Position + offset);
        img.texture = myTextures[(int) lookAt.ActionState];
    }
}
