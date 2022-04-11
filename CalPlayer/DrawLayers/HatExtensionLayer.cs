using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class HatExtensionLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow != 0f || drawInfo.drawPlayer.dead;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int headItemType = drawPlayer.armor[0].type;
            if (drawPlayer.armor[10].type > ItemID.None)
                headItemType = drawPlayer.armor[10].type;

            if (ModContent.GetModItem(headItemType) is IExtendedHat extendedHatDrawer)
            {
                if (extendedHatDrawer.PreDrawExtension(drawInfo) && !drawInfo.drawPlayer.dead)
                {
                    int dyeShader = drawPlayer.dye?[0].dye ?? 0;

                    // Remember to use drawInfo.position and not drawPlayer.position, or else it will not display properly in the player selection screen.
                    Vector2 origin = new Vector2(drawPlayer.legFrame.Width * 0.5f, drawPlayer.legFrame.Height * 0.4f);
                    Vector2 headDrawPosition = drawInfo.Center.Floor() - Main.screenPosition;

                    //Account for the hellspawns known as mounts
                    if (drawPlayer.mount.Active)
                        headDrawPosition.Y += drawPlayer.mount.HeightBoost;

                    headDrawPosition += extendedHatDrawer.ExtensionSpriteOffset(drawInfo);

                    Texture2D extraPieceTexture = ModContent.Request<Texture2D>(extendedHatDrawer.ExtensionTexture).Value;
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
}
