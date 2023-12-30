using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treatment : MonoBehaviour
{
    [SerializeField]
    int rotateSpeed;
    [SerializeField]
    float treatmentHp = 1;

    Rigidbody _rigidbody;
    Collider _collider;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void SetTreatmentHp(float hpCnt)
    {
        treatmentHp = hpCnt;
    }

    void Update()
    {
        Vector3 angle = transform.localEulerAngles;
        angle.y += rotateSpeed * Time.deltaTime;
        transform.localEulerAngles = angle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (!player.IsMaxHealth())
            {
                player.Treatment(treatmentHp);
                Destroy(gameObject);
            }
        }
    }
}
