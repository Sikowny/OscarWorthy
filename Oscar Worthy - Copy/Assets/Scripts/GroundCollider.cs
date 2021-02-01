using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GroundCollider : MonoBehaviour
{
    public event EventHandler<Collider> OnGroundEnter;
    public event EventHandler<Collider> OnGroundExit;

    private void OnTriggerEnter(Collider other)
    {
        if (OnGroundEnter != null) OnGroundEnter(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (OnGroundExit != null) OnGroundExit(this, other);
    }
}
