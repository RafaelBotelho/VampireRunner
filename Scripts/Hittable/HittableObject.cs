using UnityEngine;

[CreateAssetMenu]
public class HittableObject : ScriptableObject
{
    public int blood; //Number of blood the object provide
    public int hp; //Number of hp the object provide
    public string vampireAnimation; //Name of the animation to be played by the vampire
    public string hittableAnimation; //Name of the animation to be played by the hittable object
    public Color vignetteColor; //Color of the vignette post processing to be played
    public AudioClip hitClip; //audio to be played when hitted
}
