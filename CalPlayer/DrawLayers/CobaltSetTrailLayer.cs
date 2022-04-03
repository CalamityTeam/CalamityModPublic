using System.Collections.Generic;
using CalamityMod.Items.VanillaArmorChanges;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class CobaltSetTrailLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.EyebrellaCloud);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0f || drawPlayer.dead)
                return false;

            return drawPlayer.Calamity().CobaltSet;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            List<DrawData> existingDrawData = drawInfo.DrawDataCache;
            for (int i = 0; i < drawPlayer.Calamity().OldPositions.Length; i++)
            {
                float completionRatio = i / (float)drawPlayer.Calamity().OldPositions.Length;
                float scale = MathHelper.Lerp(1f, 0.5f, completionRatio);
                float opacity = MathHelper.Lerp(0.23f, 0.07f, completionRatio) * CobaltArmorSetChange.CalculateMovementSpeedInterpolant(drawPlayer);
                List<DrawData> afterimages = new List<DrawData>();
                for (int j = 0; j < existingDrawData.Count; j++)
                {
                    var drawData = existingDrawData[j];
                    drawData.position = existingDrawData[j].position - drawPlayer.position + drawPlayer.oldPosition;
                    drawData.color = Color.Cyan * opacity;
                    drawData.color.G = (byte)(drawData.color.G * 0.87);
                    drawData.color.B = (byte)(drawData.color.B * 1.24);
                    drawData.scale = new Vector2(scale);
                    afterimages.Add(drawData);
                }
                drawInfo.DrawDataCache.InsertRange(0, afterimages);
            }
        }
    }
}
