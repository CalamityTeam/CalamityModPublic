
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Astral;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralSand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSand[Type] = true;
            Main.tileBrick[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);
            CalamityUtils.MergeAstralTiles(Type);

            DustType = 108;
            ItemDrop = ModContent.ItemType<Items.Placeables.AstralSand>();

            AddMapEntry(new Color(187, 220, 237));

            TileID.Sets.TouchDamageSands[Type] = 15;
            TileID.Sets.Conversion.Sand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.Falling[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (j < Main.maxTilesY)
            {
                // tile[i, j+1] can still be null if it's on the edge of a chunk
                if (!Main.tile[i, j + 1].HasTile)
                {
                    Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j), new Vector2(i * 16f + 8f, j * 16f + 8f), Vector2.Zero, ModContent.ProjectileType<AstralFallingSand>(), 15, 0f);
                    WorldGen.SquareTileFrame(i, j);
                    return false;
                }
            }
            // CustomTileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AstralDirt>());
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AstralDirt>());
            return false;
        }

        public override bool HasWalkDust()
        {
            return Main.rand.Next(3) == 0;
        }

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            DustType = 108;
        }
    }
}
