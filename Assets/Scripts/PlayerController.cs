using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.AI;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
       // Rigidbody of the player.
       private Rigidbody rb;

       // Variable to keep track of collected "PickUp" objects.
       private int count;

       // Movement along X and Y axes.
       private float movementX;
       private float movementY;

       // Speed at which the player moves.
       public float speed = 0;

       // UI text component to display count of "PickUp" objects collected.
       public TextMeshProUGUI countText;
       public GameObject countObject;
       // UI object to display winning text.
       public TextMeshProUGUI winTextObject;

       private AudioSource pickupNoise;
       public GameObject explosionFX;
       public GameObject pickupFX;
       private Vector3 targetPos;
       [SerializeField] private bool isMoving = false;
       public GameObject canvas;
       public Vector2 respawnX;
       public Vector2 respawnY;
       private GameObject[] allPickups;
       // Start is called before the first frame update.
       void Start()
       {
              // Get and store the Rigidbody component attached to the player.
              rb = GetComponent<Rigidbody>();

              // Initially set the win text to be inactive.
              pickupNoise = GetComponents<AudioSource>()[0];
              allPickups = GameObject.FindGameObjectsWithTag("PickUp");

       }

       public void Restart()
       {
              // Reset the player's pickup count
              count = 0;
              SetCountText();

              // Reactivate player and set to a new random position
              gameObject.SetActive(true);
              float randomX = Random.Range(respawnX.x, respawnX.y);
              float randomZ = Random.Range(respawnY.x, respawnY.y);
              transform.position = new Vector3(randomX, transform.position.y, randomZ);
              if (rb != null)
              {
                     rb.linearVelocity = Vector3.zero;

              }

              // Hide win/lose canvas and text
              canvas.SetActive(false);
              countObject.SetActive(true);
              winTextObject.text = "";

              // Reactivate all pickups
              if (allPickups != null)
              {
                     foreach (GameObject pickup in allPickups)
                     {
                            if (pickup != null)
                                   pickup.SetActive(true);
                     }
              }

              // Reactivate all enemies
              GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
              foreach (GameObject enemy in enemies)
              {
                     enemy.SetActive(true);
                     Animator anim = enemy.GetComponentInChildren<Animator>();
                     if (anim != null)
                     {
                            anim.SetFloat("speed_f", 1); // Resume animation if used
                     }
              }

       }

       private void Update()
       {
              if (Input.GetMouseButton(0)) // Check if left mouse button is held down
              {
                     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                     Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);
                     RaycastHit hit;
                     if (Physics.Raycast(ray, out hit))
                     {
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                            {
                                   targetPos = hit.point; // Set target position
                                   isMoving = true; // Start player movement
                            }
                     }
              }
              else
              {
                     isMoving = false;
              }
       }

       // This function is called when a move input is detected.
       void OnMove(InputValue movementValue)
       {
              // Convert the input value into a Vector2 for movement.
              Vector2 movementVector = movementValue.Get<Vector2>();

              // Store the X and Y components of the movement.
              movementX = movementVector.x;
              movementY = movementVector.y;
       }

       // FixedUpdate is called once per fixed frame-rate frame.
       private void FixedUpdate()
       {
              // Create a 3D movement vector using the X and Y inputs.
              // Get the camera's forward and right directions (flattened on Y)
              Vector3 cameraForward = Camera.main.transform.forward;
              cameraForward.y = 0f;
              cameraForward.Normalize();

              Vector3 cameraRight = Camera.main.transform.right;
              cameraRight.y = 0f;
              cameraRight.Normalize();

              // Create movement direction relative to camera
              Vector3 moveDir = cameraRight * movementX + cameraForward * movementY;
              rb.AddForce(moveDir.normalized * speed);
              if (isMoving)
              {
                     // Move the player towards the target position
                     Vector3 direction = targetPos - rb.position;
                     direction.Normalize();
                     rb.AddForce(direction * speed);
              }
              if (Vector3.Distance(rb.position, targetPos) < 0.5f)
              {
                     isMoving = false;
              }
       }


       void OnTriggerEnter(Collider other)
       {
              // Check if the object the player collided with has the "PickUp" tag.
              if (other.gameObject.CompareTag("PickUp"))
              {
                     var currentPickupFX = Instantiate(pickupFX, other.transform.position, Quaternion.identity);
                     Destroy(currentPickupFX, 3);

                     pickupNoise.Play();
                     // Deactivate the collided object (making it disappear).
                     other.gameObject.SetActive(false);

                     // Increment the count of "PickUp" objects collected.
                     count = count + 1;

                     // Update the count display.
                     SetCountText();
              }
       }

       // Function to update the displayed count of "PickUp" objects collected.
       void SetCountText()
       {
              // Update the count text with the current count.
              countText.text = "Count: " + count.ToString();

              // Check if the count has reached or exceeded the win condition.
              if (count >= 12)
              {

                     GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                     foreach (GameObject enemy in enemies)
                     {
                            enemy.SetActive(false);
                     }
                     winTextObject.SetText("You win!");
                     canvas.SetActive(true);
                     countObject.SetActive(false);
              }

       }

       private void OnCollisionEnter(Collision collision)
       {
              if (collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.CompareTag("Enemy"))
              {
                     Instantiate(explosionFX, transform.position, Quaternion.identity);

                     collision.gameObject.GetComponent<AudioSource>().Play();

                     // Destroy the current object
                     gameObject.SetActive(false);

                     // Update the winText to display "You Lose!"
                     canvas.SetActive(true);
                     countObject.SetActive(false);
                     winTextObject.SetText("You lose!");


              }

       }


}