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

       // UI object to display winning text.
       public GameObject winTextObject;

       private AudioSource pickupNoise;
       public GameObject explosionFX;
       public GameObject pickupFX;
       private bool gameWon = false;
       private Vector3 targetPos;
       [SerializeField] private bool isMoving = false;
       // Start is called before the first frame update.
       void Start()
       {
              // Get and store the Rigidbody component attached to the player.
              rb = GetComponent<Rigidbody>();

              // Initialize count to zero.
              count = 0;

              // Update the count display.
              SetCountText();

              // Initially set the win text to be inactive.
              winTextObject.SetActive(false);
              pickupNoise = GetComponents<AudioSource>()[0];
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
              Vector3 movement = new Vector3(movementX, 0.0f, movementY);

              // Apply force to the Rigidbody to move the player.
              rb.AddForce(movement * speed);
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
                     // Display the win text.
                     winTextObject.SetActive(true);

                     // Destroy the enemy GameObject.
                     // Destroy(GameObject.FindGameObjectWithTag("Enemy"));
                     GameObject enemy = GameObject.Find("Enemy");
                     NavMeshAgent enemyNavMeshAgent = enemy.GetComponent<NavMeshAgent>();
                     enemyNavMeshAgent.speed = 0;
                     Animator anim = enemy.GetComponentInChildren<Animator>();
                     anim.SetFloat("speed_f", 0);

                     gameWon = true;
              }
       }

       private void OnCollisionEnter(Collision collision)
       {
              if (collision.gameObject.CompareTag("Enemy") && gameWon == false)
              {
                     Instantiate(explosionFX, transform.position, Quaternion.identity);

                     collision.gameObject.GetComponent<AudioSource>().Play();
                     collision.gameObject.GetComponentInChildren<Animator>().SetFloat("speed_f", 0);

                     // Destroy the current object
                     Destroy(gameObject);

                     // Update the winText to display "You Lose!"
                     winTextObject.gameObject.SetActive(true);
                     winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";


              }

       }


}