using DG.Tweening;
using UnityEngine;

public class MusicManagerComponent : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip WaveMusicClip;
    public AudioClip BossMusicClip;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume;
    
    [Range(0f, 1f)]
    public float lowUIVolume;

    private AudioSource _audioSource;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        PlayWaveMusic();
        
        Game.MusicManager = this;
    }
    
    public void PlayWaveMusic()
    {
        _audioSource.DOFade(0, 2.0f).OnComplete(() =>
        {
            _audioSource.clip = WaveMusicClip;
            _audioSource.loop = true;
            _audioSource.Play();
            _audioSource.DOFade(musicVolume, 2.0f);
        });
    }
    
    public void PlayBossMusic()
    {
        _audioSource.DOFade(0, 2.0f).OnComplete(() =>
        {
            _audioSource.clip = BossMusicClip;
            _audioSource.loop = true;
            _audioSource.Play();
            _audioSource.DOFade(musicVolume, 2.0f);
        });
    }
    
    public void FadeOutMusic(float duration)
    {
        _audioSource.DOFade(0, duration).OnComplete(() => _audioSource.Stop());
    }
    
    public void FadeToVolume(float targetVolume, float duration)
    {
        _audioSource.DOFade(targetVolume, duration);
    }
}
