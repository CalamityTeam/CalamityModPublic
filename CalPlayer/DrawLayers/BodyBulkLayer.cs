using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{

    //Extends the player's body sprite to draw over the head, useful for bulky chestplates such as the victide breastplate.
    public class BodyBulkLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow != 0f || drawInfo.drawPlayer.dead;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int bodyItem = drawPlayer.armor[1].type;
            if (drawPlayer.armor[11].type > ItemID.None)
                bodyItem = drawPlayer.armor[11].type;

            if (ModContent.GetModItem(bodyItem) is IBulkyArmor chestplateBulkDrawer)
            {
                int dyeShader = drawPlayer.dye?[0].dye ?? 0;

                // Remember to use drawInfo.position and not drawPlayer.position, or else it will not display properly in the player selection screen.
                Vector2 origin = new Vector2(drawPlayer.legFrame.Width * 0.5f, drawPlayer.legFrame.Height * 0.4f);
                Vector2 headDrawPosition = drawInfo.Center.Floor() - Main.screenPosition;

                //Account for the hellspawns known as mounts
                if (drawPlayer.mount.Active)
                    headDrawPosition.Y += drawPlayer.mount.HeightBoost;

                Texture2D extraPieceTexture = ModContent.Request<Texture2D>(chestplateBulkDrawer.BulkTexture).Value;
                Rectangle frame = extraPieceTexture.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);
                DrawData pieceDrawData = new DrawData(extraPieceTexture, headDrawPosition, frame, drawInfo.colorHead, drawPlayer.fullRotation, origin, 1f, drawInfo.playerEffect, 0)
                {
                    shader = dyeShader
                };
                drawInfo.DrawDataCache.Add(pieceDrawData);

            }
        }
    }
}
