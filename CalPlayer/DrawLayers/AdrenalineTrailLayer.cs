using System.Collections.Generic;
using CalamityMod.Items.VanillaArmorChanges;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class AdrenalineTrailLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.LastVanillaLayer);
        public int Time = 0;
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0f || drawPlayer.dead)
                return false;

            return drawPlayer.Calamity().AdrenalineTrail;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Time++;
            Player drawPlayer = drawInfo.drawPlayer;
            List<DrawData> existingDrawData = drawInfo.DrawDataCache;
            float movementSpeedInterpolant = CobaltArmorSetChange.CalculateMovementSpeedInterpolant(drawPlayer);
            for (float i = 0; i < drawPlayer.Calamity().OldPositions.Length; i += 0.9f)
            {
                float completionRatio = i / (float)drawPlayer.Calamity().OldPositions.Length;
                float scale = MathHelper.Lerp(1.1f, 0.1f, completionRatio);
                float opacity = MathHelper.Lerp(0.5f, 0.3f, completionRatio) * movementSpeedInterpolant;
                List<DrawData> afterimages = new List<DrawData>();
                for (int j = 0; j < existingDrawData.Count; j++)
                {
                    var drawData = existingDrawData[j];
                    Vector2 extraPosition = (Time % 6 == 0 ? new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f)) : Vector2.Zero);
                    drawData.position = existingDrawData[j].position - drawPlayer.position + drawPlayer.oldPosition + extraPosition;
                    drawData.color = new Color(0, 255, 201) * opacity;
                    drawData.scale = new Vector2(scale);
                    afterimages.Add(drawData);
                }
                drawInfo.DrawDataCache.InsertRange(0, afterimages);
            }
        }
    }
}
