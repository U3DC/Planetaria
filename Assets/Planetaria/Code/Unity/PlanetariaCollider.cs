﻿using System.Collections;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    public class PlanetariaCollider : MonoBehaviour // FIXME: while not incredibly complicated, I think there might be a way to simplify this
    {
        public float scale
        {
            get
            {
                return scale_variable;
            }
            set
            {
                scale_variable = value;
                if (scalable)
                {
                    colliders = new Sphere[] { Sphere.ideal_collider(internal_transform, new Plane(Vector3.forward, Mathf.Cos(value/2))) };
                    set_internal_collider(colliders[0]);
                    internal_collider.center = Vector3.forward * internal_collider.center.magnitude;
                }
            }
        }

        public void register(PlanetariaMonoBehaviour listener)
        {
            observer.register(listener);
        }

        public void unregister(PlanetariaMonoBehaviour listener)
        {
            observer.unregister(listener);
        }

        public CollisionObserver get_observer()
        {
            return observer;
        }

        public SphereCollider get_sphere_collider()
        {
            return internal_collider;
        }

        public void set_colliders(Sphere[] colliders)
        {
            this.colliders = colliders;
            Sphere furthest_collider = colliders.Aggregate(
                    (furthest, next_candidate) =>
                    furthest.center.magnitude - furthest.radius >
                    next_candidate.center.magnitude - next_candidate.radius
                    ? furthest : next_candidate);

            set_internal_collider(furthest_collider);
        }

        private void set_internal_collider(Sphere sphere)
        {
            Quaternion local_to_world = internal_collider.transform.rotation;

            internal_collider.center = local_to_world * sphere.center;
            internal_collider.radius = sphere.radius;
        }

        public bool is_field
        {
            get
            {
                return is_field_variable;
            }
            set
            {
                is_field_variable = value;
            }
        }

        private void Awake()
        {
            GameObject child_for_collision = new GameObject("SphereCollider");
            child_for_collision.transform.parent = this.gameObject.transform;
            internal_collider = child_for_collision.transform.GetOrAddComponent<SphereCollider>();
            internal_transform = this.GetOrAddComponent<Transform>();
            planetaria_transform = this.GetOrAddComponent<PlanetariaTransform>();
            rigidbody = this.GetComponent<PlanetariaRigidbody>();
            observer.initialize(planetaria_transform, this.GetComponentsInParent<PlanetariaMonoBehaviour>());
            // add to collision_map and trigger_map for all objects currently intersecting (via Physics.OverlapBox()) // CONSIDER: I think Unity Fixed this, right?

            StartCoroutine(wait_for_fixed_update());
        }

        private void OnTriggerStay(Collider collider)
        {
            optional<SphereCollider> sphere_collider = collider as SphereCollider;
            if (!sphere_collider.exists)
            {
                Debug.LogError("This should never happen");
                return;
            }
            optional<PlanetariaCollider> other_collider = PlanetariaCache.collider_cache.get(sphere_collider.data);
            if (!other_collider.exists)
            {
                Debug.LogError("This should never happen");
                return;
            }

            if (!(this.is_field && other_collider.data.is_field)) // fields pass through each other (same as triggers)
            {
                if (PlanetariaIntersection.collider_collider_intersection(this.colliders, other_collider.data.colliders))
                {
                    if (this.is_field || other_collider.data.is_field) // field collision
                    {
                        observer.potential_field_collision(other_collider.data); // TODO: augment field (like Unity triggers) works on both the sender and receiver.
                    }
                    else // block collision
                    {
                        if (rigidbody.exists)
                        {
                            optional<Block> block = PlanetariaCache.block_cache.get(sphere_collider.data);
                            if (block.exists &&
                                    (!observer.colliding() || !observer.collisions()[0].block.ignore.Contains(block.data))) // Should we ignore the new block? // FIXME: private API exposure (optimizes at a cost of readability)
                            {
                                optional<Arc> arc = PlanetariaCache.arc_cache.get(sphere_collider.data);
                                Vector3 position = planetaria_transform.position.data;
                                if (block.data.is_dynamic)
                                {
                                    position = Quaternion.Inverse(block.data.gameObject.transform.rotation) * position;
                                }
                                if (arc.exists && arc.data.contains(position, planetaria_transform.scale/2))
                                {
                                    observer.potential_block_collision(arc.data, block.data, other_collider.data); // block collisions are handled in OnCollisionStay(): notification stage
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator wait_for_fixed_update()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                observer.notify();
            }
        }

        [SerializeField] public PlanetariaPhysicMaterial material = fallback;
        private CollisionObserver observer = new CollisionObserver();
        private Transform internal_transform;
        private PlanetariaTransform planetaria_transform;
        private SphereCollider internal_collider;
        private new optional<PlanetariaRigidbody> rigidbody;
        [SerializeField] public Sphere[] colliders = new Sphere[0]; // FIXME: private
        private float scale_variable;
        public bool scalable = false;
        private bool is_field_variable = false;
        public static PlanetariaPhysicMaterial fallback;
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