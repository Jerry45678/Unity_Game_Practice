using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioInput : MonoBehaviour
{
    public Vector2 rbyVec;
    public bool Face = true, isOnSlope, Freeze, Carry, OnGround, SlopeDown, WallTouch, PipeTime, CapUp;
    public float Value, SlopeAngleValue;
    public string AnimName;
    private Vector2 slopeNormalPerp, Mpos;
    public SpriteRenderer sprite, SmallMarioS, SuperMarioS, FireMarioS, IceMarioS, RaccoS, KoopaS, SquirrelS, BoomerS;
    public Sprite Death2;
    public BoxCollider2D body, enemybox, starman1, shell1, shell2, shell3;
    private float SlopeAngle, PosX, TopPos, R = 1, G = 1, B = 1, rbyy;
    public Animator ai, SmallAi, SuperAi,　FireAi, IceAi, RaccoAi, KoopaAi, SquirrelAi, BoomerAi;
    public CameraV2 cameraV2;
    public PhysicsMaterial2D with,nofr;
    public Check enemy;
    public Rigidbody2D rby;
    public GameObject Mario, TailAttack, DeadNPC, DeadMario, stompeffect, smokelittle, bubble, carrypos, RaccoMario, SmallMario, FireMario, Fireball, Snowball, SlopeAtack, IceMario, KoopaMario, Squirrel;
    public GameObject Cap, Shine, Shadow, Boomerang, boomer, Star;
    public TrailRenderer Wind1, Wind2;
    public AudioSource pound, spinflyin, havecape, spinfly, fly, capfly, jump, stomp, spin, splash, swim, shrink, kick, pipe, deaths, fire, superstomp, item, grow, blockbreak, bump, hited;
    public AudioSource starmanend, starman, coin, starget;
    public float WalkSpeed, Seed, CapeSpeed;
    private bool canJump, inWater, canFly, inPipe, DownTime, GroundPound, canDown, ycantcme, bubbleTime = true, canwalljump, Climbing, ClimbF, VineJump, isDead, isHold, twoblock, canspin = true;
    private bool FlyTime, Rs, Gs, Bs = true, As, EQ, canFloat, WallKeepin, WaterCheck;
    public int InvincibleTime, canFlyTime, StarManTime;
    private int canJumpTime = 10, RunTime, PoundTime, wallJumpTime, wallholdtime, Runsmoke, canSwim, WaterTime, SlopeTime, ClimbTime, TurnColdDown, DeadRoll;
    private int FireCold, JumpHold, CapeUpTime, UpPower, DownPower, CapeSmoke, WallHold, HoldStarTime;
    // Start is called before the first frame update
    void Start()
    {
        rby = GetComponent<Rigidbody2D>();
        GameInfomation.camera2 = cameraV2;
        GameInfomation.kick = kick;
        GameInfomation.item = item;
        GameInfomation.grow = grow;
        GameInfomation.Cape = havecape;
        GameInfomation.Hit = hited;
        GameInfomation.RolldeadNPC = DeadNPC;
        GameInfomation.block = bump;
        GameInfomation.blockbreak = blockbreak;
        //PlayerPrefs.DeleteAll();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("down") && !OnGround && rby.velocity.y < 8 && !inWater && !Carry && !Climbing)
        {
            canDown = true;
        }

        MarioState();
        Ground();
        AnimatorClipInfo[] Name = ai.GetCurrentAnimatorClipInfo(0);
        if(name != null)
        {
            GameInfomation.AnimationName = Name[0].clip.name;
        }
        AnimName = GameInfomation.AnimationName;
        
        ai.SetBool("isGround", OnGround);

        if(GameInfomation.HP == 3 && !Freeze)
        {
            Fireballs();
        }
        else if(GameInfomation.HP == 4 && !Freeze)
        {
            SnowballThrow();
        }
        else if((GameInfomation.HP == 5 || GameInfomation.HP == 6) && !Freeze)
        {
            Tail();
        }
        else if(GameInfomation.HP == 9 && !Freeze)
        {
            BoomerangThrow();
        }
        if(GameInfomation.HP == 6) CapePower();
        

        //TakeDown
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || GameInfomation.HP == 1)
        {
            if(Face)
            {
                carrypos.transform.position = new Vector3(transform.position.x + 0.7f * ((ai.GetCurrentAnimatorStateInfo(0).IsName("Pipe")?0:1)),transform.position.y -0.4f,0);
            }
            else
            {
                carrypos.transform.position = new Vector3(transform.position.x - 0.7f * ((ai.GetCurrentAnimatorStateInfo(0).IsName("Pipe")?0:1)),transform.position.y -0.4f,0);
            }
        }
        else
        {
            if(Face)
            {
                carrypos.transform.position = new Vector3(transform.position.x + 0.7f * ((ai.GetCurrentAnimatorStateInfo(0).IsName("Pipe")?0:1)),transform.position.y -0.16f,0);
            }
            else
            {
                carrypos.transform.position = new Vector3(transform.position.x - 0.7f * ((ai.GetCurrentAnimatorStateInfo(0).IsName("Pipe")?0:1)),transform.position.y -0.16f,0);
            }
        }
        GameInfomation.MarioFace = Face;
        GameInfomation.Marios = sprite.sprite;
    }
    void FixedUpdate()
    {
        GameInfomation.MarioRbyVec = rby.velocity;
        GameInfomation.FireballSpeed = 5 * ((!Face)?-1:1) + WalkSpeed * 0.5f;
        GameInfomation.MarioPosition = transform.position;
        GameInfomation.WalkSpeed = WalkSpeed;
        GameInfomation.MarioisGround = OnGround;
        GameInfomation.SpinJump = ai.GetCurrentAnimatorStateInfo(0).IsName("Spin");
        rbyVec = new Vector2(Mathf.Round(rby.velocity.x * 10) *0.1f, Mathf.Round(rby.velocity.y *10) *0.1f);
        Seed = Random.Range(0,1.0f);
        if(OnGround && Mathf.Abs(rby.velocity.y) < 1 && !ai.GetCurrentAnimatorStateInfo(0).IsName("Slope") && !ai.GetCurrentAnimatorStateInfo(0).IsName("Down"))
        {
            rby.velocity = new Vector2(rby.velocity.x, 0);
        }
        if(!Freeze && GameInfomation.HP > 0 && !ai.GetCurrentAnimatorStateInfo(0).IsName("Peace"))
        {
            Faceto();
            if(wallJumpTime < 1 && !ai.GetCurrentAnimatorStateInfo(0).IsName("Slope") && !FlyTime && !ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
            {
                Walk();
            }
            if(!inWater)
            {
                if(!Carry)
                {
                    Climb();
                    Spin();
                }
                else
                {
                    CarryJump();
                }
                if((GameInfomation.HP == 5) && canFly && JumpHold < 1)
                {
                    Fly();
                }
                else if((GameInfomation.HP == 6) && canFly && !OnGround)
                {
                    CapeFly();
                }
                else
                {
                    Jump();
                }
                if(!ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
                {
                    Wall();
                }
                if(GameInfomation.HP == 8)
                {
                    Float();
                    if(OnGround)
                    {
                        Wind1.emitting = false;
                        Wind2.emitting = false;
                    }
                }
                
                Cliff();
                if(GameInfomation.HP == 5 || GameInfomation.HP == 6) FallFly();
            }
            if(GameInfomation.HP == 7 && RunTime > 80)
            {
                Shell();
            }
            else
            {
                Down();
            }
            Slope();
            StuckinWall();
            StarMan();
            CarryItem();
            Stomp();
            Topwall();
            Water();
            
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle"))
        {
            rby.gravityScale = 1;
        }
        else if(ai.GetCurrentAnimatorStateInfo(0).IsName("WallKeep"))
        {
            rby.gravityScale = 0;
        }
        else if(rby.gravityScale != 2 && !GroundPound && !inWater && !(ai.GetCurrentAnimatorStateInfo(0).IsName("ClimbL") || ai.GetCurrentAnimatorStateInfo(0).IsName("ClimbR")) && !inPipe)
        {
            rby.gravityScale = 2;
        }
        }
        else if(ai.GetCurrentAnimatorStateInfo(0).IsName("Peace"))
        {
            rby.gravityScale = 2;
        }
        else
        {
            rby.gravityScale = 0;
            rby.velocity = Vector2.zero;
        }

        if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapTurn") || ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle") || ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("CapUp"))
        {
            FlyTime = true;
        }
        else
        {
            FlyTime = false;
        }

        if(GameInfomation.HP != 7)
        {
            if(shell3.enabled == true) shell3.enabled = false;
        }
        if(GameInfomation.HP == 7 || GameInfomation.HP == 9)
        {
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Down") && !inWater) shell3.enabled = true;
            else shell3.enabled = false;
        }

        if(FireCold > 0)
        {
            FireCold -= 1;
        }
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Slope"))
        {
            SlopeAtack.SetActive(true);
        }
        else
        {
            SlopeAtack.SetActive(false);
        }
        if(StarManTime < 1 && !ai.GetCurrentAnimatorStateInfo(0).IsName("Shell") && HoldStarTime < 1) Hurt();
        Death();
        StarGet();
    }

    void Walk()
    {
        if(WalkSpeed > 4 && Input.GetKey("left") && !Input.GetKey("right"))
        {
            if(!ai.GetCurrentAnimatorStateInfo(0).IsName("Turn") && OnGround && TurnColdDown < 1 && (ai.GetCurrentAnimatorStateInfo(0).IsName("Walk") || ai.GetCurrentAnimatorStateInfo(0).IsName("Run")))
            {
                ai.Play("Turn");
                TurnColdDown = 40;
            }
        }
        if(WalkSpeed < -4 && Input.GetKey("right") && !Input.GetKey("left"))
        {
            if(!ai.GetCurrentAnimatorStateInfo(0).IsName("Turn") && OnGround && TurnColdDown < 1 && (ai.GetCurrentAnimatorStateInfo(0).IsName("Walk") || ai.GetCurrentAnimatorStateInfo(0).IsName("Run")))
            {
                ai.Play("Turn");
                TurnColdDown = 40;
            }
        }

        if(TurnColdDown == 30 || TurnColdDown == 25 || TurnColdDown == 35)
        {
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Turn"))
            {
                Instantiate(smokelittle, new Vector3(transform.position.x,transform.position.y-0.8f,transform.position.z), transform.rotation);
            }
        }
        if(TurnColdDown > 0)
        {
            TurnColdDown -= 1;
        }

        if(Input.GetKey("right") && !(DownTime&&OnGround))
        {
            if(WalkSpeed < 3 * (Input.GetKey(GameInfomation.Act)?2:1))
            {
                WalkSpeed += 0.2f;
            }
            Face = true;
        }
        else if(Input.GetKey("left") && !(DownTime&&OnGround))
        {
            if(WalkSpeed > -3 * (Input.GetKey(GameInfomation.Act)?2:1))
            {
                WalkSpeed -= 0.2f;
            }
            Face = false;
        }
        if(!Input.GetKey(GameInfomation.Act) && OnGround)
        {
            if(WalkSpeed > 3.3f)
            {
                WalkSpeed -= 0.2f;
            }
            if(WalkSpeed < -3.3f)
            {
                WalkSpeed += 0.2f;
            }
        }
        if(OnGround && !Input.GetKey("right") && !Input.GetKey("left") || DownTime && OnGround)
        {
            if(WalkSpeed > 0.4f)
            {
                WalkSpeed -= 0.4f;
            }
            else if(WalkSpeed < -0.4f)
            {
                WalkSpeed += 0.4f;
            }
            else
            {
                WalkSpeed = 0;
            }
        }

        if(WalkSpeed != 0)
        {
            RaycastHit2D groundA = Physics2D.Raycast(transform.position + new Vector3(-0.285f * ((Face)?1:-1),-0.83f,0),Vector3.down,0.8f,LayerMask.GetMask("Ground"));
            RaycastHit2D groundB = Physics2D.Raycast(transform.position + new Vector3(0.345f * ((Face)?1:-1),-0.83f,0),Vector3.down,0.8f,LayerMask.GetMask("Ground"));
            
            if(OnGround && groundA.point.y > groundB.point.y && SlopeAngleValue > 1)
            {
                SlopeDown = true;
            }
            else if(OnGround && groundA.point.y < groundB.point.y && SlopeAngleValue > 1 && ((Face && WalkSpeed > 0) || (!Face && WalkSpeed < 0)))
            {
                SlopeDown = false;
            }

            if(OnGround && SlopeAngleValue == 0 && !SlopeDown)
            {
                rby.velocity = new Vector2(WalkSpeed, 0);
            }
            else if(OnGround && SlopeAngleValue == 45 && SlopeAngleValue != 0)
            {
                rby.velocity = new Vector2(WalkSpeed / 1.414f, Mathf.Abs(WalkSpeed / 1.414f) * ((SlopeDown)?-1:1));
            }
            else if(OnGround && SlopeAngleValue == 14)
            {
                rby.velocity = new Vector2(WalkSpeed, Mathf.Abs(WalkSpeed * 0.3f) * ((SlopeDown)?-1:1));
            }
            else if(OnGround && SlopeAngleValue == 27)
            {
                rby.velocity = new Vector2(WalkSpeed, Mathf.Abs(WalkSpeed * 0.5f) * ((SlopeDown)?-1:1));
            }
            else
            {
                rby.velocity = new Vector2(WalkSpeed, rby.velocity.y);
            }
        }

        if(Mathf.Abs(WalkSpeed) > 5 && !DownTime)
        {
            if(RunTime < 101)
            {
                RunTime += 1;
            }
        }
        else
        {
            if(RunTime > 0)
            {
                RunTime -= 1;
            }
        }
        if(RunTime > 100)
        {
            canFly = true;
            if(GameInfomation.HP == 5)
            {
                if(OnGround && canFlyTime == 0) canFlyTime += 50;
                if(OnGround && canFlyTime < 300) canFlyTime += 2;
            }
            else if(GameInfomation.HP == 6)
            {
                if(OnGround && canFlyTime == 0) canFlyTime = 120;
            }
            ai.SetBool("Run", true);
        }
        else
        {
            if(OnGround) canFly = false;
            ai.SetBool("Run", false);
        }

        if(Input.GetKey("left") || Input.GetKey("right"))
        {
            ai.SetBool("Walk", true);
        }
        else
        {
            ai.SetBool("Walk", false);
        }

        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            Runsmoke += 1;
            if(Runsmoke > 15)
            {
                if(Face)
                {
                    Instantiate(smokelittle, new Vector3(transform.position.x-0.4f,transform.position.y-0.7f,transform.position.z), transform.rotation);
                }
                else
                {
                    Instantiate(smokelittle, new Vector3(transform.position.x+0.4f,transform.position.y-0.7f,transform.position.z), transform.rotation);
                }
                Runsmoke = 0;
            }
        }
    }
    void Faceto()
    {
        if(Face)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }
    }
    void Jump()
    {
        if(Input.GetKey(GameInfomation.Jump) && canJump && canJumpTime > 1 && !inWater && !ai.GetCurrentAnimatorStateInfo(0).IsName("Spin"))
        {
            rby.velocity = new Vector2(rby.velocity.x, 10 * ((GameInfomation.MarioPound)?1.8f:1));
            canJumpTime -= 1;
            if(!jump.isPlaying)
            {
               jump.Play();
            }
            if(!DownTime && !ai.GetCurrentAnimatorStateInfo(0).IsName("GroundPound"))
            {
                if(ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
                {
                    ai.Play("Shell");
                }
                else if(Carry)
                {
                    ai.Play("TakeJump");
                }
                else if(RunTime > 60)
                {
                    ai.Play("Jump2");
                } 
                else
                {
                    ai.Play("Jump");
                }
            }
        }
        if(!Input.GetKey(GameInfomation.Jump) && !OnGround)
        {
            canJump = false;
        }

        if(OnGround && !Input.GetKey(GameInfomation.Jump))
        {
            canJump = true;
            if(Mathf.Abs(WalkSpeed) > 5)
            {
                canJumpTime = 14;
            }
            else
            {
                canJumpTime = 12;
            }
        }

        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Jump") && rby.velocity.y < -2)
        {
            ai.Play("Fall");
        }
    }
    void CarryJump()
    {
        if(Input.GetKey(GameInfomation.Spin) && OnGround && !inWater)
        {
            if(!Input.GetKey("down"))
            {
                ai.Play("TakeJump");
            }
            if(!jump.isPlaying)
            {
                jump.Play();
            }
            rby.velocity = new Vector2(rby.velocity.x, 11);
        }
    }
    void Down()
    {
        if(Input.GetKey("down") && !(inWater && !OnGround))
        {
            DownTime = true;
            ai.SetBool("Down", true);
        }
        else
        {
            DownTime = false;
            ai.SetBool("Down", false);
        }

        RaycastHit2D groundc = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
        if(!Carry && !inWater && !Climbing)
        {
            if(!WallTouch && !OnGround && (groundc.point.y - transform.position.y) < -2 && canDown && !ai.GetCurrentAnimatorStateInfo(0).IsName("Down") && PoundTime == 0 && !ai.GetCurrentAnimatorStateInfo(0).IsName("Slope")&& !ai.GetCurrentAnimatorStateInfo(0).IsName("Spin"))
            {
                ai.Play("GroundPound");
                WalkSpeed = 0;
                if(ai.GetCurrentAnimatorStateInfo(0).IsName("GroundPound"))
                {
                    GroundPound = true;
                    canDown = false;
                }
            }
            if(GroundPound)
            {
                PoundTime += 1;
                if(PoundTime > 1 && PoundTime < 30)
                {
                    rby.gravityScale = 0;
                    if(PoundTime == 2)
                    {
                        pound.Play();
                    }
                    rby.velocity = new Vector2(0, 0);
                }
                else if(PoundTime > 30)
                {
                    rby.gravityScale = 4;
                    rby.velocity = new Vector2(rby.velocity.x * 0.05f, rby.velocity.y);
                }
            }
            if(OnGround)
            {
                PoundTime = 0;
                canDown = false;
                GroundPound = false;
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("GroundPoundUp"))
            {
                WalkSpeed = 0;
            }
        }

        if(ai.GetCurrentAnimatorStateInfo(0).IsName("GroundPoundUp") && !OnGround)
        {
            ai.Play("Jump");
        }
    }
    void Spin()
    {
        if(Input.GetKey(GameInfomation.Spin) && !ai.GetCurrentAnimatorStateInfo(0).IsName("GroundPound") && canspin)
        {
            if(GameInfomation.HP == 8 && Input.GetKey("up")) ai.Play("AltSpin");
            else ai.Play("Spin");
            if(!spin.isPlaying)
            {
                spin.Play();
            }
            rby.velocity = new Vector2(rby.velocity.x, 11 * ((GameInfomation.HP == 8 && Input.GetKey("up")?1.2f:1)));
            canspin = false;
        }
        if((ai.GetCurrentAnimatorStateInfo(0).IsName("Spin") || ai.GetCurrentAnimatorStateInfo(0).IsName("AltSpin")) && rby.velocity.y < 3 && OnGround)
        {
            ai.Play("Idle");
        }
        if(!Input.GetKey(GameInfomation.Spin) && OnGround)
        {
            canspin = true;
        }
        if(!OnGround)
        {
            canspin = false;
        }
    }
    void Slope()
    {
        RaycastHit2D groundLR = Physics2D.Raycast(transform.position + new Vector3(-0.66f,-0.87f,0),Vector3.right,1.32f,LayerMask.GetMask("Ground"));
        RaycastHit2D groundD = Physics2D.Raycast(transform.position + new Vector3(0.345f * ((Face)?1:-1),-0.87f,0),Vector3.down,0.9f,LayerMask.GetMask("Ground"));

        if(groundLR)
        {
            slopeNormalPerp = Vector2.Perpendicular(groundLR.normal).normalized;
            SlopeAngle = Mathf.Round(Vector2.Angle(groundLR.normal, Vector2.up));
        }
        else
        {
            SlopeAngle = 90;
        }
        SlopeAngleValue = Mathf.Round(Vector2.Angle(groundD.normal, Vector2.up));

        if(Mathf.Round(Vector2.Angle(groundD.normal, Vector2.up)) < 80 && Mathf.Round(Vector2.Angle(groundD.normal, Vector2.up)) > 10)
        {
            isOnSlope = true;
        }
        else
        {
            isOnSlope = false;
        }

        if((ai.GetCurrentAnimatorStateInfo(0).IsName("TakeIdle") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeJump") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("Idle") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump") || ai.GetCurrentAnimatorStateInfo(0).IsName("Fall") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2")) && isOnSlope)
        {
            rby.sharedMaterial = with;
        }
        else
        {
            rby.sharedMaterial = nofr;
        }

        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Down") && isOnSlope && Mathf.Abs(rby.velocity.x) > 1)
        {
            RunTime = 0;
            ai.Play("Slope");
        }
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Idle") && OnGround && Mathf.Abs(rby.velocity.x) > 0 && isOnSlope && Input.GetKey("down"))
        {
            RunTime = 0;
            ai.Play("Slope");
        }
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Slope") && Mathf.Abs(rby.velocity.x) < 1 && SlopeAngle == 0)
        {
            ai.Play("Idle");
        }
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Slope") && Mathf.Abs(rby.velocity.x) < 8)
        {
            WalkSpeed = rby.velocity.x;
        }
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Slope") && PosX == transform.position.x)
        {
            ai.Play("Idle");
        }
        PosX = transform.position.x;

        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Slope") && Mathf.Abs(WalkSpeed) > 0.2f)
        {
            SlopeTime++;
            if(SlopeTime % 20 == 0)
            {
                Instantiate(smokelittle, new Vector3(transform.position.x,transform.position.y-0.9f,transform.position.z), transform.rotation);
            }
        }
    }
    void Wall()
    {
        RaycastHit2D wallDo = Physics2D.Raycast(new Vector2(transform.position.x - 0.45f,transform.position.y), Vector2.right, 0.9f, LayerMask.GetMask("Ground"));
        RaycastHit2D ground = Physics2D.Raycast(transform.position,Vector2.down,1.5f,LayerMask.GetMask("Ground"));
        if(!Face)
        {
            RaycastHit2D blockF = Physics2D.Raycast(transform.position+new Vector3(-1,-0.37f,0),Vector2.right,1,LayerMask.GetMask("Ground"));
            RaycastHit2D blockD = Physics2D.Raycast(transform.position+new Vector3(-1,-1.37f,0),Vector2.right,1,LayerMask.GetMask("Ground"));
            RaycastHit2D blockU = Physics2D.Raycast(transform.position+new Vector3(-1,0.63f,0),Vector2.right,1,LayerMask.GetMask("Ground"));
            if((blockF && blockU) || (blockD && blockF))
            {
                twoblock = true;
            }
            else
            {
                twoblock = false;
            }
        }
        else
        {
            RaycastHit2D blockF = Physics2D.Raycast(transform.position+new Vector3(1,-0.37f,0),Vector2.left,1,LayerMask.GetMask("Ground"));
            RaycastHit2D blockD = Physics2D.Raycast(transform.position+new Vector3(1,-1.37f,0),Vector2.left,1,LayerMask.GetMask("Ground"));
            RaycastHit2D blockU = Physics2D.Raycast(transform.position+new Vector3(1,0.63f,0),Vector2.left,1,LayerMask.GetMask("Ground"));
            if((blockF && blockU) || (blockD && blockF))
            {
                twoblock = true;
            }
            else
            {
                twoblock = false;
            }
        }

        if(Face)
        {
            RaycastHit2D wallA = Physics2D.Raycast(transform.position+new Vector3(0,-0.87f,0),Vector2.right,0.4f,LayerMask.GetMask("Ground"));
            RaycastHit2D wallB = Physics2D.Raycast(transform.position+new Vector3(0,0.62f,0),Vector2.right,0.4f,LayerMask.GetMask("Ground"));
            RaycastHit2D wallC = Physics2D.Raycast(transform.position+new Vector3(0,0,0),Vector2.right,0.4f,LayerMask.GetMask("Ground"));
            if(GameInfomation.HP == 1 || ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
            {
                if(wallC || (wallA && Mathf.Round(Vector2.Angle(wallA.normal, Vector2.up)) > 88))
                {
                    WallTouch = true;
                }
                else
                {
                    WallTouch = false;
                }
            }
            else
            {
                if(wallB || wallC || (wallA && Mathf.Round(Vector2.Angle(wallA.normal, Vector2.up)) > 88))
                {
                    WallTouch = true;
                }
                else
                {
                    WallTouch = false;
                }
            }
        }
        else
        {
            RaycastHit2D wallA = Physics2D.Raycast(transform.position+new Vector3(0,-0.87f,0),Vector2.left,0.4f,LayerMask.GetMask("Ground"));
            RaycastHit2D wallB = Physics2D.Raycast(transform.position+new Vector3(0,0.62f,0),Vector2.left,0.4f,LayerMask.GetMask("Ground"));
            RaycastHit2D wallC = Physics2D.Raycast(transform.position+new Vector3(0,0,0),Vector2.left,0.4f,LayerMask.GetMask("Ground"));
            if(GameInfomation.HP == 1 || ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
            {
                if(wallC || (wallA && Mathf.Round(Vector2.Angle(wallA.normal, Vector2.up)) > 88))
                {
                    WallTouch = true;
                }
                else
                {
                    WallTouch = false;
                }
            }
            else
            {
                if(wallB || wallC  || (wallA && Mathf.Round(Vector2.Angle(wallA.normal, Vector2.up)) > 88))
                {
                    WallTouch = true;
                }
                else
                {
                    WallTouch = false;
                }
            }
        }
        if(WallTouch && wallDo && twoblock)
        {
            rby.velocity = new Vector2(0, rby.velocity.y);
            WalkSpeed = 0;
            RunTime = 0;
            if(!Carry && !Climbing)
            {
                if(WallHold > 0 && !OnGround && wallJumpTime < 1 && !ground && rby.velocity.y < -2 && GameInfomation.HP == 8)
                {
                    rby.velocity = Vector2.zero;
                    ai.Play("WallKeep");
                }
                else if(!OnGround && rby.velocity.y < -2 && wallJumpTime < 1 && !ground && WallHold < 1)
                {
                    if(Input.GetKey("right") || Input.GetKey("left"))
                    {
                        rby.velocity = new Vector2(0, -2);
                        ai.Play("WallSide");
                        wallholdtime += 1;
                        if(wallholdtime > 14)
                        {
                            if(Face)
                            {
                                Instantiate(smokelittle, new Vector3(transform.position.x+0.5f,transform.position.y-0.3f,transform.position.z), transform.rotation);
                            }
                            else
                            {
                                Instantiate(smokelittle, new Vector3(transform.position.x-0.5f,transform.position.y-0.3f,transform.position.z), transform.rotation);
                            }
                            wallholdtime = 0;
                        }
                    }
                    else
                    {
                        if(rby.velocity.y < -2 && !ai.GetCurrentAnimatorStateInfo(0).IsName("WallKeep"))
                        {
                            ai.Play("Fall");
                        }
                    }
                }
            }
        }
        else if(WallTouch && !wallDo)
        {
            rby.velocity = new Vector2(0, rby.velocity.y);
            WalkSpeed = 0;
        }
        else if(WallTouch && !twoblock)
        {
            rby.velocity = new Vector2(0, rby.velocity.y);
            WalkSpeed = 0;
        }
        else if((!WallTouch && ai.GetCurrentAnimatorStateInfo(0).IsName("WallSide")) || (ground && ai.GetCurrentAnimatorStateInfo(0).IsName("WallSide")))
        {
            ai.Play("Fall");
        }
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("WallSide") && Input.GetKey(GameInfomation.Jump) && wallJumpTime < 1 && canwalljump)
        {
            rby.velocity = new Vector2(rby.velocity.x, 12);
            wallJumpTime = 20;
            ai.Play("Jump");
            canwalljump = false;
        }
        
        if(wallJumpTime > 0)
        {
            if(wallJumpTime == 19)
            {
                RaycastHit2D wallRL = Physics2D.Raycast(new Vector2(transform.position.x - 0.45f,transform.position.y), Vector2.right, 0.9f, LayerMask.GetMask("Ground"));
                if(wallRL.point.x > transform.position.x)
                {
                    Face = false;
                }
                else
                {
                    Face = true;
                }
                stomp.Play();
                if(Face)
                {
                    rby.velocity = new Vector2(6, rby.velocity.y);
                    WalkSpeed = 6;
                    WallTouch = false;
                    Instantiate(stompeffect, new Vector2(transform.position.x - 0.4f,transform.position.y-0.5f), transform.rotation);
                }
                else
                {
                    rby.velocity = new Vector2(-6, rby.velocity.y);
                    WalkSpeed = -6;
                    WallTouch = false;
                    Instantiate(stompeffect, new Vector2(transform.position.x + 0.4f,transform.position.y-0.5f), transform.rotation);
                }
            }
            wallJumpTime -= 1;
        }
        if(!canwalljump && !Input.GetKey(GameInfomation.Jump))
        {
            canwalljump = true;
        }
        RaycastHit2D wallSp = Physics2D.Raycast(new Vector2(transform.position.x - 0.45f,transform.position.y+0.2f), Vector2.right, 0.9f, LayerMask.GetMask("Ground"));
        if((wallSp && Face && WalkSpeed < 0 && !OnGround) || (wallSp && !Face && WalkSpeed > 0 && !OnGround))
        {
            WalkSpeed = 0;
        }
    }
    void Water()
    {
        if(inWater && !ai.GetCurrentAnimatorStateInfo(0).IsName("Swim") && !ai.GetCurrentAnimatorStateInfo(0).IsName("TakeJump"))
        {
            rby.velocity = new Vector2(rby.velocity.x * 0.7f, rby.velocity.y);
        }

        RaycastHit2D waterA = Physics2D.Raycast(transform.position+new Vector3(-0.4f,0.4f,0),new Vector2(1,-1),0.8f,LayerMask.GetMask("Water"));
        RaycastHit2D waterB = Physics2D.Raycast(transform.position+new Vector3(-0.4f,0.4f,0),new Vector2(1,-1),0.8f,LayerMask.GetMask("Water"));
        if(waterA || waterB) inWater = true;
        else inWater = false;

        if(!WaterCheck && inWater)
        {
            if(!Carry)
            {
                ai.Play("SwimIdle");
            }
            else
            {
                ai.Play("TakeJump");
            }
            Invoke("WaterBubble", 0.2f);
            if(rby.velocity.y < -10)
            {
                splash.Play();
            }
            rby.velocity = new Vector2(rby.velocity.x, -3);
            PoundTime = 0;
            GroundPound = false;
        }
        else if(!inWater && WaterCheck)
        {
            if(Input.GetKey(GameInfomation.Jump) || Input.GetKey(GameInfomation.Spin))
            {
                rby.velocity = new Vector2(rby.velocity.x, 11);
                if(!Carry)
                {
                    ai.Play("Jump");
                }
                else
                {
                    ai.Play("TakeJump");
                }
            }
            else
            {
                rby.velocity = new Vector2(rby.velocity.x, -1);
            }
            bubbleTime = true;
            WaterTime = 0;
        }

        WaterCheck = inWater;
        
        if(inWater && canSwim < 1 && (Input.GetKey(GameInfomation.Jump) || Input.GetKey(GameInfomation.Spin)) && !ai.GetCurrentAnimatorStateInfo(0).IsName("Swim"))
        {
            if(Input.GetKey("up"))
            {
                rby.velocity = new Vector2(rby.velocity.x, 7);
            }
            else
            {
                rby.velocity = new Vector2(rby.velocity.x, 4);
            }
            
            swim.Play();
            if(!Carry)
            {
                ai.Play("Swim");
            }
            else
            {
                ai.Play("TakeJump");
            }
            canSwim = 12;
        }
        if(canSwim > 0)
        {
            canSwim -= 1;
        }
        if(inWater)
        {
            RunTime = 0;
            if(Input.GetKey("down"))
            {
                rby.gravityScale = 1;
            }
            else
            {
                rby.gravityScale = 0.5f;
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("SwimIdle") && OnGround)
            {
                ai.Play("Idle");
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
            {
                ai.Play("Swim");
            }
            else if(ai.GetCurrentAnimatorStateInfo(0).IsName("TakeJump") && OnGround)
            {
                ai.Play("TakeIdle");
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Idle") || ai.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                if(Input.GetKey(GameInfomation.Jump) || Input.GetKey(GameInfomation.Spin))
                {
                    rby.velocity = new Vector2(rby.velocity.x, 8);
                    if(!Carry)
                    {
                        ai.Play("Swim");
                    }
                }
            }
            WaterTime++;
            if(WaterTime > 60)
            {
                if(Face)
                {
                    Instantiate(bubble, new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z), transform.rotation);
                }
                else
                {
                    Instantiate(bubble, new Vector3(transform.position.x - 0.3f, transform.position.y, transform.position.z), transform.rotation);
                }
                WaterTime = 0;
            }
            if(WalkSpeed > 0 || (WalkSpeed == 0 && Face))
            {
                RaycastHit2D wallA = Physics2D.Raycast(transform.position+new Vector3(0,-0.87f,0),Vector2.right,0.4f,LayerMask.GetMask("Ground"));
                RaycastHit2D wallB = Physics2D.Raycast(transform.position+new Vector3(0,0.62f,0),Vector2.right,0.4f,LayerMask.GetMask("Ground"));
                RaycastHit2D wallC = Physics2D.Raycast(transform.position+new Vector3(0,0,0),Vector2.right,0.4f,LayerMask.GetMask("Ground"));
                if(GameInfomation.HP == 1 || ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown"))
                {
                    if(wallC || (wallA && SlopeAngle > 88))
                    {
                        WalkSpeed = 0;
                        rby.velocity = new Vector2(0,rby.velocity.y);
                    }
                }
                else
                {
                    if(wallB || wallC || (wallA && SlopeAngle > 88))
                    {
                        WalkSpeed = 0;
                        rby.velocity = new Vector2(0,rby.velocity.y);
                    }
                }
            }
            else if(WalkSpeed < 0 || (WalkSpeed == 0 && !Face))
            {
                RaycastHit2D wallA = Physics2D.Raycast(transform.position+new Vector3(0,-0.87f,0),Vector2.left,0.4f,LayerMask.GetMask("Ground"));
                RaycastHit2D wallB = Physics2D.Raycast(transform.position+new Vector3(0,0.62f,0),Vector2.left,0.4f,LayerMask.GetMask("Ground"));
                RaycastHit2D wallC = Physics2D.Raycast(transform.position+new Vector3(0,0,0),Vector2.left,0.4f,LayerMask.GetMask("Ground"));
                if(GameInfomation.HP == 1 || ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown"))
                {
                    if(wallC || (wallA && SlopeAngle > 88))
                    {
                        WalkSpeed = 0;
                        rby.velocity = new Vector2(0,rby.velocity.y);
                    }
                }
                else
                {
                    if(wallB || wallC || (wallA && SlopeAngle > 88))
                    {
                        WalkSpeed = 0;
                        rby.velocity = new Vector2(0,rby.velocity.y);
                    }
                }
            }
        }
    }
    void Hurt()
    {
        if(enemy.Touch && InvincibleTime < 1 && !GameInfomation.Stomp && rby.velocity.y > -5 && !ai.GetCurrentAnimatorStateInfo(0).IsName("Pipe") && !ai.GetCurrentAnimatorStateInfo(0).IsName("Slope"))
        {
            if(GameInfomation.HP > 2)
            {
                GameInfomation.HP = 2;
            }
            else
            {
                GameInfomation.HP -= 1;
            }
            if(GameInfomation.HP > 0)
            {
                InvincibleTime = 120;
            }
        }
        if(GameInfomation.MarioTurn)
        {
            InvincibleTime = 118;
            Freeze = true;
            GameInfomation.MarioTurn = false;
        }

        if(GameInfomation.HP > 0)
        {
            if(InvincibleTime > 0)
            {
                InvincibleTime -= 1;
            }
            if(InvincibleTime > 0 && InvincibleTime % 10 == 0)
            {
                ycantcme = !ycantcme;
                if(ycantcme)
                {
                    sprite.color = new Color(1,1,1,0);
                }
                else
                {
                    sprite.color = new Color(1,1,1,1);
                }
            }
            if(InvincibleTime < 1 && ycantcme)
            {
                sprite.color = new Color(1,1,1,1);
                ycantcme = false;
            }
            switch(InvincibleTime)
            {
                case 119:
                    Freeze = true;
                    shrink.Play();
                    break;
                case 99:
                    if(GameInfomation.isCarry)
                    {
                        ai.Play("TakeIdle");
                    }
                    break;
                case 80:
                    Freeze = false;
                    break;
                case 1:
                    enemy.Touch = false;
                    break;
            }
        }
    }
    void Cliff()
    {
        RaycastHit2D cliffl = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y-0.5f), new Vector2(-1,-1), 0.6f, LayerMask.GetMask("Ground"));
        RaycastHit2D cliffr = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y-0.5f), new Vector2(1,-1), 0.6f, LayerMask.GetMask("Ground"));
        if(Mpos == new Vector2(transform.position.x, transform.position.y) && !OnGround)
        {
            if(cliffr)
            {
                transform.position = new Vector3(transform.position.x-0.1f,transform.position.y,transform.position.z);
                WalkSpeed = 0;
                rby.velocity = new Vector2(0, rby.velocity.y);
            }
            if(cliffl)
            {
                transform.position = new Vector3(transform.position.x+0.1f,transform.position.y,transform.position.z);
                WalkSpeed = 0;
                rby.velocity = new Vector2(0, rby.velocity.y);
            }
        }
        Mpos = transform.position;
    }
    void Climb()
    {
        if(Climbing)
        {
            if(WalkSpeed != 0)
            {
                WalkSpeed = 0;
            }
            if(Input.GetKey("up"))
            {
                if(Input.GetKey("right"))
                {
                    rby.velocity = new Vector2(3,4);
                }
                else if(Input.GetKey("left"))
                {
                    rby.velocity = new Vector2(-3,4);
                }
                else
                {
                    rby.velocity = new Vector2(0,4);
                }
                ClimbTime++;
            }
            else if(Input.GetKey("down"))
            {
                if(Input.GetKey("right"))
                {
                    rby.velocity = new Vector2(3,-4);
                }
                else if(Input.GetKey("left"))
                {
                    rby.velocity = new Vector2(-3,-4);
                }
                else
                {
                    rby.velocity = new Vector2(0,-4);
                }
                ClimbTime++;
            }
            else if(Input.GetKey("right"))
            {
                if(Input.GetKey("up"))
                {
                    rby.velocity = new Vector2(3,4);
                }
                else if(Input.GetKey("down"))
                {
                    rby.velocity = new Vector2(3,-4);
                }
                else
                {
                    rby.velocity = new Vector2(3,0);
                }
                ClimbTime++;
            }
            else if(Input.GetKey("left"))
            {
                if(Input.GetKey("up"))
                {
                    rby.velocity = new Vector2(-3,4);
                }
                else if(Input.GetKey("down"))
                {
                    rby.velocity = new Vector2(-3,-4);
                }
                else
                {
                    rby.velocity = new Vector2(-3,0);
                }
                ClimbTime++;
            }
            else
            {
                rby.velocity = new Vector2(0,0);
                if(ClimbTime % 10 == 0)
                {
                    ClimbTime = ClimbTime - 1;
                }
            }

            if(ClimbTime % 10 == 0)
            {
                ClimbF = !ClimbF;
            }

            if(ClimbF)
            {
                ai.Play("ClimbR");
            }
            else
            {
                ai.Play("ClimbL");
            }


            if(Input.GetKey(GameInfomation.Jump))
            {
                if(VineJump)
                {
                    ai.Play("Jump");
                    rby.velocity = new Vector2(0,10);
                    jump.Play();
                    VineJump = false;
                }
                if(!VineJump)
                {
                    Climbing = false;
                }
            }
        }
        else
        {
            if(!Input.GetKey(GameInfomation.Jump))
            {
                VineJump = true;
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("ClimbL") || ai.GetCurrentAnimatorStateInfo(0).IsName("ClimbR"))
            {
                ai.Play("Fall");
            }
        }
    }
    void WaterBubble()
    {
        if(bubbleTime && inWater)
        {
            int bubblecount = Random.Range(3,6);
            while(bubblecount > 0)
            {
                float bubblepos = 0.8f;
                Instantiate(bubble, new Vector3(transform.position.x + Random.Range(-bubblepos, bubblepos),transform.position.y + Random.Range(-bubblepos*1.5f, 0), transform.position.z), transform.rotation);
                bubblecount -= 1;
            }
            bubbleTime = false;
        }
    }
    void CarryItem()
    {
        if(GameInfomation.isCarry && isHold != GameInfomation.isCarry)
        {
            ai.Play("TakeIdle");
            Carry = true;
        }
        else if(!GameInfomation.isCarry)
        {
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("TakeIdle") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeWalk") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeJump") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown"))
            {
                ai.Play("Kick");
            }
            Carry = false;
        }

        isHold = GameInfomation.isCarry;
    }
    void MarioState()
    {
        if(GameInfomation.HP == 1 || ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
        {
            body.offset = new Vector2(0.03f,-0.44f);
            body.size = new Vector2(0.63f,0.87f);
            enemybox.offset = new Vector2(0.012f, -0.42f);
            enemybox.size = new Vector2(0.82f, 0.9f);
        }
        else
        {
            body.offset = new Vector2(0.03f,-0.125f);
            body.size = new Vector2(0.63f,1.5f);
            enemybox.offset = new Vector2(0.012f, -0.12f);
            enemybox.size = new Vector2(0.82f, 1.5f);
        }

        if(InvincibleTime < 100)
        {
            if(GameInfomation.HP < 1)
            {
                ai = null;
                sprite = null;
            }
            if(GameInfomation.HP == 1)
            {
                if(ai != SmallAi)
                {
                    ai = SmallAi;
                }
                if(sprite != SmallMarioS)
                {
                    sprite = SmallMarioS;
                }
                if(!SmallMario.activeSelf)
                {
                    SmallMario.SetActive(true);
                }
                if(FireMario.activeSelf)
                {
                    FireMario.SetActive(false);
                }
                if(Mario.activeSelf)
                {
                    Mario.SetActive(false);
                }
                if(IceMario.activeSelf)
                {
                    IceMario.SetActive(false);
                }
                if(RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(false);
                }
                if(Cap.activeSelf)
                {
                    Cap.SetActive(false);
                }
                if(KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(false);
                }
                if(Squirrel.activeSelf)
                {
                    Squirrel.SetActive(false);
                }
                if(Boomerang.activeSelf)
                {
                    Boomerang.SetActive(false);
                }
            }
            else if(GameInfomation.HP == 2)
            {
                if(ai != SuperAi)
                {
                    ai = SuperAi;
                }
                if(sprite != SuperMarioS)
                {
                    sprite = SuperMarioS;
                }
                if(SmallMario.activeSelf)
                {
                    SmallMario.SetActive(false);
                }
                if(FireMario.activeSelf)
                {
                    FireMario.SetActive(false);
                }
                if(!Mario.activeSelf)
                {
                    Mario.SetActive(true);
                }
                if(IceMario.activeSelf)
                {
                    IceMario.SetActive(false);
                }
                if(RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(false);
                }
                if(Cap.activeSelf)
                {
                    Cap.SetActive(false);
                }
                if(KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(false);
                }
                if(Squirrel.activeSelf)
                {
                    Squirrel.SetActive(false);
                }
                if(Boomerang.activeSelf)
                {
                    Boomerang.SetActive(false);
                }
            }
            else if(GameInfomation.HP == 3)
            {
                if(ai != FireAi)
                {
                    ai = FireAi;
                }
                if(sprite != FireMarioS)
                {
                    sprite = FireMarioS;
                }
                if(SmallMario.activeSelf)
                {
                    SmallMario.SetActive(false);
                }
                if(Mario.activeSelf)
                {
                    Mario.SetActive(false);
                }
                if(!FireMario.activeSelf)
                {
                    FireMario.SetActive(true);
                }
                if(IceMario.activeSelf)
                {
                    IceMario.SetActive(false);
                }
                if(RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(false);
                }
                if(Cap.activeSelf)
                {
                    Cap.SetActive(false);
                }
                if(KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(false);
                }
                if(Squirrel.activeSelf)
                {
                    Squirrel.SetActive(false);
                }
                if(Boomerang.activeSelf)
                {
                    Boomerang.SetActive(false);
                }
            }
            else if(GameInfomation.HP == 4)
            {
                if(ai != IceAi)
                {
                    ai = IceAi;
                }
                if(sprite != IceMarioS)
                {
                    sprite = IceMarioS;
                }
                if(SmallMario.activeSelf)
                {
                    SmallMario.SetActive(false);
                }
                if(Mario.activeSelf)
                {
                    Mario.SetActive(false);
                }
                if(FireMario.activeSelf)
                {
                    FireMario.SetActive(false);
                }
                if(!IceMario.activeSelf)
                {
                    IceMario.SetActive(true);
                }
                if(RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(false);
                }
                if(Cap.activeSelf)
                {
                    Cap.SetActive(false);
                }
                if(KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(false);
                }
                if(Squirrel.activeSelf)
                {
                    Squirrel.SetActive(false);
                }
                if(Boomerang.activeSelf)
                {
                    Boomerang.SetActive(false);
                }
            }
            else if(GameInfomation.HP == 5)
            {
                if(ai != RaccoAi)
                {
                    ai = RaccoAi;
                }
                if(sprite != RaccoS)
                {
                    sprite = RaccoS;
                }
                if(SmallMario.activeSelf)
                {
                    SmallMario.SetActive(false);
                }
                if(Mario.activeSelf)
                {
                    Mario.SetActive(false);
                }
                if(FireMario.activeSelf)
                {
                    FireMario.SetActive(false);
                }
                if(IceMario.activeSelf)
                {
                    IceMario.SetActive(false);
                }
                if(!RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(true);
                }
                if(Cap.activeSelf)
                {
                    Cap.SetActive(false);
                }
                if(KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(false);
                }
                if(Squirrel.activeSelf)
                {
                    Squirrel.SetActive(false);
                }
                if(Boomerang.activeSelf)
                {
                    Boomerang.SetActive(false);
                }
            }
            else if(GameInfomation.HP == 6)
            {
                if(ai != SuperAi)
                {
                    ai = SuperAi;
                }
                if(sprite != SuperMarioS)
                {
                    sprite = SuperMarioS;
                }
                if(SmallMario.activeSelf)
                {
                    SmallMario.SetActive(false);
                }
                if(FireMario.activeSelf)
                {
                    FireMario.SetActive(false);
                }
                if(!Mario.activeSelf)
                {
                    Mario.SetActive(true);
                }
                if(IceMario.activeSelf)
                {
                    IceMario.SetActive(false);
                }
                if(RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(false);
                }
                if(!Cap.activeSelf)
                {
                    Cap.SetActive(true);
                }
                if(KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(false);
                }
                if(Squirrel.activeSelf)
                {
                    Squirrel.SetActive(false);
                }
                if(Boomerang.activeSelf)
                {
                    Boomerang.SetActive(false);
                }
            }
            else if(GameInfomation.HP == 7)
            {
                if(ai != KoopaAi)
                {
                    ai = KoopaAi;
                }
                if(sprite != KoopaS)
                {
                    sprite = KoopaS;
                }
                if(SmallMario.activeSelf)
                {
                    SmallMario.SetActive(false);
                }
                if(Mario.activeSelf)
                {
                    Mario.SetActive(false);
                }
                if(FireMario.activeSelf)
                {
                    FireMario.SetActive(false);
                }
                if(IceMario.activeSelf)
                {
                    IceMario.SetActive(false);
                }
                if(RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(false);
                }
                if(Cap.activeSelf)
                {
                    Cap.SetActive(false);
                }
                if(!KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(true);
                }
                if(Squirrel.activeSelf)
                {
                    Squirrel.SetActive(false);
                }
                if(Boomerang.activeSelf)
                {
                    Boomerang.SetActive(false);
                }
            }
            else if(GameInfomation.HP == 8)
            {
                if(ai != SquirrelAi)
                {
                    ai = SquirrelAi;
                }
                if(sprite != SquirrelS)
                {
                    sprite = SquirrelS;
                }
                if(SmallMario.activeSelf)
                {
                    SmallMario.SetActive(false);
                }
                if(Mario.activeSelf)
                {
                    Mario.SetActive(false);
                }
                if(FireMario.activeSelf)
                {
                    FireMario.SetActive(false);
                }
                if(IceMario.activeSelf)
                {
                    IceMario.SetActive(false);
                }
                if(RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(false);
                }
                if(Cap.activeSelf)
                {
                    Cap.SetActive(false);
                }
                if(KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(false);
                }
                if(!Squirrel.activeSelf)
                {
                    Squirrel.SetActive(true);
                }
                if(Boomerang.activeSelf)
                {
                    Boomerang.SetActive(false);
                }
            }
            else if(GameInfomation.HP == 9)
            {
                if(ai != BoomerAi)
                {
                    ai = BoomerAi;
                }
                if(sprite != BoomerS)
                {
                    sprite = BoomerS;
                }
                if(SmallMario.activeSelf)
                {
                    SmallMario.SetActive(false);
                }
                if(Mario.activeSelf)
                {
                    Mario.SetActive(false);
                }
                if(FireMario.activeSelf)
                {
                    FireMario.SetActive(false);
                }
                if(IceMario.activeSelf)
                {
                    IceMario.SetActive(false);
                }
                if(RaccoMario.activeSelf)
                {
                    RaccoMario.SetActive(false);
                }
                if(Cap.activeSelf)
                {
                    Cap.SetActive(false);
                }
                if(KoopaMario.activeSelf)
                {
                    KoopaMario.SetActive(false);
                }
                if(Squirrel.activeSelf)
                {
                    Squirrel.SetActive(false);
                }
                if(!Boomerang.activeSelf)
                {
                    Boomerang.SetActive(true);
                }
            }
        }
        
    }
    void Death()
    {
        if(GameInfomation.HP < 1 && transform.position.y > -12)
        {
            Instantiate(DeadMario,transform.position,transform.rotation);
            Destroy(this.gameObject);
        }

        if(transform.position.y < -12 && transform.position.y > -20 && !deaths.isPlaying)
        {
            cameraV2.FollowPlayer = false;
            transform.position += new Vector3(-100,0,0);
            cameraV2.Music.volume = 0;
            deaths.Play();
        }
        else if(transform.position.y < -20 && !deaths.isPlaying)
        {
            GameInfomation.HP = 2;
            GameInfomation.Life -= 1;
            isDead = true;
            SceneManager.LoadScene(0);
        }
    }
    void Topwall()
    {
        if(GameInfomation.HP < 2 || ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))TopPos = -0.02f;
        else TopPos = 0.6f;
        RaycastHit2D topA = Physics2D.Raycast(transform.position + new Vector3(-0.285f * ((Face)?1:-1),TopPos,0),Vector3.up,0.075f,LayerMask.GetMask("Ground"));
        RaycastHit2D topB = Physics2D.Raycast(transform.position + new Vector3(0.345f * ((Face)?1:-1),TopPos,0),Vector3.up,0.075f,LayerMask.GetMask("Ground"));
        RaycastHit2D topC = Physics2D.Raycast(transform.position + new Vector3(0,TopPos,0),Vector3.up,0.075f,LayerMask.GetMask("Ground"));
        if(topA || topB || topC)
        {
            if(rby.velocity.y > 0) rby.velocity = new Vector2(rby.velocity.x,-1);if(!bump.isPlaying) bump.Play();canJumpTime = 0;
        }
    }
    void Stomp()
    {
        if(GameInfomation.Stomp && rby.velocity.y < 0)
        {
            if(Input.GetKey(GameInfomation.Jump) && !ai.GetCurrentAnimatorStateInfo(0).IsName("Spin") && !inWater)
            {
                rby.velocity = new Vector2(rby.velocity.x, 10);
                if(!GameInfomation.isCarry)
                {
                    if(ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2")) ai.Play("Jump2");
                    else ai.Play("Jump");
                } 
                else ai.Play("TakeJump");
                stomp.Play();
            }
            else if(ai.GetCurrentAnimatorStateInfo(0).IsName("Spin") && !inWater)
            {
                rby.velocity = new Vector2(rby.velocity.x, 4);
                superstomp.Play();
            }
            else
            {
                rby.velocity = new Vector2(rby.velocity.x, 5);
                if(!GameInfomation.isCarry)
                {
                    if(ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2")) ai.Play("Jump2");
                    else ai.Play("Jump");
                } 
                else ai.Play("TakeJump");
                stomp.Play();
            }
            GameInfomation.Stomp = false;
        }
        if(GameInfomation.StompSpin)
        {
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Spin") && !inWater && rby.velocity.y < 0)
            {
                rby.velocity = new Vector2(rby.velocity.x, 4);
                Instantiate(stompeffect,new Vector3(transform.position.x,transform.position.y - Random.Range(0.75f,0.85f),0),transform.rotation);
                stomp.Play();
            }
            GameInfomation.StompSpin = false;
        }
        if(GameInfomation.StompBlockSpin)
        {
            if(!inWater && rby.velocity.y <= 0)
            {
                Invoke("PlaySpinAnim",0.01f);
                ai.Play("Spin");
                rby.velocity = new Vector2(rby.velocity.x, 5);
            }
            GameInfomation.StompBlockSpin = false;
        }
        if(GameInfomation.StompJump)
        {
            if(Input.GetKey(GameInfomation.Jump) && !ai.GetCurrentAnimatorStateInfo(0).IsName("Spin") && !inWater)
            {
                rby.velocity = new Vector2(rby.velocity.x, 10);
                if(!GameInfomation.isCarry) ai.Play("Jump");
                else ai.Play("TakeJump");
                stomp.Play();
            }
            else if(ai.GetCurrentAnimatorStateInfo(0).IsName("Spin") && !inWater)
            {
                rby.velocity = new Vector2(rby.velocity.x, 4);
                stomp.Play();
            }
            else
            {
                rby.velocity = new Vector2(rby.velocity.x, 5);
                if(!GameInfomation.isCarry) ai.Play("Jump");
                else ai.Play("TakeJump");
                stomp.Play();
            }
            GameInfomation.StompJump = false;
        }
    }
    void PlaySpinAnim()
    {
        ai.Play("Spin");
    }
    void Ground()
    {
        RaycastHit2D groundA = Physics2D.Raycast(transform.position + new Vector3(-0.285f * ((Face)?1:-1),-0.83f,0),Vector3.down,0.15f,LayerMask.GetMask("Ground"));
        RaycastHit2D groundB = Physics2D.Raycast(transform.position + new Vector3(0.345f * ((Face)?1:-1),-0.83f,0),Vector3.down,0.15f,LayerMask.GetMask("Ground"));
        RaycastHit2D groundC = Physics2D.Raycast(transform.position + new Vector3(-0.285f * ((Face)?1:-1),-0.83f,0),Vector3.down,0.15f,LayerMask.GetMask("Platform"));
        RaycastHit2D groundD = Physics2D.Raycast(transform.position + new Vector3(0.345f * ((Face)?1:-1),-0.83f,0),Vector3.down,0.15f,LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position + new Vector3(-0.285f * ((Face)?1:-1),-0.83f,0),Vector3.down*0.15f,Color.red);
        Debug.DrawRay(transform.position + new Vector3(0.345f * ((Face)?1:-1),-0.83f,0),Vector3.down*0.15f,Color.red);
            
        if(groundA || groundB || groundC || groundD)
        {
            OnGround = true;
        }
        else
        {
            OnGround = false;
        }
    }
    void Fireballs()
    {
        if(Input.GetKeyDown(GameInfomation.Act) && FireCold < 1 && !Carry && !Input.GetKey("down") && !Climbing && PoundTime == 0 && !ai.GetCurrentAnimatorStateInfo(0).IsName("WallSide"))
        {
            if(Face)
            {
                Instantiate(Fireball,transform.position + new Vector3(0.4f,0,0),transform.rotation);
            }
            else
            {
                Instantiate(Fireball,transform.position + new Vector3(-0.4f,0,0),transform.rotation);
            }
            if(OnGround)
            {
                ai.Play("Fire");
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Jump") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2") || ai.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
            {
                ai.Play("FireJump");
            }
            fire.Play();
            FireCold = 36;
        }
    }
    void SnowballThrow()
    {
        if(Input.GetKeyDown(GameInfomation.Act) && FireCold < 1 && !Carry && !Input.GetKey("down") && !Climbing && PoundTime == 0 && !ai.GetCurrentAnimatorStateInfo(0).IsName("WallSide"))
        {
            if(Face)
            {
                Instantiate(Snowball,transform.position + new Vector3(0.4f,0,0),transform.rotation);
            }
            else
            {
                Instantiate(Snowball,transform.position + new Vector3(-0.4f,0,0),transform.rotation);
            }
            if(OnGround)
            {
                ai.Play("Fire");
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Jump") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2") || ai.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
            {
                ai.Play("FireJump");
            }
            fire.Play();
            FireCold = 36;
        }
    }
    void Tail()
    {
        if(Input.GetKey(GameInfomation.Jump) && JumpHold > 0) JumpHold--;
        else if(!Input.GetKey(GameInfomation.Jump) && OnGround && rby.velocity.y >= 0) JumpHold = 10;

        if(Input.GetKeyDown(GameInfomation.Act) && (ai.GetCurrentAnimatorStateInfo(0).IsName("Walk") || ai.GetCurrentAnimatorStateInfo(0).IsName("Fly") || ai.GetCurrentAnimatorStateInfo(0).IsName("FallFly") || ai.GetCurrentAnimatorStateInfo(0).IsName("SwimIdle") || ai.GetCurrentAnimatorStateInfo(0).IsName("Swim") || ai.GetCurrentAnimatorStateInfo(0).IsName("Idle") || ai.GetCurrentAnimatorStateInfo(0).IsName("Run") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump") || ai.GetCurrentAnimatorStateInfo(0).IsName("Fall")))
        {
            ai.Play("Tail");
            if(Face) Instantiate(TailAttack,transform.position+new Vector3(0.75f,-0.45f,0),transform.rotation);
            else  Instantiate(TailAttack,transform.position+new Vector3(-0.75f,-0.45f,0),transform.rotation);
            spin.Play();
        }
    }
    void FallFly()
    {
        if(Input.GetKey(GameInfomation.Jump) && (ai.GetCurrentAnimatorStateInfo(0).IsName("Fall")))
        {
            ai.Play("FallFly");
        }
        else if(Input.GetKey(GameInfomation.Jump) && (ai.GetCurrentAnimatorStateInfo(0).IsName("FallFly")))
        {
            rby.velocity = new Vector2(rby.velocity.x,-4);
        }
        else if(Input.GetKey(GameInfomation.Jump) && rby.velocity.y < -3 && (ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeJump")))
        {
            rby.velocity = new Vector2(rby.velocity.x,-4);
        }
        else if(!Input.GetKey(GameInfomation.Jump) && (ai.GetCurrentAnimatorStateInfo(0).IsName("FallFly")) && !OnGround)
        {
            ai.Play("Fall");
        }
    }
    void Fly()
    {
        if(Input.GetKey(GameInfomation.Jump) && canFlyTime > 0 && !ai.GetCurrentAnimatorStateInfo(0).IsName("GroundPound") && !ai.GetCurrentAnimatorStateInfo(0).IsName("GroundPoundUp"))
        {
            if(!ai.GetCurrentAnimatorStateInfo(0).IsName("Tail") && !GameInfomation.isCarry && !(isOnSlope && !SlopeDown && SlopeAngle == 45)) ai.Play("Fly");
            if(canFlyTime % 15 == 0 && canFlyTime > 0 && canFlyTime < 300) fly.Play();
            canFlyTime--;
            rby.velocity = new Vector2(rby.velocity.x,5);
        }
        else if(canFlyTime < 1 && !OnGround && (ai.GetCurrentAnimatorStateInfo(0).IsName("Fly")))
        {
            ai.Play("Fall");
        }
    }
    void CapeFly()
    {
        if(Input.GetKey(GameInfomation.Act))
        {
            if(Input.GetKey(GameInfomation.Jump) && canFlyTime > 0 && !ai.GetCurrentAnimatorStateInfo(0).IsName("GroundPound") && (ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeJump")))
            {
                canFlyTime--;
                rby.velocity = new Vector2(rby.velocity.x,8);
            }
            else if((canFlyTime < 1 && !OnGround && (ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2"))) || (!Input.GetKey(GameInfomation.Jump) && !OnGround && ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2")))
            {
                if(!ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle"))
                {
                    rby.velocity = new Vector2(rby.velocity.x,0);
                    CapeSpeed = WalkSpeed;
                    ai.Play("CapIdle");
                } 
            }

            if(CapUp)
            {
                if(!capfly.isPlaying && !spinfly.isPlaying) capfly.Play();
                UpPower = 10;
                if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapTurn"))CapeUpTime = 8;
                else if(DownPower < 1)CapeUpTime = 18;
                else CapeUpTime = 28;
                CapUp = false;
            }

            if(CapeUpTime > 0)
            {
                rby.velocity = new Vector2(rby.velocity.x, UpPower);
                if(UpPower > 5 && CapeUpTime < 15)UpPower--;
                CapeUpTime--;
            }

            if(FlyTime) rby.velocity = new Vector2(CapeSpeed,rby.velocity.y);
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown"))
            {
                DownPower = 20;
                if(Face && CapeSpeed < 8) CapeSpeed += 0.2f;
                else if(!Face && CapeSpeed > -8) CapeSpeed -= 0.2f;
            }
            else
            {
                if(DownPower > 0) DownPower--;
            }
        }
        else
        {
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle") || ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("CapUp"))
            {
                FlyTime = false;
                WalkSpeed = CapeSpeed;
                ai.Play("Fall");
            }
        }
    }
    void CapePower()
    {
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle"))
        {
            if(Face)
            {
                if(Input.GetKeyDown("left")) ai.Play("CapUp");
                else if(Input.GetKey("right")) ai.Play("CapDown");
                
                if(Input.GetKeyDown(GameInfomation.Spin) && ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle"))
                {
                    Face = false;
                    CapUp = true;
                    CapeSpeed *= 0.5f;
                    spinfly.Play();
                    ai.Play("CapTurn");
                }
            }
            else
            {
                if(Input.GetKeyDown("right")) ai.Play("CapUp");
                else if(Input.GetKey("left")) ai.Play("CapDown");
                
                if(Input.GetKeyDown(GameInfomation.Spin) && ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle"))
                {
                    Face = true;
                    CapUp = true;
                    CapeSpeed *= 0.5f;
                    spinfly.Play();
                    ai.Play("CapTurn");
                }
            }
        }

        if(Face)
        {
            if(!Input.GetKey("right") && ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown") && Input.GetKey("left")) ai.Play("CapUp");
            else if(!Input.GetKey("right") && ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown")) ai.Play("CapIdle");
        }
        else
        {
            if(!Input.GetKey("left") && ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown") && Input.GetKey("right")) ai.Play("CapUp");
            else if(!Input.GetKey("left") && ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown")) ai.Play("CapIdle");
        }

        if(OnGround)
        {
            FlyTime = false;
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapUp")) ai.Play("Idle");
            else if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown") && EQ)
            {
                GameInfomation.Earthquack = true;
                WalkSpeed = CapeSpeed;
                ai.Play("Idle");
                EQ = false;
            }
            else if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown"))
            {
                WalkSpeed = CapeSpeed;
                ai.Play("Idle");
            }
            else if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle"))
            {
                WalkSpeed = CapeSpeed;
                CapeSpeed = Mathf.MoveTowards(CapeSpeed,0,0.04f);
                CapeSmoke++;
                if(CapeSmoke % 20 == 0) Instantiate(smokelittle, new Vector3(transform.position.x,transform.position.y-0.9f,transform.position.z), transform.rotation);
                if(CapeSpeed == 0)
                {
                    CapeSmoke = 1;
                    ai.Play("Idle");
                }
            }
        }

        if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown") && rby.velocity.y < -20) EQ = true;
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("CapUp") || ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle")) EQ = false;

        if((ai.GetCurrentAnimatorStateInfo(0).IsName("CapIdle") || ai.GetCurrentAnimatorStateInfo(0).IsName("CapDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("CapUp")) && WallTouch)
        {
            CapeSpeed = 0;
            ai.Play("Fall");
        }
    }
    void StuckinWall()
    {
        if(GameInfomation.HP > 1 && !Input.GetKey("down"))
        {
            RaycastHit2D stuckB = Physics2D.Raycast(transform.position + new Vector3(0.345f * ((Face)?1:-1),-0.2f,0),Vector2.up,0.7f,LayerMask.GetMask("Ground"));
            Debug.DrawRay(transform.position,Vector2.up*0.6f,Color.green);
            if(stuckB)
            {
                if(Face) transform.position -= new Vector3(0.01f,0,0);
                else transform.position += new Vector3(0.01f,0,0);
            }
        }
    }
    void StarMan()
    {
        if(StarManTime > 0)
        {
            sprite.color = new Color(R,G,B,1);
            if(StarManTime > 1)
            {
                if(R == 1 && G == 1 && B == 1)
                {
                    cameraV2.Music.Stop();
                    starman.Play();
                    starman1.enabled = true;
                    G = 0;
                    B = 0;
                }
                if(StarManTime % 8 == 0)
                {
                    Instantiate(Shine,transform.position+new Vector3(-0.5f+Random.Range(0,1f),-1+Random.Range(0,2f),0),transform.rotation);
                    Instantiate(Shadow,transform.position,transform.rotation);
                } 
                if(StarManTime == 40) starmanend.Play();

                if(Bs && !As)
                {
                    B = Mathf.MoveTowards(B,1,0.1f);
                    if(B == 1)
                    {
                        Rs = true;
                        As = true;
                        Bs = false;
                    }
                }
                else if(Rs && As)
                {
                    R = Mathf.MoveTowards(R,0,0.1f);
                    if(R == 0)
                    {
                        Rs = false;
                        As = false;
                        Gs = true;
                    }
                }
                else if(Gs && !As)
                {
                    G = Mathf.MoveTowards(G,1,0.1f);
                    if(G == 1)
                    {
                        Gs = false;
                        As = true;
                        Bs = true;
                    }
                }
                else if(Bs && As)
                {
                    B = Mathf.MoveTowards(B,0,0.1f);
                    if(B == 0)
                    {
                        Rs = true;
                        As = false;
                        Bs = false;
                    }
                }
                else if(Rs && !As)
                {
                    R = Mathf.MoveTowards(R,1,0.1f);
                    if(R == 1)
                    {
                        Rs = false;
                        As = true;
                        Gs = true;
                    }
                }
                else if(Gs && As)
                {
                    G = Mathf.MoveTowards(G,0,0.1f);
                    if(G == 0)
                    {
                        Bs = true;
                        As = false;
                        Gs = false;
                    }
                }
            }
            else if(StarManTime == 1)
            {
                Rs = false;
                Bs = true;
                Gs = false;
                As = false;
                starman1.enabled = false;
                cameraV2.Music.Play();
                starman.Stop();
                R = 1;
                G = 1;
                B = 1;
                sprite.color = new Color(R,G,B,1);
            }
            StarManTime--;
        }
    }
    void Shell()
    {
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Run") && Input.GetKey(GameInfomation.Act) && Input.GetKey("down"))
        {
            ai.Play("IntoShell");
        }

        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
        {
            if(Face) rby.velocity=new Vector2(8,rby.velocity.y);
            else rby.velocity=new Vector2(-8,rby.velocity.y);
            shell1.enabled = true;
            shell2.enabled = true;
            RaycastHit2D wallA = Physics2D.Raycast(transform.position+new Vector3(-0.5f,-0.1f,0),Vector2.right,1,LayerMask.GetMask("Ground"));
            RaycastHit2D wallB = Physics2D.Raycast(transform.position+new Vector3(-0.5f,-0.8f,0),Vector2.right,1,LayerMask.GetMask("Ground"));
            if(wallA)
            {
                if(wallA.point.x < transform.position.x) Face = true;
                else Face = false;
                if(!bump.isPlaying) bump.Play();
            }
            else if(wallB && Vector2.Angle(wallB.normal,Vector2.up) > 89 && Vector2.Angle(wallB.normal,Vector2.up) < 91)
            {
                if(wallB.point.x < transform.position.x) Face = true;
                else Face = false;
                if(!bump.isPlaying) bump.Play();
            }
           
            if(!Input.GetKey(GameInfomation.Act))
            {
                WalkSpeed = rby.velocity.x;
                if(enemy.Touch) enemy.Touch = false;
                ai.Play("Idle");
            } 
        }
        else
        {
            shell1.enabled = false;
            shell2.enabled = false;
        }
    }
    void Float()
    {
        if(OnGround)
        {
            canFloat = true;
            WallHold = 50;
        }
        else
        {
            if(Input.GetKey(GameInfomation.Jump) && (ai.GetCurrentAnimatorStateInfo(0).IsName("Fall") || (ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2") && rby.velocity.y < -2)) && !ai.GetCurrentAnimatorStateInfo(0).IsName("Fly"))
            {
                ai.Play("FallFly");
            }
            else if(Input.GetKey(GameInfomation.Jump) && (ai.GetCurrentAnimatorStateInfo(0).IsName("FallFly")))
            {
                rby.velocity = new Vector2(rby.velocity.x,-4);
            }
            else if(Input.GetKey(GameInfomation.Jump) && rby.velocity.y < -3 && (ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeJump")))
            {
                rby.velocity = new Vector2(rby.velocity.x,-4);
            }

            if(canFloat && Input.GetKey(GameInfomation.Spin) && (ai.GetCurrentAnimatorStateInfo(0).IsName("Fall") || ai.GetCurrentAnimatorStateInfo(0).IsName("FallFly") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump")))
            {
                ai.Play("Fly");
                if(WalkSpeed < 3 && Face)
                {
                    WalkSpeed = 4;
                }
                else if(WalkSpeed > -3 && !Face)
                {
                    WalkSpeed = -4;
                }
                spinflyin.Play();
                rby.velocity = new Vector2(rby.velocity.x,12);
                canFloat = false;
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Fly") || ai.GetCurrentAnimatorStateInfo(0).IsName("FallFly"))
            {
                Wind1.emitting = true;
                Wind2.emitting = true;
            }

            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Fly") && rby.velocity.y < -4) ai.Play("FallFly");
            if(Face)
            {
                RaycastHit2D wallA = Physics2D.Raycast(transform.position+new Vector3(0,-0.87f,0),Vector2.right,0.5f,LayerMask.GetMask("Ground"));
                RaycastHit2D wallB = Physics2D.Raycast(transform.position+new Vector3(0,0.62f,0),Vector2.right,0.5f,LayerMask.GetMask("Ground"));
                RaycastHit2D wallC = Physics2D.Raycast(transform.position+new Vector3(0,0,0),Vector2.right,0.5f,LayerMask.GetMask("Ground"));
                if(GameInfomation.HP == 1 || ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
                {
                    if(wallC || (wallA && Mathf.Round(Vector2.Angle(wallA.normal, Vector2.up)) > 88))
                    {
                        WallKeepin = true;
                    }
                    else
                    {
                        WallKeepin = false;
                    }
                }
                else
                {
                    if(wallB || wallC || (wallA && Mathf.Round(Vector2.Angle(wallA.normal, Vector2.up)) > 88))
                    {
                        WallKeepin = true;
                    }
                    else
                    {
                        WallKeepin = false;
                    }
                }
            }
            else
            {
                RaycastHit2D wallA = Physics2D.Raycast(transform.position+new Vector3(0,-0.87f,0),Vector2.left,0.5f,LayerMask.GetMask("Ground"));
                RaycastHit2D wallB = Physics2D.Raycast(transform.position+new Vector3(0,0.62f,0),Vector2.left,0.5f,LayerMask.GetMask("Ground"));
                RaycastHit2D wallC = Physics2D.Raycast(transform.position+new Vector3(0,0,0),Vector2.left,0.5f,LayerMask.GetMask("Ground"));
                if(GameInfomation.HP == 1 || ai.GetCurrentAnimatorStateInfo(0).IsName("Down") || ai.GetCurrentAnimatorStateInfo(0).IsName("TakeDown") || ai.GetCurrentAnimatorStateInfo(0).IsName("Shell"))
                {
                    if(wallC || (wallA && Mathf.Round(Vector2.Angle(wallA.normal, Vector2.up)) > 88))
                    {
                        WallKeepin = true;
                    }
                    else
                    {
                        WallKeepin = false;
                    }
                }
                else
                {
                    if(wallB || wallC  || (wallA && Mathf.Round(Vector2.Angle(wallA.normal, Vector2.up)) > 88))
                    {
                        WallKeepin = true;
                    }
                    else
                    {
                        WallKeepin = false;
                    }
                }
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("WallKeep"))
            {
                WallHold--;
                if(Input.GetKey(GameInfomation.Jump)) WallHold = 0;
                if((Face && !Input.GetKey("right")) || (!Face && !Input.GetKey("left")) || !WallKeepin)
                {
                    WallHold = 0;
                    ai.Play("Fall");
                }
                if(WallHold < 1)
                {
                    rby.velocity = new Vector2(0,-2.5f);
                    ai.Play("WallSide");
                }
            }
        }
    }
    void BoomerangThrow()
    {
        if(Input.GetKeyDown(GameInfomation.Act) && GameInfomation.BoomerangNumber > 0 && FireCold < 1 && !Carry && !Input.GetKey("down") && !Climbing && PoundTime == 0 && !ai.GetCurrentAnimatorStateInfo(0).IsName("WallSide"))
        {
            if(Face)
            {
                Instantiate(boomer,transform.position + new Vector3(0.4f,0,0),transform.rotation);
            }
            else
            {
                Instantiate(boomer,transform.position + new Vector3(-0.4f,0,0),transform.rotation);
            }
            if(OnGround)
            {
                ai.Play("Fire");
            }
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("Jump") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2") || ai.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
            {
                ai.Play("FireJump");
            }
            fire.Play();
            FireCold = 36;
        }
    }
    void StarGet()
    {
        if(GameInfomation.StarGeted && OnGround)
        {
            ai.Play("Peace");
            starget.Play();
            GameInfomation.StarGeted = false;
        }
        if(ai.GetCurrentAnimatorStateInfo(0).IsName("Peace"))
        {
            HoldStarTime++;
            if(HoldStarTime == 75)
            {
                if(GameInfomation.HP == 1)
                {
                    Star.transform.position = transform.position + new Vector3(0.5f * ((Face)?1:-1),1.3f,0);
                }
                else
                {
                    Star.transform.position = transform.position + new Vector3(0.6f * ((Face)?1:-1),1.7f,0);
                }
                GameInfomation.UGAS.SetActive(true);
                Star.SetActive(true);
            }
            if(HoldStarTime == 5)
            {
                cameraV2.Music.Pause();
            }
            else if(HoldStarTime > 150)
            {
                if(Input.GetKey(GameInfomation.Jump) && !starget.isPlaying)
                {
                    HoldStarTime = 0;
                    GameInfomation.UGAS.SetActive(false);
                    canJump = false;
                    ai.Play("Idle");
                    cameraV2.Music.UnPause();
                    Star.SetActive(false);
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (VineJump && col.gameObject.layer == 12 && (ai.GetCurrentAnimatorStateInfo(0).IsName("Jump2") || ai.GetCurrentAnimatorStateInfo(0).IsName("Spin") || ai.GetCurrentAnimatorStateInfo(0).IsName("Idle") || ai.GetCurrentAnimatorStateInfo(0).IsName("Fall") || ai.GetCurrentAnimatorStateInfo(0).IsName("Jump")))
        {
            if(Input.GetKey("up") || Input.GetKey("down"))
            {
                if(!Climbing)
                {
                    if(ClimbF)
                    {
                        ai.Play("ClimbR");
                    }
                    else
                    {
                        ai.Play("ClimbL");
                    }
                    PoundTime = 0;
                    canDown = false;
                    GroundPound = false;
                    rby.gravityScale = 0;
                    WalkSpeed = 0;
                    RunTime = 0;
                    rby.velocity = new Vector2(0, 4); 
                }
                Climbing = true;
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 12)
        {
            if(ai.GetCurrentAnimatorStateInfo(0).IsName("ClimbL") || ai.GetCurrentAnimatorStateInfo(0).IsName("ClimbR"))
            {
                ai.Play("Fall");
                Climbing = false;
            }
        }
    }
}
