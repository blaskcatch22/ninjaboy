using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace UnityStandardAssets._2D
{
    public class ninjaControl : MonoBehaviour
    {
        public Rigidbody2D Kunai;
        public float projectile_move_speed = 3f;
        public float base_playergravity = 3f;                    // Base player Rigidbody mass
        public float glide_playergravity = 1f;                    // Rigidbody mass while gliding
        public float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        public float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)]
        public float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        public bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        public LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        public float fallThresh;                  // Thresh hold of falling
		public Text countText;

		const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .75f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private bool m_glide;               // Whether the player is gliding;
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        private int m_facing_modifier = 1;
        private bool falling = false;
        private Vector3 rigidvector;
        private List<Rigidbody2D> flying_kunais;
        private Transform k_GroundCheck;
        private SpriteRenderer m_SpriteRender;
        private bool m_jump;
		private int score;
		private bool doubleJump;

        private void Awake()
        {

            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
			score = 0;
            //psystem = GetComponentInChildren<ParticleSystem>();

            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            flying_kunais = new List<Rigidbody2D>();
            m_SpriteRender = GetComponent<SpriteRenderer>();
            
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            
            if (collision.gameObject.tag == "Finish")
            {
                Debug.Log("finished");
                m_Anim.SetBool("dead", true);
                //GameObject.Destroy(m_Rigidbody2D);
                
            }
        }

		void OnTriggerEnter2D(Collider2D other)
		{

			if (other.gameObject.tag == "Finish")
			{
				Debug.Log("finished");
				m_Anim.SetBool("dead", true);
				//GameObject.Destroy(m_Rigidbody2D);

			}
			if (other.gameObject.tag == "Pickup")
			{
				Debug.Log("picked");
				other.gameObject.SetActive (false);
				countPickups (1);

				//GameObject.Destroy(m_Rigidbody2D);

			}
		}

        private void FixedUpdate()
        {
            m_Grounded = false;
            m_jump = true;
            
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
                    m_glide = false;
                    m_AirControl = false;
                    m_jump = false;
                    m_Anim.SetBool("glide", m_glide);
                    m_Anim.SetBool("jump", m_jump);
					//psystem.Stop();
					doubleJump = false;
                    //m_SpriteRender.enabled = true;
                    m_Rigidbody2D.gravityScale = base_playergravity;
            }
            m_Anim.SetBool("Ground", m_Grounded);
            m_Anim.SetBool("jump", m_jump);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

            for (int i = 0; i < flying_kunais.Count; i++)
            {
                k_GroundCheck = flying_kunais[flying_kunais.Count - 1].transform;
                Collider2D[] kunaiCollideGround = Physics2D.OverlapCircleAll(k_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
                for (int j = 0; j < kunaiCollideGround.Length; j++)
                {
                    if (kunaiCollideGround[j].gameObject != gameObject)
                    {
                        Destroy(flying_kunais[i].gameObject,0);
                        flying_kunais.RemoveAt(i);
                    }
                        
                }
            }
            


        }

        public void Attack(bool attack)
        {
            
            m_Anim.SetBool("Attack", attack);
            
        }

        public void ninjaThrow(bool m_throw)
        {
            if (m_throw) { 
                m_Anim.SetBool("Throw", m_throw);

                if (!m_FacingRight) {
                    m_facing_modifier = -1;
                }
                else
                {
                    m_facing_modifier = 1;
                }

                flying_kunais.Add((Rigidbody2D)Instantiate(Kunai,transform.position + (new Vector3(2f * m_facing_modifier,0.1f)), Quaternion.Euler(0, 0, 270 * m_facing_modifier)));
                
                flying_kunais[flying_kunais.Count-1].velocity = new Vector2(projectile_move_speed * m_facing_modifier, flying_kunais[flying_kunais.Count - 1].velocity.y);
                
            }
            m_Anim.SetBool("Throw", m_throw);
        }

        public void Move(float move, bool jump, bool glide)
        {
            falling = false;

            rigidvector = transform.InverseTransformDirection(m_Rigidbody2D.velocity);
            if (rigidvector.y < fallThresh)
            {
                falling = true;
            }

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                
                m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
                
                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...

            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                Debug.Log("jump");
                m_Anim.SetBool("jump",jump);
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));


             }
            
            // If the player can glide
            if (!m_Grounded && glide && !m_Anim.GetBool("Ground") && falling && !doubleJump) 

            {
                //Set glide param and downgrade gravity           
                m_Anim.SetBool("glide", true);
                m_AirControl = true;
                m_Rigidbody2D.gravityScale = glide_playergravity;
            }

            if (!m_Grounded && jump && !m_Anim.GetBool("Ground") && falling && !m_Anim.GetBool("glide") && !doubleJump)
            {
                //Set doublejump
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				//psystem.emission.rate.constantMax = 1000;
                m_AirControl = true;
                doubleJump = true;
                //m_SpriteRender.enabled = !m_SpriteRender.enabled;
            }
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
		private void countPickups(int pickupScore)
		{
			score = score + pickupScore;
			Debug.Log (score);
			countText.text = "Score:" + score.ToString ();
		}
    }
}
