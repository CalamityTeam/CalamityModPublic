using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.CraftingStations
{
    public class AshenAltar : ModTile
    {
        int animationFrame = 0;
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ashen Altar");
            AddMapEntry(new Color(191, 142, 111), name);
            AnimationFrameHeight = 54;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 6;
                animationFrame = frame;
                frameCounter = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 0.5f;
            b = 0.5f;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<Items.Placeables.Furniture.CraftingStations.AshenAltar>());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CalamityUtils.DrawStaticFlameEffect(ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/CraftingStations/AshenAltarFlame"), i, j, offsetY: animationFrame * animationFrameHeight);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX == 18 && tile.TileFrameY == 18)
            {
                CalamityUtils.DrawFlameSparks(60, 5, i, j);
            }
        }
    }
}
