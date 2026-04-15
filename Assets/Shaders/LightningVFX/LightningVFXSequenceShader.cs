using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class LightningVFXSequenceShader : MonoBehaviour
{
    private Material cloudMaterial;
    private Material lightningStrikeMaterial;
    private Material splatterMaterial;

    private float cloudLerpT = 0f;
    private float lightningStrikeLerpT = 0f;

    void Start()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        cloudMaterial = meshRenderers[1].material;
        lightningStrikeMaterial = meshRenderers[2].material;
        splatterMaterial = meshRenderers[3].material;
    }

    [Button("Run Lightning VFX", EButtonEnableMode.Playmode)]
    public void PlayLightningEffect()
    {
        StartCoroutine(LightningVFX());   
    }

    private IEnumerator LightningVFX()
    {
        cloudLerpT = 0;
        while (cloudMaterial.GetFloat("_ErosionThreshold") > .01f)
        {
            cloudLerpT += .05f;
            float interpolatedValue = Mathf.Lerp(1, 0.01f, cloudLerpT);
            cloudMaterial.SetFloat("_ErosionThreshold", interpolatedValue);
            yield return new WaitForSeconds(.05f);
        }

        yield return new WaitForSeconds(.5f);

        int emissiveBlinkCount = 0;

        while (emissiveBlinkCount < 3)
        {
            cloudMaterial.SetInteger("_Emission", 1);
            yield return new WaitForSeconds(.2f);
            cloudMaterial.SetInteger("_Emission", 0);
            yield return new WaitForSeconds(.2f);
            
            emissiveBlinkCount++;
        }
        
        cloudMaterial.SetInteger("_Emission", 1);

        lightningStrikeLerpT = 0f;
        while (lightningStrikeMaterial.GetFloat("_AnimationRange") > 0f)
        {
            lightningStrikeLerpT += .45f;
            float interpolatedValue = Mathf.Lerp(1, 0, lightningStrikeLerpT);
            lightningStrikeMaterial.SetFloat("_AnimationRange", interpolatedValue);
            yield return new WaitForSeconds(.02f);
        }
        lightningStrikeMaterial.SetFloat("_AnimationRange", 1);
        cloudMaterial.SetInteger("_Emission", 0);

        for (int i = 0; i < 49; i++)
        {
            splatterMaterial.SetFloat("_Slice", i);
            yield return new WaitForSeconds(.05f);
        }

        cloudLerpT = 0;
        while (cloudMaterial.GetFloat("_ErosionThreshold") < 1f)
        {
            cloudLerpT += .05f;
            float interpolatedValue = Mathf.Lerp(0.01f, 1f, cloudLerpT);
            cloudMaterial.SetFloat("_ErosionThreshold", interpolatedValue);
            yield return new WaitForSeconds(.05f);
        }
    }

}