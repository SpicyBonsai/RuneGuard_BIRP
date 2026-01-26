using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    Transform swivel, stick;
    public HexGrid grid;

    float zoom = 1f;

    [SerializeField]
    float moveSpeedMinZoom, moveSpeedMaxZoom;
    [SerializeField]
    float zoomSensitivity = 1f;
    [SerializeField]
    float stickMinZoom, stickMaxZoom;
    [SerializeField]
    float swivelMinZoom, swivelMaxZoom;

    static HexMapCamera instance; 
    public static bool Locked
    {
        set
        {
            instance.enabled = !value;
        }
    }
    public static void ValidatePosition()
    {
        instance.AdjustPosition(0f, 0f);
    }

    private void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);

        Camera cam = Camera.main;
        if (cam.transform.parent != stick.transform && cam != null)
        {
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.identity;
            cam.transform.SetParent(stick.transform, false );
        }
    }

    void OnEnable()
    {
        if (instance == null) { instance = this; }
    }

    void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
            damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }
}
