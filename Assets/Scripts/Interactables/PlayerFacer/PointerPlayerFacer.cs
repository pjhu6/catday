using UnityEngine;

public class PointerPlayerFacer : AbstractPlayerFacer
{
    public float bobSpeed = 1f;
    public float bobHeight = 0.1f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        base.Update();

        // Bob up and down slightly
        float offsetY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = initialPosition + Vector3.up * offsetY;
    }
    
    public void SetBasePosition(Vector3 worldPosition)
    {
        initialPosition = worldPosition;
        transform.position = initialPosition;
    }

}
