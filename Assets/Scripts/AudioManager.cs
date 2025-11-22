using UnityEngine;

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource footsetp1;
    public AudioSource chop1;
    public AudioSource enemy1;
    public AudioSource down;
    public AudioSource soda1;
    public AudioSource fruit1;
    public AudioSource fruit2;



    public void PlayFootstep() => footsetp1.Play();
    public void PlayChop() => chop1.Play();
    public void PlayEnemy() => enemy1.Play();
    public void PlayDown() => down.Play();
    public void PlaySoda() => soda1.Play();
    public void PlayFruit() => fruit1.Play();
    public void PlayBroke() => fruit2.Play();

}

