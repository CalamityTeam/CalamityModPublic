using System.Collections.Generic;
using CalamityMod.Items.VanillaArmorChanges;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class AscendantTrailLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.LastVanillaLayer);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0f || drawPlayer.dead || drawPlayer.Calamity().AdrenalineTrail)
                return false;

            return drawPlayer.Calamity().ascendantTrail;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            List<DrawData> existingDrawData = drawInfo.DrawDataCache;
            float movementSpeedInterpolant = CobaltArmorSetChange.CalculateMovementSpeedInterpolant(drawPlayer);
            for (float i = 0f; i < drawPlayer.Calamity().OldPositions.Length; i += 1.2f)
            {
                float completionRatio = i / (float)drawPlayer.Calamity().OldPositions.Length;
                float scale = MathHelper.Lerp(1f, 0.7f, completionRatio);
                float opacity = MathHelper.Lerp(0.18f, 0.06f, completionRatio) * movementSpeedInterpolant;
                List<DrawData> afterimages = new List<DrawData>();
                for (int j = 0; j < existingDrawData.Count; j++)
                {
                    var drawData = existingDrawData[j];
                    drawData.position = existingDrawData[j].position - drawPlayer.position + drawPlayer.oldPosition;
                    drawData.color = new Color(155, 228, 232) * opacity;
                    drawData.scale = new Vector2(scale);
                    afterimages.Add(drawData);
                }
                drawInfo.DrawDataCache.InsertRange(0, afterimages);
            }
        }
    }
}
