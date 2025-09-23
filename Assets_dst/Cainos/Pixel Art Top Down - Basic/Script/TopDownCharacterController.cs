using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        // Interact settings
        public float interactRange = 1.5f;
        private ChestController nearbyChest;
        private SignController nearbySign;
        private DoorController nearbyDoor;

        public float speed;
        private Animator animator;
        
        // Pause menu reference
        public GameObject pauseMenuUI;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Check if chest is opening or waiting for key confirmation - disable movement
            if (ChestAudio.IsChestOpening || ChestAudio.IsWaitingForKeyConfirmation())
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                animator.SetBool("IsMoving", false);
                return;
            }

            // Check if dialogue is showing - disable movement
            if (DialogueManager.IsDialogueShowing() || DialogueManager.IsWaitingForKeyConfirmation())
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                animator.SetBool("IsMoving", false);
                return;
            }

            // Handle pause menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
                return;
            }

            // Interact with chest or sign when pressing F (but not if dialogue is showing)
            if (Input.GetKeyDown(KeyCode.F) && !DialogueManager.IsDialogueShowing())
            {
                TryInteract();
            }

            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2);
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0);
            }

            dir.Normalize();
            animator.SetBool("IsMoving", dir.magnitude > 0);

            GetComponent<Rigidbody2D>().velocity = speed * dir;
            DetectNearbyChest();
            DetectNearbySign();
            DetectNearbyDoor();
        }

        // Detect if a visible chest is nearby
        private void DetectNearbyChest()
        {
            ChestController previousChest = nearbyChest;
            nearbyChest = null;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var hit in hits)
            {
                ChestController chest = hit.GetComponent<ChestController>();
                if (chest != null && chest.IsChestVisible())
                {
                    nearbyChest = chest;
                    break;
                }
            }
            
            // Handle chest highlighting
            if (previousChest != nearbyChest)
            {
                // Player left previous chest's range
                if (previousChest != null)
                {
                    previousChest.OnPlayerExitRange();
                }
                
                // Player entered new chest's range
                if (nearbyChest != null)
                {
                    nearbyChest.OnPlayerEnterRange();
                }
            }
        }

        // Try to interact with the nearby chest, sign, or door
        private void TryInteract()
        {
            if (nearbyChest != null)
            {
                nearbyChest.TryOpenChestByPlayer();
            }
            else if (nearbySign != null)
            {
                nearbySign.TryInteractWithSign();
            }
            else if (nearbyDoor != null)
            {
                // Door interaction is handled within DoorController's Update method
                // This ensures the key check and dialogue logic work properly
            }
        }

        // Detect if a sign is nearby
        private void DetectNearbySign()
        {
            SignController previousSign = nearbySign;
            nearbySign = null;
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var hit in hits)
            {
                SignController sign = hit.GetComponent<SignController>();
                if (sign != null && sign.IsSignInteractable())
                {
                    nearbySign = sign;
                    break;
                }
            }

            // Handle highlighting
            if (previousSign != nearbySign)
            {
                // Player left previous sign's range
                if (previousSign != null)
                {
                    previousSign.OnPlayerExitRange();
                }
                
                // Player entered new sign's range
                if (nearbySign != null)
                {
                    nearbySign.OnPlayerEnterRange();
                }
            }
        }
        
        // Detect if a door is nearby
        private void DetectNearbyDoor()
        {
            DoorController previousDoor = nearbyDoor;
            nearbyDoor = null;
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var hit in hits)
            {
                DoorController door = hit.GetComponent<DoorController>();
                if (door != null && !door.IsDoorOpen())
                {
                    nearbyDoor = door;
                    break;
                }
            }
            
            // Handle door highlighting
            if (previousDoor != nearbyDoor)
            {
                // Player left previous door's range
                if (previousDoor != null)
                {
                    previousDoor.OnPlayerExitRange();
                }
                
                // Player entered new door's range
                if (nearbyDoor != null)
                {
                    nearbyDoor.OnPlayerEnterRange();
                }
            }
        }
        
        private void TogglePauseMenu()
        {
            if (pauseMenuUI != null)
            {
                bool isActive = pauseMenuUI.activeSelf;
                pauseMenuUI.SetActive(!isActive);
                
                if (!isActive)
                {
                    // Pause the game
                    Time.timeScale = 0f;
                }
                else
                {
                    // Resume the game
                    Time.timeScale = 1f;
                }
            }
        }
    }
}
