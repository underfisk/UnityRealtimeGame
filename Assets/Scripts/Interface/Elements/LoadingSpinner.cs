using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LoadingSpinner : MonoBehaviour
{
    [SerializeField]
    public Sprite[] loadingSprites;

    [SerializeField]
    public float interval = 0.3f;

    private Image target;
    

    protected void Start()
    {
        if (target == null) target = gameObject.transform.GetComponent<Image>();
        if (loadingSprites.Length > 0)
            StartCoroutine(SpinLoader());
    }

    /// <summary>
    /// Corouting Enumerator for Change spinner images
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpinLoader()
    {

        int count = loadingSprites.Length;
        int index = 0;

        for (; index <= count; index++)
        {
            if (index >= count)
            {
                index = 0;
                target.sprite = loadingSprites[index];
            }

            target.sprite = loadingSprites[index];

            yield return new WaitForSeconds(interval);
        }
    }

}
