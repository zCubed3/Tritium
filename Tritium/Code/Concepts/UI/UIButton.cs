using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Tritium.Input;

namespace Tritium.Concepts.UI
{
    public class UIButton : UIPanel
    {
        public Color baseColor = Color.DarkGray;
        public Color hoverColor = Color.Gray;
        public Color clickColor = Color.LightGray;

        protected Color interpolateColor = Color.White;

        public Action onClick, onRelease, onEnter, onExit, onHold;

        protected override Color TargetColor => interpolateColor;

        public bool scaleToText = false;
        public float colorLerpSpeed = 15;

        protected UIText textEntity;
        public string Text
        {
            set 
            {
                if (textEntity == null)
                {
                    textEntity = new UIText(value, Assets.ScalableFont.HAlignment.Center, Assets.ScalableFont.VAlignment.Middle) { ignoreScale = false };
                    textEntity.Depth -= 0.01F;
                    Parent(textEntity);
                }
                else
                    textEntity.text = value;
            }
        }

        public UIButton(Color? baseColor = null) 
        {
            color = interpolateColor = baseColor ?? this.baseColor;
        }

        public UIButton(string text) 
        {
            Text = text;
            color = interpolateColor = baseColor;
        }

        public override void OnUIEnter()
        {
            color = hoverColor;
            onEnter?.Invoke();
        }

        public override void OnUIExit()
        {
            color = baseColor;
            onExit?.Invoke();
        }

        public override void OnUIClick()
        {
            color = clickColor;
            onClick?.Invoke();
        }

        public override void OnUIHold()
        {
            color = clickColor;
            onHold?.Invoke();
        }

        public override void OnUIRelease()
        {
            color = hoverColor;
            onRelease?.Invoke();
        }

        public override void UIUpdate(Matrix? worldToScreenMatrix)
        {
            Vector2 cursor = InputSystem.mouseInfo.position;

            if (CursorWithinBounds(cursor, worldToScreenMatrix) && IsCloser())
                SelectedEntity = this;

            // TODO: Safe zone and auto grow?
            if (scaleToText)
            {

            }
            else
                textEntity?.FitToBounds(LocalScale);

            base.UIUpdate(worldToScreenMatrix ?? Matrix.Identity);
        }

        public override void Render(SpriteBatch batch, Entity parent)
        {
            interpolateColor = Color.Lerp(interpolateColor, color, colorLerpSpeed * Timing.unscaledDeltaTime);
            base.Render(batch, parent);
        }
    }
}
