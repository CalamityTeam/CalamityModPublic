using Terraria.ModLoader;
using Terraria.ID;
using static CalamityMod.CalPlayer.CalamityPlayer;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class ArtemisMask : ModItem, IExtendedHat
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artemis Mask");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }

        public override bool DrawHead() => false;

        public void DrawExtension(PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int dyeShader = drawPlayer.dye?[0].dye ?? 0;

            Vector2 origin = drawInfo.headOrigin;
            Vector2 headDrawPosition = drawInfo.position.Floor() + origin - Main.screenPosition;

            if (drawPlayer.mount.Active)
                headDrawPosition.Y += drawPlayer.mount.HeightBoost;

            headDrawPosition.X -= drawPlayer.direction == 1f ? 16f : 10f;
            headDrawPosition.Y += drawPlayer.gfxOffY - 10f;

            Texture2D extraPieceTexture = ModContent.GetTexture("CalamityMod/Items/Armor/Vanity/ArtemisMask_Extra");
            Rectangle frame = extraPieceTexture.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);
            DrawData pieceDrawData = new DrawData(extraPieceTexture, headDrawPosition, frame, drawInfo.upperArmorColor, drawPlayer.fullRotation, origin, 1f, drawInfo.spriteEffects, 0);
            pieceDrawData.shader = dyeShader;
            Main.playerDrawData.Add(pieceDrawData);
        }
    }
}
