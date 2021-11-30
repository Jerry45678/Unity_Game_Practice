using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioAnimation : MonoBehaviour
{
    public Rigidbody2D rby;
    public AudioSource pound;
    public GameObject groundpound, spineffect;
    public MarioInput mario;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GroundPoundEffect()
    {
        RaycastHit2D groundA = Physics2D.Raycast(transform.position+new Vector3(-0.4f,-0.8f,0),Vector2.down,0.2f,LayerMask.GetMask("Ground"));
        RaycastHit2D groundB = Physics2D.Raycast(transform.position+new Vector3(0.4f,-0.8f,0),Vector2.down,0.2f,LayerMask.GetMask("Ground"));
        RaycastHit2D groundC = Physics2D.Raycast(transform.position+new Vector3(-0.4f,-0.8f,0),Vector2.down,0.2f,LayerMask.GetMask("Platform"));
        RaycastHit2D groundD = Physics2D.Raycast(transform.position+new Vector3(0.4f,-0.8f,0),Vector2.down,0.2f,LayerMask.GetMask("Platform"));
        if(groundB || groundA || groundD || groundC)
        {
            pound.Play();
            GameInfomation.MarioPound = true;
            Invoke("PoundBoxDisable", 0.15f);
            Instantiate(groundpound, new Vector3(transform.position.x, transform.position.y - 0.8f,transform.position.z), transform.rotation);
        }
    }
    void PoundBoxDisable()
    {
        GameInfomation.MarioPound = false;
        RaycastHit2D groundA = Physics2D.Raycast(transform.position+new Vector3(-0.4f,-0.8f,0),Vector2.down,0.2f,LayerMask.GetMask("Ground"));
        RaycastHit2D groundB = Physics2D.Raycast(transform.position+new Vector3(0.4f,-0.8f,0),Vector2.down,0.2f,LayerMask.GetMask("Ground"));
        if(!groundA && !groundB)
        {
            mario.OnGround = false;
        }
    }
    void TailBack()
    {
        if(!mario.OnGround)
        {
            if(mario.Face) Instantiate(mario.TailAttack,mario.transform.position+new Vector3(-0.75f,-0.45f,0),transform.rotation);
            else Instantiate(mario.TailAttack,mario.transform.position+new Vector3(0.75f,-0.45f,0),transform.rotation);
        }
    }
    void TailFore()
    {
        if(!mario.OnGround)
        {
            if(!mario.Face) Instantiate(mario.TailAttack,mario.transform.position+new Vector3(-0.75f,-0.45f,0),transform.rotation);
            else Instantiate(mario.TailAttack,mario.transform.position+new Vector3(0.75f,-0.45f,0),transform.rotation);
        }
    }
    void Jump()
    {
        rby.velocity = new Vector2(0,8);
    }
    void SpinEffect()
    {
        Instantiate(spineffect,transform.position,transform.rotation);
    }
    void Fly()
    {
        mario.CapUp = true;
    }
}
