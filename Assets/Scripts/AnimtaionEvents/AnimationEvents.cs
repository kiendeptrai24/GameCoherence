using System;
using UnityEngine;


public abstract class AnimationEvents : MonoBehaviour
{
    protected Charactor charactor;

    public ParticleSystem runParticles;
    public ParticleSystem jumpParticles;
    public ParticleSystem landParticles;

    Transform _landParticlesTransform;

    private void Awake()
    {
        _landParticlesTransform = landParticles.transform;
        charactor = GetComponentInParent<Charactor>();
    }

    public void PlayRunParticles() => runParticles.Play();
    public void StopRunParticles() => runParticles.Stop();
    public void PlayLandParticles()
    {
        Ray ray = new(transform.position + Vector3.up * .5f, Vector3.down);
        Physics.Raycast(ray, out RaycastHit raycastHit, 1.3f);
        _landParticlesTransform.position = raycastHit.point + Vector3.up * .1f;

        landParticles.Play();
    }
    public void AnimtionTrigger() => charactor.stateMachine.GetFeature<IAnimationTrigger>()?.AnimationTrigger();

    public void PlayJumpParticles()
    {
        jumpParticles.Play();
        StopRunParticles();
    }
}
