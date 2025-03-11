using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace PilotoStudio
{
    public class ProjectileController : MonoBehaviour
    {

        public GameObject muzzle;
        public GameObject impact;
        [SerializeField]
        private float duration;
        [SerializeField]
        private GameObject projectileCore;
        public Vector3 Destination { get; set; }

        public Sequence AttackSequence()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(Destination, duration)).Pause();
            sequence.AppendCallback(AttackArrive).Pause(); ;
            sequence.Play();
            return sequence;
        }
        private void AttackArrive()
        {
            /*  ContactPoint contact = other.contacts[0];
              Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);*/



            Transform dummyT = transform;
            dummyT.position = this.transform.position;
            //  dummyT.rotation = rot;

            SpawnSubFX(impact, dummyT);

            if (projectileCore != null)
            {

                if (projectileCore.gameObject.TryGetComponent(out ParticleSystem p))
                {
                    p.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                    if (p.gameObject.TryGetComponent<Animation>(out Animation b))
                    {
                        b.Stop();
                    }
                }

                foreach (Transform ps in projectileCore.GetComponentsInChildren<Transform>())
                {
                    if (ps.gameObject.TryGetComponent(out ParticleSystem childP))
                    {
                        childP.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                    }

                    if (ps.gameObject.TryGetComponent<Animation>(out Animation b))
                    {
                        b.Stop();
                    }

                    if (projectileCore.gameObject.TryGetComponent(out MeshRenderer mr))
                    {
                        mr.enabled = false;
                    }
                    /*if (projectileCore.gameObject.TryGetComponent(out Rotator rotComp))
                    {
                        rotComp.enabled = false;
                    }*/
                }


            }

            foreach (Transform ps in transform.GetComponentsInChildren<Transform>())
            {
                if (ps.gameObject.TryGetComponent(out ParticleSystem p))
                {
                    if (p.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                        p.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                }

                if (ps.gameObject.TryGetComponent<Animation>(out Animation b))
                {
                    b.Stop();
                }
            }

            Destroy(gameObject, 5f);

        }

        /*  private void OnCollisionEnter(Collision other)
          {
              ContactPoint contact = other.contacts[0];
              Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);



              Transform dummyT = transform;
              dummyT.position = this.transform.position;
              dummyT.rotation = rot;

              SpawnSubFX(impact, dummyT);

              if (projectileCore != null)
              {

                  if (projectileCore.gameObject.TryGetComponent(out ParticleSystem p))
                  {
                      p.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                      if (p.gameObject.TryGetComponent<Animation>(out Animation b))
                      {
                          b.Stop();
                      }
                  }

                  foreach (Transform ps in projectileCore.GetComponentsInChildren<Transform>())
                  {
                      if (ps.gameObject.TryGetComponent(out ParticleSystem childP))
                      {
                          childP.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                      }

                      if (ps.gameObject.TryGetComponent<Animation>(out Animation b))
                      {
                          b.Stop();
                      }

                      if (projectileCore.gameObject.TryGetComponent(out MeshRenderer mr))
                      {
                          mr.enabled = false;
                      }
                      /*if (projectileCore.gameObject.TryGetComponent(out Rotator rotComp))
                      {
                          rotComp.enabled = false;
                      }
                  }


              }

              foreach (Transform ps in transform.GetComponentsInChildren<Transform>())
              {
                  if (ps.gameObject.TryGetComponent(out ParticleSystem p))
                  {
                      if (p.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                          p.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                  }

                  if (ps.gameObject.TryGetComponent<Animation>(out Animation b))
                  {
                      b.Stop();
                  }
              }

              Destroy(gameObject, 5f);

          }

  */

        public void SpawnSubFX(GameObject fx, Transform spawnPos)
        {
            GameObject instance = Instantiate(fx, spawnPos.position, spawnPos.rotation);
            instance.GetComponent<ParticleSystem>().Play(true);
            Destroy(instance, 5f);
        }
    }
}