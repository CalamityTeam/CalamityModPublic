using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class EncasementParryLayer : PlayerDrawLayer
    {
        public enum EncasementType
        {
            BlazingCore,
            FlameLickedShell
        }
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.FrontAccFront); //me when the player layer is called front acc front :skull:

        public EncasementType GetEncasementTypeFor(CalamityPlayer modPlayer) => modPlayer.blazingCore ? EncasementType.BlazingCore : EncasementType.FlameLickedShell;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            var encasement = GetEncasementTypeFor(modPlayer);
            bool visible = drawInfo.shadow == 0f && !drawPlayer.dead;
            visible = encasement switch
            {
                EncasementType.BlazingCore => visible && modPlayer.blazingCoreParry > 0,
                EncasementType.FlameLickedShell => visible && modPlayer.flameLickedShellParry > 0,
                _ => false
            };
            return visible;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            var calPlayer = drawPlayer.Calamity();
            var encasement = GetEncasementTypeFor(calPlayer);
            string tex = "CalamityMod/CalPlayer/DrawLayers/";
            int currentParry;
            float defaultOpacity;
            float scale;
            switch (encasement)
            {
                case EncasementType.BlazingCore:
                    tex += "BlazingCoreCrystal";
                    currentParry = calPlayer.blazingCoreParry;
                    defaultOpacity = 0.725f;
                    scale = 1.15f;
                    break;
                case EncasementType.FlameLickedShell:
                    tex += "PetrifiedRoseBud";
                    currentParry = calPlayer.flameLickedShellParry;
                    defaultOpacity = 0.875f;
                    scale = 1.2f;
                    break;
                default: //should never be this option
                    tex += "BlazingCoreCrystal";
                    currentParry = 0;
                    defaultOpacity = 0f;
                    scale = 0f;
                    break;
            }
            
            Texture2D texture = ModContent.Request<Texture2D>(tex).Value;
            Vector2 drawPos = drawInfo.Center - Main.screenPosition + new Vector2(0f, drawPlayer.gfxOffY);
            drawPos.Y += 15f;
            drawPos.X += 15f;
            
            
            int maxParry = 30; //all parries use thirty seconds as a max at this point in time, if this changes, this too should change
            float colorIntensity = currentParry >= 18 ? defaultOpacity : 1f - Utils.GetLerpValue(maxParry, 0f, currentParry, true);
            SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            drawInfo.DrawDataCache.Add(new DrawData(texture, drawPos, null, Color.White * colorIntensity, 0f, texture.Size() * 0.75f, scale, spriteEffects, 0));
        }
    }
}
