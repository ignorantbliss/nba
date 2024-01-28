using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource MusicPlayer; 
    public List<AudioClip> Clips = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {
        MusicPlayer.clip = Clips[0];
        MusicPlayer.Play();
    }

    public void PlaySong(int x)
    {
        MusicPlayer.clip = Clips[x];
        MusicPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
