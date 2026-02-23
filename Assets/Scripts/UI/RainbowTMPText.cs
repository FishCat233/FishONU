using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class RainbowTMPText : MonoBehaviour
{
    [SerializeField] private bool enableEffect = true;

    [Range(0f, 1f)][SerializeField] private float saltiness = 0f;

    [Range(0f, 10f)][SerializeField] private float rainbowSpeed = 1f;

    [SerializeField] private float waveIntensity = 10f;
    [SerializeField] private float waveSpeed = 1f;

    private TMP_Text tmp;
    private Material _rainbowMaterial;

    public TMP_Text Tmp => tmp;

    public float Saltiness
    {
        get => saltiness;
        set { saltiness = Mathf.Clamp01(value); UpdateMaterial(); }
    }

    public bool EnableEffect
    {
        get => enableEffect;
        set { enableEffect = value; UpdateMaterial(); }
    }

    void OnEnable()
    {
        tmp = GetComponent<TMP_Text>();

        if (tmp != null)
        {
            _rainbowMaterial = new Material(tmp.fontMaterial);
            tmp.fontMaterial = _rainbowMaterial;
        }

        UpdateMaterial();
    }

    void OnValidate()
    {
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (_rainbowMaterial == null) return;

        _rainbowMaterial.SetFloat("_EnableSaltiness", enableEffect ? 1f : 0f);
        _rainbowMaterial.SetFloat("_Saltiness", saltiness);
        _rainbowMaterial.SetFloat("_RainbowSpeed", rainbowSpeed);
        _rainbowMaterial.SetFloat("_WaveIntensity", waveIntensity);
        _rainbowMaterial.SetFloat("_WaveSpeed", waveSpeed);
    }
}
