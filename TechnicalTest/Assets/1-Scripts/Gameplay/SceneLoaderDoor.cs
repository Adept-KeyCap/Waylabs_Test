using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderDoor : MonoBehaviour
{
    public static SceneLoaderDoor Instance;

    public int sceneIndex;
    public Image whiteFlashPanel;

    public Color fadeInColor;
    public Color fadeOutColor;

    private Collider colliderSelf;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        colliderSelf = GetComponent<Collider>();
        if (sceneIndex == 0)
        {
            colliderSelf.enabled = false;
            whiteFlashPanel.gameObject.SetActive(true);
        }

        if (whiteFlashPanel.color == fadeInColor)
        {
            whiteFlashPanel.DOColor(fadeOutColor, 3).OnComplete(() => { DOTween.Kill(whiteFlashPanel); });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(WhiteFlashAndLoadScene());
    }

    private IEnumerator WhiteFlashAndLoadScene()
    {
        whiteFlashPanel.gameObject.SetActive(true);
        whiteFlashPanel.DOColor(fadeInColor, 1).OnComplete(() => { DOTween.Kill(whiteFlashPanel); });

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneIndex);
    }

    public void OpenExit()
    {
        colliderSelf.enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
