using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MoveAndLook : MonoBehaviour, IDataPersistence
{
    //public GameObject CameraTarget;
    public GameObject MainCamera;
    public GameObject RaycastOrigin;
    private CharacterController CharacterController;
    private HmsCalculator HmsCalculatorScript;
    private LookAtTarget LookAtTargetScript;

    //player movement speeds
    private readonly int sneakSpeed = 1;
    private readonly int walkSpeed = 2;
    private readonly int runSpeed = 5;
    private readonly int sprintSpeed = 10;
    public int maxSpeed;
    private float playerSpeed;
    public float ySpeed;
    private Vector3 velocity;
    private Vector3 velocityAir;
    public Vector3 slidingSpeed;
    //jumping stuff
    private readonly float jumpHeight = 1.5f;
    private float jumpSpeed = 1;
    private readonly float jumpButtonGracePeriod = 0.1f;
    private float? lastGroundTime;
    private float? jumpButtonPressedTime;
    //player states
    public bool isMoving;
    public bool isJumping;
    public bool isGrounded;
    public bool isFalling;
    public bool isSliding;
    public bool isSneaking;
    public bool isSwimming;

    //swimming
    private Transform waterSurfaceObject;
    private float diff;
    [SerializeField] private float floatingForce;
    [SerializeField] private float waterSlowingMultiplier;
    [SerializeField] private float waterFloatingMultiplier;

    [Header("Spawning into World")]
    [SerializeField] private bool alwaysSpawnPoint;
    private Vector3 position;
    private Vector3 spawnPosition = new Vector3(0, 0, 0);

    public void LoadData(GameData data)
    {
        /*if (alwaysSpawnPoint)
        {
            transform.SetPositionAndRotation(GameObject.FindGameObjectWithTag("SpawnPoint").transform.position, 
                Quaternion.Euler(0, 125, 0));
        }
        else
        {
            transform.position = data.spawnPosition;
        }*/
        string sceneName = SceneManager.GetActiveScene().name;
        if (data.PlayerPosition.ContainsKey(sceneName))
        {
            Debug.Log("load player position saved in: " + sceneName);
            spawnPosition = data.PlayerPosition[sceneName];
            if (sceneName == "Terrain")
            {
                spawnPosition = Vector3.zero;
            }
        }
        else
        {
            Debug.Log("load spawn position from: " + sceneName);
            if (sceneName == "SampleScene")
            {
                spawnPosition = data.spawnPositionSampleScene;
            }
            else if (sceneName == "Dungeon Slime")
            {
                spawnPosition = data.spawnPositionDungeonSlime;
            }
            else if (sceneName == "DungeonGoblin")
            {
                spawnPosition = data.spawnPositionDungeonGoblin;
            }
        }
    }
    public void SaveData(ref GameData data)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (data.PlayerPosition.ContainsKey(sceneName))
        {
            data.PlayerPosition.Remove(sceneName);
        }
        data.PlayerPosition.Add(sceneName, position);
        //data.spawnPosition = transform.position;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //CameraTarget = GameObject.FindGameObjectWithTag("CinemachineTarget");
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        CharacterController = GetComponent<CharacterController>();
        CharacterController.enabled = false;
        transform.position = spawnPosition;
        Debug.Log("start player");
        CharacterController.enabled = true;
        HmsCalculatorScript = GetComponent<HmsCalculator>();
        LookAtTargetScript = GetComponent<LookAtTarget>();
    }

    void Update()
    {
        position = transform.position;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        #region cam target rotation
        //TODO camera only between -80 and +80 degrees on x-axis
        //= onyl rotate when between 280 - 360 and 0 - 80
        //float xRotation = CameraTarget.transform.eulerAngles.x;
        float cameraLeftRight = Input.GetAxis("Mouse X");
        float cameraUpdown = Mathf.Clamp(Input.GetAxis("Mouse Y"), -15, 15); //fix for now
        float xRotation = RaycastOrigin.transform.eulerAngles.x;
        //dont rotate more that 80 dwon (looking up)
        if (xRotation > 90 && xRotation < 290 && cameraUpdown > 0)
        {
            cameraUpdown = 0;
        }
        //dont rotate more that 80 up (looking down)
        if (xRotation > 70 && xRotation < 90 && cameraUpdown < 0)
        {
            cameraUpdown = 0;
        }
        Vector3 camRotation = new Vector3(cameraUpdown, -cameraLeftRight, 0);
        Vector3 rayOriginRotation = new Vector3(cameraUpdown, -cameraLeftRight, 0);
        if (LookAtTargetScript.canMoveCamera)
        {
            Debug.DrawRay(RaycastOrigin.transform.position, RaycastOrigin.transform.position - MainCamera.transform.position, Color.blue);
            Debug.DrawRay(RaycastOrigin.transform.position, Vector3.up, Color.blue);
            Vector3 lookdirection = RaycastOrigin.transform.position - MainCamera.transform.position;
            Vector3 updirection = RaycastOrigin.transform.position + Vector3.up;
            float angle = Vector3.Angle(lookdirection, Vector3.up);
            if (angle < 29)
            {
                //RaycastOrigin.transform.eulerAngles = new Vector3(290, RaycastOrigin.transform.eulerAngles.y, RaycastOrigin.transform.eulerAngles.z);
            }
            if (angle > 162)
            {
                //RaycastOrigin.transform.eulerAngles = new Vector3(70, RaycastOrigin.transform.eulerAngles.y, RaycastOrigin.transform.eulerAngles.z);
            }
            //CameraTarget.transform.eulerAngles -= camRotation;
            RaycastOrigin.transform.eulerAngles -= rayOriginRotation;
        }
        else
        {
            isMoving = false;
        }
        //float camEulerY = CameraTarget.transform.eulerAngles.y;
        float rayEulerY = RaycastOrigin.transform.eulerAngles.y;
        #endregion

        #region jumping/falling
        if (!isSwimming)
        {
            //high/low jump by pressing/tapping space
            jumpSpeed = 1;
            if (isJumping && ySpeed > 0 && Input.GetKey(KeyCode.Space) == false)
            {
                jumpSpeed = 2;
            }
            //smooth falling (-9m/s² per time)
            ySpeed += Physics.gravity.y * Time.deltaTime * jumpSpeed;
            SetSlidingSpeed();
            if (slidingSpeed == Vector3.zero)
            {
                isSliding = false;
            }

            if (CharacterController.isGrounded)
            {
                lastGroundTime = Time.time;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpButtonPressedTime = Time.time;
            }
            //jumping
            if (Time.time - lastGroundTime <= jumpButtonGracePeriod)
            {
                if (slidingSpeed != Vector3.zero)
                {
                    isSliding = true;
                }
                if (!isSliding)
                {
                    ySpeed = -1f;
                }
                isGrounded = true;
                isJumping = false;
                isFalling = false;
                if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod && HmsCalculatorScript.canJump && HmsCalculatorScript.isAlive && LookAtTargetScript.canMoveBody && !isSliding)
                {
                    //jumping jumpHeight meters
                    ySpeed = Mathf.Sqrt(jumpHeight * -3 * Physics.gravity.y);
                    isJumping = true;
                    jumpButtonPressedTime = null;
                    lastGroundTime = null;
                }
            }
            else
            {
                isGrounded = false;

                if ((isJumping && ySpeed < 0) || ySpeed < -4)
                {
                    isFalling = true;
                }
            }
        }
