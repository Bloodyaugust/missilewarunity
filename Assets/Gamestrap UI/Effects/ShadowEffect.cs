using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamestrap
{
    [AddComponentMenu("UI/Gamestrap UI/Shadow")]
    // Shadow Effect should be the same as Unity's default shadow effect, 
    // this was created because Unity wans't allowing to instantiate the default shadow effect component through code in the editor.
    public class ShadowEffect : BaseVertexEffect
    {
        [SerializeField]
        private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);

        [SerializeField]
        private Vector2 m_EffectDistance = new Vector2(1f, -1f);

        [SerializeField]
        private bool m_UseGraphicAlpha = true;

        #region Get and Sets
        public Color effectColor
        {
            get
            {
                return this.m_EffectColor;
            }
            set
            {
                this.m_EffectColor = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public Vector2 effectDistance
        {
            get
            {
                return this.m_EffectDistance;
            }
            set
            {
                if (this.m_EffectDistance == value)
                {
                    return;
                }
                this.m_EffectDistance = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }
        #endregion

        #region BaseVertexEffect methods
        protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            int verticesCapacity = verts.Count * 2;
            if (verts.Capacity < verticesCapacity)
            {
                verts.Capacity = verticesCapacity;
            }

            for (int i = start; i < end; i++)
            {
                UIVertex uIVertex = verts[i];
                verts.Add(uIVertex);

                Vector3 position = uIVertex.position;
                position.x += x;
                position.y += y;
                uIVertex.position = position;
                Color32 color2 = color;
                if (this.m_UseGraphicAlpha)
                {
                    color2.a = (byte)(color2.a * verts[i].color.a / 255);
                }
                uIVertex.color = color2;
                verts[i] = uIVertex;
            }
        }

        public override void ModifyVertices(List<UIVertex> verts)
        {
            if (!this.IsActive())
            {
                return;
            }

            Text foundtext = GetComponent<Text>();
            float best_fit_adjustment = 1f;
            if (foundtext && foundtext.resizeTextForBestFit)
            {
                best_fit_adjustment = (float)foundtext.cachedTextGenerator.fontSizeUsedForBestFit / (foundtext.resizeTextMaxSize - 1);

            }
            float distanceX = this.effectDistance.x * best_fit_adjustment;
            float distanceY = this.effectDistance.y * best_fit_adjustment;

            this.ApplyShadow(verts, this.effectColor, 0, verts.Count, distanceX, distanceY);
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            this.effectDistance = this.m_EffectDistance;
            base.OnValidate();
        }
        #endif
    #endregion
    }
}
