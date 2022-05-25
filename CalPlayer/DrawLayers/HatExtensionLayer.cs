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

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead;

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

                    // It is imperative that drawPlayer is not used to get any position stuff, or else it will break in the character selection screen.
                    //We use the floor function to remove the jitter with the small float changes
                    Vector2 headDrawPosition = drawInfo.Position.Floor() - Main.screenPosition ;

                    //Account for the hellspawns known as mounts. But properly this time. HHHHAAAAAAAAAAAAAAAAAAAAAAAAA
                    headDrawPosition += Vector2.UnitY * drawInfo.mountOffSet;

                    //headDrawPosition += extendedHatDrawer.ExtensionSpriteOffset(drawInfo);

                    Texture2D extraPieceTexture = ModContent.Request<Texture2D>(extendedHatDrawer.ExtensionTexture).Value;

                    extraPieceTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Armor/WulfrumHeadgear_Head").Value;
                    Rectangle frame = extraPieceTexture.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);

                    Vector2 origin = frame.Size() / 2f;

                    DrawData pieceDrawData = new DrawData(extraPieceTexture, headDrawPosition, frame, drawInfo.colorArmorHead, drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect, 0)
                    {
                        shader = dyeShader
                    };
                    drawInfo.DrawDataCache.Add(pieceDrawData);
                }
            }
        }
    }
}
