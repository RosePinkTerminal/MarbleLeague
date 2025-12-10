using Unity.Mathematics;
using UnityEngine;



public class FPSCamera : MonoBehaviour
{
    public Transform target;   
    private float desiredX;
    private Quaternion desiredRotation;

    



    
void LateUpdate()
    {

        if(Input.GetMouseButton(1))
        {
            desiredX -= Input.GetAxis("Mouse X") * 5.0f;
            desiredX = Mathf.Clamp(desiredX, -360f, 360f);

        }

        desiredRotation = Quaternion.Euler(transform.rotation.x , desiredX, transform.rotation.z);

        transform.position = target.position;
        transform.rotation = desiredRotation;

        }    
    }