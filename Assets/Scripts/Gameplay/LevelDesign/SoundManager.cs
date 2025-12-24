using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private List<AudioSource> _sourcePool;
    public  int               poolSize = 32;

    private Dictionary<AudioClip, float> _lastPlayedTimes;

    private float _minimalInterval = 0.1f;

    public void Init()
    {
        _sourcePool = new List<AudioSource>();
        var poolHolder = new GameObject("AudioSourcePool");
        poolHolder.transform.SetParent(transform);

        for (var i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"AudioSource_{i}");
            go.transform.SetParent(poolHolder.transform);

            var source = go.AddComponent<AudioSource>();
            source.playOnAwake  = false;
            source.rolloffMode  = AudioRolloffMode.Linear;

            _sourcePool.Add(source);
        }
    }

    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1.0f, float pitchVariation = 0.1f)
    {
        if (!clip)
            return;

        _lastPlayedTimes ??= new Dictionary<AudioClip, float>();

        if (_lastPlayedTimes.TryGetValue(clip, out var lastPlayedTime))
        {
            if (Time.time - lastPlayedTime < _minimalInterval)
                return;
        }

        foreach (var source in _sourcePool)
        {
            if (source.isPlaying) continue;

            source.transform.position = position;

            source.clip   = clip;
            source.volume = volume;
            source.pitch  = 1.0f + Random.Range(-pitchVariation, pitchVariation);
            source.Play();

            _lastPlayedTimes[clip] = Time.time;

            break;
        }
    }
}