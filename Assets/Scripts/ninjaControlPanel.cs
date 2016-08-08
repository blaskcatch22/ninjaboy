using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(ninjaControl))]
    public class ninjaControlPanel : MonoBehaviour
    {
        private ninjaControl m_Character;
        private Transform ninjatransform;
        private bool m_Jump;
        private bool m_throw;
        private bool m_glide;
        private bool attack;
        
        private Animator ninja_anim;

        private void Awake()
        {
            m_Character = GetComponent<ninjaControl>();
            ninja_anim = GetComponent<Animator>();
            ninjatransform = GetComponent<Transform>();
        }

        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = Input.GetButtonDown("Jump");
                
            }
            m_throw = Input.GetButtonDown("Throw");
            m_glide = Input.GetButton("Glide");
            attack = Input.GetButton("Attack");
        }

        private void FixedUpdate()
        {
                if (ninjatransform.position.y < -30)
                {
                reload();
                }
            
                // Read the inputs.

                float h = Input.GetAxis("Horizontal");

                // Pass all parameters to the character control script.

                if (!ninja_anim.GetCurrentAnimatorStateInfo(0).IsName("ninjaboy_attack") && !ninja_anim.GetBool("dead"))
                {
                    m_Character.Attack(attack);
                    m_Character.ninjaThrow(m_throw);
                    m_Character.Move(h, m_Jump, m_glide);
                }
                else {
                    m_Character.Move(0f, false, m_glide);
                }

                m_Jump = false;
        }
        public void reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
    }
}