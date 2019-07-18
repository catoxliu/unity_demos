using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    private static AudioManager _Insatance;

    public static AudioManager Instance
    {
        get
        {
            return _Insatance;
        }
    }

    private AudioSource Audio;

	// Use this for initialization
	void Start () {
		if (_Insatance == null)
        {
            _Insatance = this;
        }
        else
        {
            Debug.LogError("There are more than 1 Audio Manager!");
        }
        Audio = GetComponent<AudioSource>();
        if (Audio == null)
        {
            Debug.LogError("Audio Manager can't find any AudioSource!");
        }
    }

    void OnDestroy()
    {
        _Insatance = null;
    }

    public void Play()
    {
        Audio.Play();
    }
}
