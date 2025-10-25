

using UnityEngine;

namespace Coherence.FirstSteps
{
    /// <summary>
    /// Reacts to animation events invoked from within player's Animation Clips, to play particles.
    /// </summary>
    public class RobotAnimationEvents : MonoBehaviour
    {
        private RobotController _robot;

        public ParticleSystem runParticles;
        public ParticleSystem jumpParticles;
        public ParticleSystem landParticles;

        Transform _landParticlesTransform;

        private void Awake()
        {
            _landParticlesTransform = landParticles.transform;
            _robot = GetComponentInParent<RobotController>();
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
        public void AnimtionTrigger() => _robot.m_robotSM.GetFeature<IAnimationTrigger>()?.AnimationTrigger();

        public void PlayJumpParticles()
        {
            jumpParticles.Play();
            StopRunParticles();
        }
    }
}
