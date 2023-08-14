using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ModLoader;
namespace CalamityMod.Tiles.FurnitureWulfrum
{
    public class WulfrumSofa : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpSofa(ModContent.ItemType<Items.Placeables.FurnitureWulfrum.WulfrumSofa>());

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 107, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) => CalamityUtils.BenchSitInfo(i, j, ref info);

        public override bool RightClick(int i, int j) => CalamityUtils.ChairRightClick(i, j);

        public override void MouseOver(int i, int j) => CalamityUtils.BenchMouseOver(i, j, ModContent.ItemType<Items.Placeables.FurnitureWulfrum.WulfrumSofa>());

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xFrameOffset = Main.tile[i, j].TileFrameX;
            int yFrameOffset = Main.tile[i, j].TileFrameY;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureWulfrum/WulfrumSofaGlow").Value;
            Vector2 drawOffest = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + drawOffest;
            Color drawColour = Color.White;
            Tile trackTile = Main.tile[i, j];
            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
                spriteBatch.Draw(glowmask, drawPosition, new Rectangle(xFrameOffset, yFrameOffset, 18, 18), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (trackTile.IsHalfBlock)
                spriteBatch.Draw(glowmask, drawPosition + new Vector2(0f, 8f), new Rectangle(xFrameOffset, yFrameOffset, 18, 8), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }
    }
}
