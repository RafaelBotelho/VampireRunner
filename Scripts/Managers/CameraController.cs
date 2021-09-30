using System.Collections;
using UnityEngine;
using EZCameraShake;
using SleekRender;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SleekRenderSettings settings; //Reference to the post processing settings

    //Subscribe to the OnHitted event
    private void OnEnable()
    {
        PlayerController.OnHitted += CameraHitEffect; //Subscribe the method CameraHitEffect to the OnHitted event
    }

    //Unsubscribe to the OnHitted event
    private void OnDisable()
    {
        PlayerController.OnHitted -= CameraHitEffect; //Unsubscribe the method CameraHitEffect to the OnHitted event
    }

    //Use a effect on the camera
    private void CameraHitEffect(int score, int hp, Color vignetteColor)
    {
        //Check if the collor is white
        if (vignetteColor == Color.white)
        {
            CameraShaker.Instance.ShakeOnce(4, 4, .1f, .5f); //Make the camera shake
        }
        StartCoroutine(ChangeColor(vignetteColor)); //Start a coroutine to use the vignette effect
    }

    //Use the vignette effect
     private IEnumerator ChangeColor(Color color)
    {
        WaitForSeconds delay = new WaitForSeconds(.1f); //Delay to continue
        settings.vignetteEnabled = true; //Enable the vignette effect
        settings.vignetteColor = color; //Set a color to the vignette

        //Loop to aplly the vignette hit effect
        while (settings.vignetteColor.a >= 0)
        {
            settings.vignetteColor.a -= .1f; //Decrease the alpha value of the vignette
            yield return delay; //Wait the delay
        }
        settings.vignetteEnabled = false; //Disable the vignette effect
    }
}
