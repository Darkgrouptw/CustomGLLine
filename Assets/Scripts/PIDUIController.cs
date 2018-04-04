using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDUIController : MonoBehaviour
{
    // 外面定義的參數
    public int Aggressive = 5;
    public int Stable = 5;
    public int Tolerate = 5;

    // 計算的參數
    private float ScaleSinValue = 10;                       // Sin 波的變數
    private float GoalValue = 0.8f;                         // 目標值
    private float StartChange = 0.15f;                      // 開始從 1 往下掉
    private float ScaleAfterChnage = 4;                     // 後來調動的變數

    public float PIDFunction(float x)
    {
        #region 參數計算
        // Aggressive
        ScaleSinValue = 0.91f + 11 * (float)Aggressive / 10;
        StartChange = Mathf.PI / 2 / ScaleSinValue;

        // Stable
        ScaleAfterChnage = CircleFunction((float)Stable / 10) * 1.5f;

        // Tolerate
        float TolerateValue = CircleFunction((float)Tolerate / 10) * 5 + 5;
        #endregion
        #region Function 運算
        // 0.5
        if (x < StartChange)
            return Mathf.Sin(ScaleSinValue * x);
        else
        {
            float baseWave = Mathf.Sin(ScaleSinValue * ScaleAfterChnage * ScaleAfterChnage * (x - StartChange) + Mathf.PI / 2);
            float StableWave = baseWave * Mathf.Pow(1 - x + StartChange, TolerateValue);
            float ScaleBaseWave = StableWave * (1 - GoalValue) + GoalValue;
            return ScaleBaseWave;
        }
        #endregion
    }

    private float CircleFunction(float x)
    {
        return Mathf.Sqrt(1 - x * x) + 1;
    }
}
