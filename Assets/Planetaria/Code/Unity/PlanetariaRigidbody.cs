﻿using UnityEngine;

namespace Planetaria
{
    public class PlanetariaRigidbody : MonoBehaviour
    {
        private void Awake()
        {
            transform = this.GetOrAddComponent<PlanetariaTransform>();
            internal_rigidbody = this.GetOrAddComponent<Rigidbody>();
            internal_rigidbody.isKinematic = true;
            internal_rigidbody.useGravity = false;
            position = transform.position.data;
            get_acceleration();
        }

        private void FixedUpdate()
        {
            if (!collision.exists)
            {
                position = transform.position.data; // FIXME: theoretically, this corrupts the velocity vector
                // I am making a bet this is relevant in spherical coordinates (it is Euler isn't it?): http://openarena.ws/board/index.php?topic=5100.0
                velocity = velocity + acceleration * (Time.deltaTime / 2);
                Vector3 next_position = PlanetariaMath.slerp(position, velocity.normalized, velocity.magnitude * Time.deltaTime); // Note: when velocity = Vector3.zero, it luckily still returns "position" intact.
                Vector3 next_velocity = PlanetariaMath.slerp(position, velocity.normalized, velocity.magnitude * Time.deltaTime + Mathf.PI/2);
                position = next_position;
                velocity = next_velocity.normalized * velocity.magnitude;
                acceleration = get_acceleration();
                velocity = velocity + acceleration * (Time.deltaTime / 2);
                transform.position = new NormalizedCartesianCoordinates(position);

                // in theory, position might need to be Orthonormalized with velocity every thousand or so frames.
            }
            else
            {

            }
        }

        public Vector3 get_acceleration()
        {
            Vector3 result = Vector3.zero; // in theory this could mess things up
            for (int force = 0; force < gravity_wells.Length; ++force)
            {
                result += Bearing.attractor(position, gravity_wells[force])*gravity_wells[force].magnitude;
            }
            return result;
        }

        public void collide(BlockCollision collision)
        {
            horizontal_velocity = Vector3.Dot(this.velocity, Bearing.right(collision.position().data, collision.normal().data));
            float vertical_velocity = Vector3.Dot(this.velocity, collision.normal().data);
            if (vertical_velocity < 0)
            {
                vertical_velocity *= -collision.elasticity;
                Debug.Log(vertical_velocity);
            }
            if (vertical_velocity > collision.magnetism)
            {
                vertical_velocity = 0;
                Debug.Log(vertical_velocity);
            }
            else
            {
                // Force OnCollisionExit, "un-collision"
            }
            
            this.collision = collision;
        }
        
        // FIXME: implement "un-collision"



        // position
        private Vector3 position; // magnitude = 1
        private Vector3 velocity; // magnitude in [0, infinity]
        private Vector3 acceleration; // magnitude in [0, infinity]

        // gravity
        [SerializeField] public Vector3[] gravity_wells;

        private new PlanetariaTransform transform;
        private Rigidbody internal_rigidbody;
        
        private optional<BlockCollision> collision;
        private float horizontal_velocity;
    }
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/