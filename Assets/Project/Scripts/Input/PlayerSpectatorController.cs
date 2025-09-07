using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerSpectatorController : MonoBehaviour
{
    [Header("Look")]
    public Transform cameraPivot;                 // Assign your Camera
    public float mouseSensitivity = 120f;
    public float minPitch = -89f;
    public float maxPitch =  89f;

    [Header("Move")]
    public float moveSpeed = 8f;
    public float fastMultiplier = 3f;
    public float slowMultiplier = 0.3f;

    [Header("Cursor & Pause")]
    public KeyCode togglePauseKey = KeyCode.Escape;

    private CharacterController controller;
    private float yaw, pitch;
    private bool paused = false;

    [Header("Room")]
    public RoomBuilder room; 

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraPivot == null)
            cameraPivot = Camera.main != null ? Camera.main.transform : transform;

        var e = transform.eulerAngles;
        yaw = e.y;
        pitch = cameraPivot.localEulerAngles.x;

        SetPaused(false, force: true); // lock & hide cursor by default
    }

    void Start()
    {
        if (room != null)
        {
            var size = room.roomSize;
            // e.g., back corner, looking at center
            Vector3 pos = new Vector3(0f, size.y * 0.5f, -size.z * 0.4f);
            transform.position = pos;
            //transform.LookAt(new Vector3(4f, 0f, 0f));
            //yaw = transform.eulerAngles.x;
            //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            //cameraPivot.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    } 

    void Update()
    {
        // Pause toggle (Esc)
        if (Input.GetKeyDown(togglePauseKey))
            SetPaused(!paused);

        if (paused)
            return;

        // ---------- LOOK ----------
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw   += mx * mouseSensitivity * Time.deltaTime;
        pitch -= my * mouseSensitivity * Time.deltaTime;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation        = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // ---------- MOVE ----------
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftControl))  speed *= fastMultiplier;
        //if (Input.GetKey(KeyCode.LeftControl)) speed *= slowMultiplier;

        // WASD relative to camera direction (includes pitch)
        Vector3 fwd   = cameraPivot.forward;   // has vertical component → W moves "up" if you're looking up
        Vector3 right = cameraPivot.right;

        float h = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);

        Vector3 moveDir = (fwd * v + right * h);
        if (moveDir.sqrMagnitude > 1e-6f) moveDir.Normalize();

        // always world up/down regardless of view
        float upDown = (Input.GetKey(KeyCode.LeftShift) ? -1f : 0f) + (Input.GetKey(KeyCode.Space) ? 1f : 0f);
        Vector3 vertical = Vector3.up * upDown;

        Vector3 velocity = (moveDir + vertical).normalized * speed;

        controller.Move(velocity * Time.deltaTime);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // If we alt-tabbed back while unpaused, re-lock cursor
        if (hasFocus && !paused) ApplyCursor(lockCursor: true);
    }

    private void SetPaused(bool value, bool force = false)
    {
        if (!force && value == paused) return;

        paused = value;

        if (paused)
        {
            ApplyCursor(lockCursor: false);
        }
        else
        {
            ApplyCursor(lockCursor: true);
        }
    }

    private void ApplyCursor(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible   = !lockCursor;
    }
}