#endregion
#region swimming
        if (waterSurfaceObject != null)
        {
            diff = transform.position.y - waterSurfaceObject.position.y + 0.3f;
        }
        if (diff < 0)
        {
            //totalFloatingForce = Mathf.Lerp(totalFloatingForce, floatingForce, Time.deltaTime * 0.1f);
            //rb.AddForceAtPosition(Vector3.up * totalFloatingForce * Mathf.Abs(diff), transform.position, ForceMode.Force);
            //ySpeed = 0.1f;
            if (ySpeed < 0)
            {
                //slow down falling into water
                ySpeed = Mathf.Lerp(ySpeed, floatingForce * Mathf.Abs(diff) * waterSlowingMultiplier, Time.deltaTime);
            }
            else
            {
                //float back up to surface
                ySpeed = Mathf.Lerp(ySpeed, floatingForce * Mathf.Abs(diff) * waterFloatingMultiplier, Time.deltaTime);
            }
            isSwimming = true;
            isFalling = false;
        }
        else if (isSwimming && diff > 0 && diff < 0.08f)
        {
            ySpeed = 0;
        }
        else if (isSwimming && diff > 0.08f)
        {
            isSwimming = false;
        }
        if (isSwimming && Input.GetKeyDown(KeyCode.Space))
        {
            ySpeed = Mathf.Sqrt(jumpHeight * -3 * Physics.gravity.y);
            isJumping = true;
        }
