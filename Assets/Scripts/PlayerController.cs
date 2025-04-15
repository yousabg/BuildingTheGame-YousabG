using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.AI;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
       private Rigidbody rb;

       private int count;

       private float movementX;
       private float movementY;

       public float speed = 0;

       public TextMeshProUGUI countText;
       public GameObject countObject;
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
       void Start()
       {
              rb = GetComponent<Rigidbody>();

              pickupNoise = GetComponents<AudioSource>()[0];
              allPickups = GameObject.FindGameObjectsWithTag("PickUp");

       }

       public void Restart()
       {
              count = 0;
              SetCountText();

              gameObject.SetActive(true);
              float randomX = Random.Range(respawnX.x, respawnX.y);
              float randomZ = Random.Range(respawnY.x, respawnY.y);
              transform.position = new Vector3(randomX, transform.position.y, randomZ);
              if (rb != null)
              {
                     rb.linearVelocity = Vector3.zero;

              }

              canvas.SetActive(false);
              countObject.SetActive(true);
              winTextObject.text = "";

              if (allPickups != null)
              {
                     foreach (GameObject pickup in allPickups)
                     {
                            if (pickup != null)
                                   pickup.SetActive(true);
                     }
              }

              GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
              foreach (GameObject enemy in enemies)
              {
                     enemy.SetActive(true);
                     Animator anim = enemy.GetComponentInChildren<Animator>();
                     if (anim != null)
                     {
                            anim.SetFloat("speed_f", 1);
                     }
              }

       }

       private void Update()
       {
              if (Input.GetMouseButton(0))
              {
                     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                     Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);
                     RaycastHit hit;
                     if (Physics.Raycast(ray, out hit))
                     {
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                            {
                                   targetPos = hit.point;
                                   isMoving = true;
                            }
                     }
              }
              else
              {
                     isMoving = false;
              }
       }

       void OnMove(InputValue movementValue)
       {
              Vector2 movementVector = movementValue.Get<Vector2>();

              movementX = movementVector.x;
              movementY = movementVector.y;
       }

       private void FixedUpdate()
       {
              Vector3 cameraForward = Camera.main.transform.forward;
              cameraForward.y = 0f;
              cameraForward.Normalize();

              Vector3 cameraRight = Camera.main.transform.right;
              cameraRight.y = 0f;
              cameraRight.Normalize();

              Vector3 moveDir = cameraRight * movementX + cameraForward * movementY;
              rb.AddForce(moveDir.normalized * speed);
              if (isMoving)
              {
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
              if (other.gameObject.CompareTag("PickUp"))
              {
                     var currentPickupFX = Instantiate(pickupFX, other.transform.position, Quaternion.identity);
                     Destroy(currentPickupFX, 3);

                     pickupNoise.Play();
                     other.gameObject.SetActive(false);

                     count = count + 1;

                     SetCountText();
              }
       }

       void SetCountText()
       {
              countText.text = "Count: " + count.ToString();

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

                     gameObject.SetActive(false);

                     canvas.SetActive(true);
                     countObject.SetActive(false);
                     winTextObject.SetText("You lose!");


              }

       }


}