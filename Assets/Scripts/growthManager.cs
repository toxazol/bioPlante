using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class growthManager : MonoBehaviour
{
    public Image growthBar;
    public float maxGrowthPoints = 100f;
    public float curGrowthPoints;
    public float photoSynthesisTimestep = 3f;

    bool isPhotoSynth = false;
    
    void Start()
    {
        curGrowthPoints = maxGrowthPoints;
        StartCoroutine(PhotoSynthesis());
    }

    void Update()
    {
        
    }

    IEnumerator PhotoSynthesis()
    {
        isPhotoSynth = true;
        while(curGrowthPoints < maxGrowthPoints)
        {
            Restore(1);
            yield return new WaitForSeconds(photoSynthesisTimestep); 
        }
        isPhotoSynth = false;
    }

    public void Grow(float growthPoints) 
    {
        updateGrowthPoints(-1*growthPoints);
        if(!isPhotoSynth)
        {
            StartCoroutine(PhotoSynthesis());
        }
    }

    public void Restore(float growthPoints)
    {
        updateGrowthPoints(growthPoints);
    }

    public void updateGrowthPoints(float growthPoints)
    {
        curGrowthPoints += growthPoints;
        curGrowthPoints = Mathf.Clamp(curGrowthPoints, 0, maxGrowthPoints);
        growthBar.fillAmount = curGrowthPoints / maxGrowthPoints;
    }
}