#endregion

#region inputSpeed
        if (HmsCalculatorScript.isAlive && LookAtTargetScript.canMoveBody)
        {
            //player speed change by input
            if (Input.GetKey(KeyCode.LeftControl) && HmsCalculatorScript.canRun && isGrounded && !isSwimming)
            {
                maxSpeed = sprintSpeed;
                isMoving = true;
                isSneaking = false;
            }
            else if (Input.GetKey(KeyCode.LeftShift) && HmsCalculatorScript.canRun && isGrounded && !isSwimming)
            {
                maxSpeed = runSpeed;
                isMoving = true;
                isSneaking = false;
            }
            else if(!isGrounded)
            {
                maxSpeed = walkSpeed;
                isMoving = true;
            }
            else
            {
                maxSpeed = walkSpeed;
                isMoving = true;
            }
            if (horizontalInput == 0 && verticalInput == 0)
            {
                maxSpeed = 0;
                isMoving = false;
            }
            if (maxSpeed <= walkSpeed && Input.GetKeyDown(KeyCode.C))
            {
                isSneaking = !isSneaking;
            }
            if (isSneaking)
            {
                maxSpeed = sneakSpeed;
            }
            EventsManager.Instance.playerStateEvent.PlayerSneaking(isSneaking);
        }
        else
        {
            maxSpeed = 0;
        }
        #endregion

#region applying input speed on player
        playerSpeed = Mathf.Clamp01(movementDirection.magnitude) * maxSpeed;

        //applay y-rotation of CameraTarget to the movement direction
        movementDirection = Quaternion.AngleAxis(RaycastOrigin.transform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        
        movementDirection.Normalize();
        if (isGrounded || isSwimming)
        {
            velocity = movementDirection * playerSpeed;
        }
        else
        {
            velocity.y = 0;
            velocityAir = velocity + movementDirection * playerSpeed;
            velocityAir.y += ySpeed;
        }
        if (isSliding && !isSwimming)
        {
            velocity = slidingSpeed;
            velocity.y = ySpeed;
        }
        else
        {
            velocity = SmoothSlope(velocity);
            velocity.y += ySpeed;
        }
        if (isGrounded || isSwimming)
        {
            CharacterController.Move(velocity * Time.deltaTime);
        }
        else
        {
            CharacterController.Move(velocityAir * Time.deltaTime);
        }
#endregion

#region move towards look direction
        if (movementDirection != Vector3.zero && HmsCalculatorScript.isAlive && LookAtTargetScript.canMoveBody)
        {
            Quaternion toRotationMovement = Quaternion.LookRotation(movementDirection, Vector3.up);

            //set player rotation to CameraTarget rotaion
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotationMovement, 500 * Time.deltaTime);
            
            //CameraTarget.transform.localRotation = Quaternion.Euler(
               // CameraTarget.transform.localEulerAngles.x, -transform.eulerAngles.y + camEulerY, 0);
            RaycastOrigin.transform.localRotation = Quaternion.Euler(
                RaycastOrigin.transform.localEulerAngles.x, -transform.eulerAngles.y + rayEulerY, 0);
        }
#endregion
    }
    private Vector3 SmoothSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 1.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            var adjustVelocity = slopeRotation * velocity;

            if (adjustVelocity.y < 0)
            {
                return adjustVelocity;
            }
        }
        return velocity;
    }

    private void SetSlidingSpeed()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle >= CharacterController.slopeLimit)
            {
                slidingSpeed = Vector3.ProjectOnPlane(new Vector3(0, ySpeed, 0), hit.normal);
                return;
            }
        }
        if (isSliding)
        {
            slidingSpeed -= slidingSpeed * Time.deltaTime * 3;
            if (slidingSpeed.magnitude > 1)
            {
                return;
            }
        }
        slidingSpeed = Vector3.zero;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterSurfaceObject = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterSurfaceObject = null;
            isSwimming = false;
        }
    }
}
