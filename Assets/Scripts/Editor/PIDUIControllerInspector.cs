using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(PIDUIController))]
public class PIDUIControllerInspector : Editor
{
    // 一定要有這個他才會畫
    private Material mat;

    // 繪畫相關參數
    private const int RectWidth = 200;
    private const int RectHeight = 200;
    private int TicOffset = 10;                 // 坐標軸 offset


    private void OnEnable()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);
    }
    private void OnDisable()
    {
        DestroyImmediate(mat);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        PIDUIController pid = (PIDUIController)target;

        EditorGUILayout.LabelField("========== 參數 ==========");

        #region 積極性
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("積極性", GUILayout.MaxWidth(40));
        pid.Aggressive = EditorGUILayout.IntSlider(pid.Aggressive, 0, 10, GUILayout.MinWidth(150));
        EditorGUILayout.EndHorizontal();
        #endregion
        #region 平穩性
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("平穩性", GUILayout.MaxWidth(40));
        pid.Stable = EditorGUILayout.IntSlider(pid.Stable, 0, 10, GUILayout.MinWidth(150));
        EditorGUILayout.EndHorizontal();
        #endregion
        #region 容忍性
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("容忍性", GUILayout.MaxWidth(40));
        pid.Tolerate = EditorGUILayout.IntSlider(pid.Tolerate, 0, 10, GUILayout.MinWidth(150));
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("========== 示意圖 ==========");
        
        #region 曲線示意圖
        Rect rect = GUILayoutUtility.GetRect(0, 0, RectWidth + TicOffset * 2, RectHeight + TicOffset * 2);
        Debug.Log(rect);
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            //GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            mat.SetPass(0);
            
            // 左上角為 0, 0
            #region 背景
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(rect.width, 0, 0);
            GL.Vertex3(rect.width, rect.height, 0);
            GL.Vertex3(0, rect.height, 0);
            GL.End();
            #endregion
            #region X 坐標軸
            // 線
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex3(TicOffset, rect.height - TicOffset, 0);
            GL.Vertex3(rect.width - TicOffset, rect.height - TicOffset, 0);
            GL.End();

            // 三角形
            //GL.Begin(GL.TRIANGLES);
            #endregion
            #region Y 坐標軸
            // 線
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex3(TicOffset, rect.height - TicOffset, 0);
            GL.Vertex3(TicOffset, TicOffset, 0);
            //GL.Vertex3f
            GL.End();

            // 三角形
            //GL.Begin(GL.TRIANGLES);
            #endregion
            #region 虛線
            GL.Begin(GL.LINES);
            float LightWhite = 1.0f / 2;

            GL.Color(new Color(LightWhite, LightWhite, LightWhite));
            for(int i = 1; i < 10 ; i += 1)
            {
                // X
                float tempX = (float)i / 10;
                Vector2 startPos = GLCordToPixelCord(tempX, 0);
                Vector2 endPos = GLCordToPixelCord(tempX, 1);
                GL.Vertex3(startPos.x, startPos.y, 0);
                GL.Vertex3(endPos.x, endPos.y, 0);

                // Y
                startPos = GLCordToPixelCord(0, tempX);
                endPos = GLCordToPixelCord(1, tempX);
                GL.Vertex3(startPos.x, startPos.y, 0);
                GL.Vertex3(endPos.x, endPos.y, 0);
            }
            GL.End();
            #endregion
            #region 畫線
            GL.Begin(GL.LINES);
            GL.Color(Color.yellow);
            for(int i = 1; i < 1000; i++)
            {
                float lastX = (float)(i - 1) / 1000;
                float lastY = pid.PIDFunction(lastX);
                float x = (float)i / 1000;
                float y = pid.PIDFunction(x);
                Vector2 startPos = GLCordToPixelCord(lastX, lastY);
                Vector2 endPos = GLCordToPixelCord(x, y);

                if(IsInside(startPos) && IsInside(endPos))
                {
                    GL.Vertex3(startPos.x, startPos.y, 0);
                    GL.Vertex3(endPos.x, endPos.y, 0);
                }
            }
            GL.End();
            #endregion
            GUI.EndClip();
        }
        #endregion
    }

    // 因為座標是從 0 ~ 1
    // 所以結果轉換到 OnInspectorGUI 上，要有一個座標轉換
    private Vector2 GLCordToPixelCord(float x, float y)
    {
        return new Vector2(x * RectWidth + TicOffset, RectHeight * (1 - y) + TicOffset);
    }

    // 是否在內圈
    private bool IsInside(Vector2 pos)
    {
        if (0 <= pos.x && pos.x < RectWidth + TicOffset * 2 && 0 <= pos.y && pos.y < RectHeight + TicOffset * 2)
            return true;
        return false;
    }
}
