using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TreeDissolveComponent : MonoBehaviour
{
    public Material treeMaterial;

    private static readonly int _DISSOLVE_AMOUNT = Shader.PropertyToID("_DissolveAmount");

    private List<Renderer> _treeRenderers;

    private void Start()
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            if (treeMaterial != null)
            {
                rend.material = treeMaterial;
            }

            treeMaterial = rend.material;
        }

        treeMaterial?.SetFloat(_DISSOLVE_AMOUNT, 0.0f);

        // Find all tree renderers in the scene
        var treeRenderersObj = GameObject.FindGameObjectsWithTag("Tree");
        _treeRenderers = new List<Renderer>();
        foreach (var treeObject in treeRenderersObj)
        {
            var treeRenderer = treeObject.GetComponent<Renderer>();
            if (treeRenderer) _treeRenderers.Add(treeRenderer);
        }
    }

    public void DissolveTrees()
    {
        foreach (var treeRenderer in _treeRenderers)
        {
            var mat   = treeRenderer.material;
            var delay = Random.Range(0.0f, 1.0f);
            mat.DOFloat(1.0f, _DISSOLVE_AMOUNT, 2.0f)
               .SetEase(Ease.InOutSine)
               .SetDelay(delay)
               .OnComplete(() => { Destroy(treeRenderer.gameObject); });
        }
    }
}