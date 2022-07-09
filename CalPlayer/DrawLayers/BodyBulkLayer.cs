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

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            //While the extra bulk draws over the players head position, we don't want to have it drawn when the head alone is being drawn.
            if (drawInfo.headOnlyRender)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            int bodyItem = drawPlayer.armor[1].type;
            if (drawPlayer.armor[11].type > ItemID.None)
                bodyItem = drawPlayer.armor[11].type;


            if (ModContent.GetModItem(bodyItem) is IBulkyArmor chestplateBulkDrawer)
            {
                int equipSlot = chestplateBulkDrawer.BodySlot(drawPlayer);
                if (equipSlot == -1)
                    equipSlot = EquipLoader.GetEquipSlot(Mod, ModContent.GetModItem(bodyItem).Name, EquipType.Body);

                if (drawPlayer.body != equipSlot)
                    return;


                int dyeShader = drawPlayer.dye?[1].dye ?? 0;
                Vector2 drawPosition = drawInfo.Position - Main.screenPosition;

                // Using drawPlayer to get width & height and such is perfectly fine, on the other hand. Just center everything
                drawPosition += new Vector2((drawPlayer.width - drawPlayer.bodyFrame.Width) / 2f, drawPlayer.height - drawPlayer.bodyFrame.Height + 4f);

                //Convert to int to remove the jitter.
                drawPosition = new Vector2((int)drawPosition.X, (int)drawPosition.Y);

                //Some dispalcements
                drawPosition += drawPlayer.bodyPosition + drawInfo.bodyVect;

                //Grab the extension texture
                Texture2D extraPieceTexture = ModContent.Request<Texture2D>(chestplateBulkDrawer.BulkTexture).Value;

                //Get the frame of the extension based on the players body frame
                Rectangle frame = extraPieceTexture.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);

                DrawData pieceDrawData = new DrawData(extraPieceTexture, drawPosition, frame, drawInfo.colorArmorBody, drawPlayer.fullRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0)
                {
                    shader = dyeShader
                };
                drawInfo.DrawDataCache.Add(pieceDrawData);

            }
        }
    }
}
