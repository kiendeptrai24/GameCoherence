

using UnityEngine;

public class AnimationSync : MonoBehaviour
{
    private Animator _anim;
    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
    }
    public void SetBool(string animName, bool value) => _anim.SetBool(animName, value);
    public void SetFloat(string animName, int value) => _anim.SetFloat(animName, value);

}