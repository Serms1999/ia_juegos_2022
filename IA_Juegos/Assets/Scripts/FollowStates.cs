using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FollowStates : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] public AgentNPC lookAt;
    [SerializeField] public Vector3 offset;
    [Header("Logic")]
    private Camera cam;
    public RawImage img;

    private Texture[] myTextures = new Texture[6];
    // Start is called before the first frame update

    void Start()
    {
        cam = Camera.main;
        myTextures[0] = Resources.Load("AttackEnemy") as Texture;
        myTextures[1] = Resources.Load("CaptureBase") as Texture;
        myTextures[2] = Resources.Load("Defend") as Texture;
        myTextures[3] = Resources.Load("GoHealing") as Texture;
        myTextures[4] = Resources.Load("GoToEnemyBase") as Texture;
        myTextures[5] = Resources.Load("Heal") as Texture;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(lookAt.transform.position + offset);

        if (transform.position != pos)
            transform.position = pos;

        switch (lookAt.ActionState)
        {
            
            case ActionState.AttackEnemy:
                img.texture = myTextures[0];

                // code block
                break;
            case ActionState.CaptureBase:
                img.texture = myTextures[1];

                // code block
                break;
            case ActionState.Defend:
                img.texture = myTextures[2];

                // code block
                break;
            case ActionState.GoHealing:
                img.texture = myTextures[3];

                // code block
                break;
            case ActionState.GoEnemyBase:
                img.texture = myTextures[4];

                // code block
                break;
            case ActionState.Heal:
                img.texture = myTextures[4];

                // code block
                break;
            default:
                // code block
                break;
        }

    }

    void changeImage(int x)
    {
        img.texture = myTextures[x];
        

    }
}
