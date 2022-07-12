using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{

    //Draws the front arm of the armor over the shoulderpad when walking. 
    public class FrontARmOverShoulderpadLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Item bodyItem = drawPlayer.armor[1];
            if (drawPlayer.armor[11].type > ItemID.None)
                bodyItem = drawPlayer.armor[11];

            if (ModContent.GetModItem(bodyItem.type) is IDrawArmOverShoulderpad frontArmDrawer)
            {
                string equipSlotName = frontArmDrawer.EquipSlotName(drawPlayer) != "" ? frontArmDrawer.EquipSlotName(drawPlayer) : bodyItem.ModItem.Name;
                int equipSlot = EquipLoader.GetEquipSlot(Mod, equipSlotName, EquipType.Body);
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
                Texture2D extraPieceTexture = ModContent.Request<Texture2D>(frontArmDrawer.FrontArmTexture).Value;

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
