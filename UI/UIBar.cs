using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;
using Terraria.UI;

namespace CalamityMod.UI
{
    public class UIBar : UIState
    {
        public UIElement backPanel, barPanel; //the 'panels' of each part of the UI.
        public int barWidth = 150; //the exact width of the bar texture/panel.
        public Func<int> getValue; //the func used to get the value displayed.
        public int valueMax = 1, barOffset = 0; //the maximum value of the value displayed :: the offset used to draw the bar inwards.

        public UIBar(Texture2D imageBack, Texture2D imageBar, int barOff) : this(imageBack, imageBar, barOff, 10000, GetTickedValue) //uses textures and the test ticker for drawing
        {
        }

        public UIBar(Texture2D imageBack, Texture2D imageBar, int barOff, int valMax, Func<int> gValue) : this(valMax, gValue) //uses textures
        {
            backPanel = new UIImage(imageBack);
            barOffset = barOff;
            barPanel = new UIImage(imageBar);
            barWidth = (int)barPanel.Width.Pixels;
        }

        public UIBar() : this(10000, GetTickedValue) //for testing purposes
        {
        }

        public UIBar(int valMax, Func<int> gValue) : base() //uses panels for drawing instead of textures
        {
            valueMax = valMax;
            getValue = gValue;
        }

        public static int tick; //for testing
        public static int GetTickedValue() //ditto
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            tick = Main.player[Main.myPlayer].Calamity().stress;
            return tick;
        }

        public override void OnInitialize()
        {
            float posX = (float)Config.RageMeterXPos, posY = (float)Config.RageMeterYPos; //CHANGE THESE TWO TO CHANGE WHERE IT STARTS ON SCREEN!
            if (backPanel == null) //if not using textures set up panels
            {
                backPanel = new UIPanel();
                ((UIPanel)backPanel).SetPadding(0);
                backPanel.Left.Set(posX, 0f);
                backPanel.Top.Set(posY, 0f);
                backPanel.Width.Set(barWidth + 20f, 0f);
                backPanel.Height.Set(50f, 0f);
                ((UIPanel)backPanel).BackgroundColor = new Color(73, 94, 171);

                backPanel.OnMouseDown += new UIElement.MouseEvent(DragStart);
                backPanel.OnMouseUp += new UIElement.MouseEvent(DragEnd);

                barPanel = new UIPanel();
                ((UIPanel)barPanel).SetPadding(0);
                barPanel.Left.Set(10f, 0f);
                barPanel.Top.Set(10f, 0f);
                barPanel.Width.Set(barWidth, 0f);
                barPanel.Height.Set(30f, 0f);
                ((UIPanel)barPanel).BackgroundColor = new Color(200, 0, 0);
                backPanel.Append(barPanel);
            }
            else //otherwise using images so just move it into position
            {
                backPanel.Left.Set(posX, 0f);
                backPanel.Top.Set(posY, 0f);
                backPanel.OnMouseDown += new UIElement.MouseEvent(DragStart);
                backPanel.OnMouseUp += new UIElement.MouseEvent(DragEnd);

                barPanel.Left.Set(barOffset, 0f);
                barPanel.Top.Set(0f, 0f);

                backPanel.Append(barPanel);
            }

            base.Append(backPanel);
        }

        public float GetPercentile()
        {
            return (float)getValue() / Math.Max(1, (float)valueMax - 1);
        }

        Vector2 offset;
        public bool dragging = false;
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            offset = new Vector2(evt.MousePosition.X - backPanel.Left.Pixels, evt.MousePosition.Y - backPanel.Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            Vector2 end = evt.MousePosition;
            dragging = false;

            backPanel.Left.Set(end.X - offset.X, 0f);
            backPanel.Top.Set(end.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            base.Update(gameTime);
            Recalculate(); //THIS IS IMPORTANT! IDK why but when this is included it updates the drawing every tick.
            tick = Main.player[Main.myPlayer].Calamity().stress; //updates the testing tick
            if (tick >= 10000)
            {
                tick = 10000;
            }
            barPanel.Width.Set(GetPercentile() * barWidth, 0f); //set the bar's width to the given percentile.
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (backPanel.ContainsPoint(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText("Rage: " + getValue() + "/" + valueMax + "", 0, 0, -1, -1, -1, -1); //only way I got this to show up consistently, otherwise it fucked up and showed up anywhere onscreen lol.
            }
            if (dragging)
            {
                backPanel.Left.Set(MousePosition.X - offset.X, 0f);
                backPanel.Top.Set(MousePosition.Y - offset.Y, 0f);
                Recalculate();
            }
        }
    }
}
