using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;


    void Awake()
    {
        if(instance == null){
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume){

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject,clipLength);
    }
}
