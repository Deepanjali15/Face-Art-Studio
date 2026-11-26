
using UnityEngine;

public class FaceSmoothing : MonoBehaviour
{
    public Transform target;

    [Range(1f, 40f)]
    public float positionSpeed = 20f;

    [Range(1f, 40f)]
    public float rotationSpeed = 20f;

    private void LateUpdate()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
            return;

        float pt = 1f - Mathf.Exp(-positionSpeed * Time.deltaTime);
        float rt = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position, target.position, pt
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation, target.rotation, rt
        );
    }
}
