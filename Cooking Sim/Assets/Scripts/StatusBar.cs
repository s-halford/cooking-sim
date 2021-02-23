using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    [SerializeField] Transform barFill;
    [SerializeField] Transform barEmpty;
    [SerializeField] Color fillColor;
    [SerializeField] Color emptyColor;

    private float maxFillWidthScale;
    private float fillHeightScale;

    void Start()
    {
        maxFillWidthScale = barFill.transform.localScale.x;
        fillHeightScale = barFill.transform.localScale.y;
    }

    public void SetFillPercent(float fillPercent)
    {
        float barFillWidthScale = fillPercent * maxFillWidthScale;
        barFill.transform.localScale = new Vector3(barFillWidthScale, fillHeightScale, 1f);
    }
}
