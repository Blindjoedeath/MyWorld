using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour {

    protected ParticleSystem currParticleSystem;


    public float Duration
    {
        get
        {
            return currParticleSystem.main.duration;
        }
    }

    private void Start()
    {
        currParticleSystem = GetComponent<ParticleSystem>();
    }

    public virtual void Play()
    {
        if (!currParticleSystem.isPlaying)
        {
            currParticleSystem.Play();
        }
    }

    public virtual void Pause()
    {
        if(currParticleSystem.isPlaying)
        {
            currParticleSystem.Pause();
        }
    }

    public virtual void Stop(bool withChildren, ParticleSystemStopBehavior behaviour)
    {
        currParticleSystem.Stop(withChildren, behaviour);
    }

    public virtual void Emit(int count)
    {
        currParticleSystem.Emit(count);
    }
}
