using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigibody;

    DissolveEffect dissolveEffect;

    void Start()
    {
        myRigibody = GetComponent<Rigidbody>();
        dissolveEffect = GetComponent<DissolveEffect>();
        if (dissolveEffect.enabled)
        {
            dissolveEffect.OnAppear();
        }
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        myRigibody.MovePosition(myRigibody.position + velocity * Time.deltaTime);
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        lookPoint.y = transform.position.y;
        transform.LookAt(lookPoint);
    }
}
