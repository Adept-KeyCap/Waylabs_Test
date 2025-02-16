using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefeatScreen : MonoBehaviour
{
    public TMP_Text text;
    public GameObject buttons;

    private void Start()
    {
        text.color = Color.clear;
    }

    private void OnEnable()
    {
        StartCoroutine(DefeatSequence());
    }

    private IEnumerator DefeatSequence()
    {
        text.DOColor(Color.white, 2).OnComplete(() => { DOTween.Kill(text); });

        yield return new WaitForSeconds(3);

        buttons.SetActive(true);
    }
}
