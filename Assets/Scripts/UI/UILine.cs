using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILine : MaskableGraphic
{
    public RectTransform PointA;
    public Vector2 OffsetA;

    public RectTransform PointB;    
    public Vector2 OffsetB;

    Vector3 PreviousAPos;
    Vector3 PreviousBPos;

    public float LineWidth = 1;

    void BGFill(VertexHelper vh, Vector2 A, Vector2 B)
    {        
        int i = vh.currentVertCount;

        UIVertex vert = new UIVertex();
        vert.color = this.color;  // Do not forget to set this, otherwise 

        vert.position = A;
        vert.uv0 = new Vector2(0, 0);
        vh.AddVert(vert);

        vert.position = new Vector2(B.x, A.y);
        vert.uv0 = new Vector2(1, 0);
        vh.AddVert(vert);

        vert.position = B;
        vert.uv0 = new Vector2(1, 1);
        vh.AddVert(vert);

        vert.position = new Vector2(A.x, B.y);
        vert.uv0 = new Vector2(0, 1);
        vh.AddVert(vert);

        vh.AddTriangle(i + 0, i + 2, i + 1);
        vh.AddTriangle(i + 3, i + 2, i + 0);
    }

    void StraightLine(VertexHelper vh,Vector2 A, Vector2 B)
    {
        int i = vh.currentVertCount;

        Quaternion Q = Quaternion.LookRotation((B - A).normalized, Vector3.forward);
        Matrix4x4 Mat = Matrix4x4.TRS(A, Q, Vector3.one);

        Vector2 TL = Mat.MultiplyPoint(new Vector2(-LineWidth, 0));
        Vector2 TR = Mat.MultiplyPoint(new Vector2(LineWidth, 0));

        Mat = Matrix4x4.TRS(B, Q, Vector3.one);

        Vector2 BL = Mat.MultiplyPoint(new Vector2(-LineWidth, 0));
        Vector2 BR = Mat.MultiplyPoint(new Vector2(LineWidth, 0));

        UIVertex vert = new UIVertex();
        vert.color = this.color;  // Do not forget to set this, otherwise 

        vert.position = TL;
        vert.uv0 = new Vector2(0,0);
        vh.AddVert(vert);

        vert.position = BL;
        vert.uv0 = new Vector2(1, 0);
        vh.AddVert(vert);

        vert.position = BR;
        vert.uv0 = new Vector2(1,1);
        vh.AddVert(vert);

        vert.position = TR;
        vert.uv0 = new Vector2(0,1);
        vh.AddVert(vert);

        vh.AddTriangle(i + 0, i + 2, i + 1);
        vh.AddTriangle(i + 3, i + 2, i + 0);

        Debug.Log(TL + " to " + BR);        
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {        
        // Clear vertex helper to reset vertices, indices etc.
        vh.Clear();

        Vector2 bottomLeftCorner = new Vector2(0, 0) - rectTransform.pivot;
        bottomLeftCorner.x *= rectTransform.rect.width;
        bottomLeftCorner.y *= rectTransform.rect.height;

        //BGFill(vh, new Vector2(0, 0), bottomLeftCorner);

        Vector3 PA = rectTransform.InverseTransformPoint(PointA.TransformPoint(OffsetA));
        Vector3 PB = rectTransform.InverseTransformPoint(PointB.TransformPoint(OffsetB));

        StraightLine(vh, PA,PB);

        Debug.Log("Mesh was redrawn!");
    }

    void Update()
    {
        //Debug.Log("Looping - " + PointA.anchoredPosition);
        if (PreviousAPos != PointA.position)
        {
            PreviousAPos = PointA.position;
            PreviousBPos = PointB.position;
            SetVerticesDirty();
            SetMaterialDirty();
        }
        else
        {
            if (PreviousBPos != PointB.position)
            {
                PreviousBPos = PointB.position;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }
        
    }
}
