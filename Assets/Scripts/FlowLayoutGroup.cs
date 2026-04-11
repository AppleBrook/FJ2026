using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FlowLayoutGroup : LayoutGroup
{
    public float spacingX = 15f;
    public float spacingY = 15f;

    public override void CalculateLayoutInputHorizontal() => base.CalculateLayoutInputHorizontal();
    public override void CalculateLayoutInputVertical() { }
    public override void SetLayoutHorizontal() => UpdateLayout();
    public override void SetLayoutVertical() { }

    private void UpdateLayout()
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        List<List<RectTransform>> rows = new List<List<RectTransform>>();
        List<float> rowWidths = new List<float>();
        List<float> rowHeights = new List<float>();

        List<RectTransform> currentRow = new List<RectTransform>();
        float currentX = padding.left;
        float currentRowWidth = 0;
        float currentRowHeight = 0;
        float totalRowsHeight = 0;

        foreach (var child in rectChildren)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(child);
            float childWidth = LayoutUtility.GetPreferredSize(child, 0);
            float childHeight = LayoutUtility.GetPreferredSize(child, 1);

            if (currentX + childWidth > width - padding.right && currentRow.Count > 0)
            {
                rows.Add(currentRow);
                rowWidths.Add(currentRowWidth);
                rowHeights.Add(currentRowHeight);
                totalRowsHeight += currentRowHeight + (rows.Count > 1 ? spacingY : 0);

                currentRow = new List<RectTransform>();
                currentX = padding.left;
                currentRowWidth = 0;
                currentRowHeight = 0;
            }

            currentRow.Add(child);
            currentRowWidth += childWidth + (currentRow.Count > 1 ? spacingX : 0);
            currentRowHeight = Mathf.Max(currentRowHeight, childHeight);
            currentX += childWidth + spacingX;
        }

        if (currentRow.Count > 0)
        {
            rows.Add(currentRow);
            rowWidths.Add(currentRowWidth);
            rowHeights.Add(currentRowHeight);
            totalRowsHeight += currentRowHeight + (rows.Count > 1 ? spacingY : 0);
        }

        // --- 修正部分：使用 Lower 而不是 Bottom ---
        float yOffset = padding.top;
        if (childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.MiddleLeft)
            yOffset = (height - totalRowsHeight) / 2f;
        else if (childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.LowerLeft)
            yOffset = height - totalRowsHeight - padding.bottom;

        float currentY = yOffset;
        for (int i = 0; i < rows.Count; i++)
        {
            float rowW = rowWidths[i];
            float xOffset = padding.left; 

            if (childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.UpperCenter || childAlignment == TextAnchor.LowerCenter)
                xOffset = (width - rowW) / 2f;
            else if (childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.UpperRight || childAlignment == TextAnchor.LowerRight)
                xOffset = width - rowW - padding.right;

            float drawX = xOffset;
            foreach (var child in rows[i])
            {
                float childWidth = LayoutUtility.GetPreferredSize(child, 0);
                float childHeight = LayoutUtility.GetPreferredSize(child, 1);
                SetChildAlongAxis(child, 0, drawX, childWidth);
                SetChildAlongAxis(child, 1, currentY, childHeight);
                drawX += childWidth + spacingX;
            }
            currentY += rowHeights[i] + spacingY;
        }
    }
}