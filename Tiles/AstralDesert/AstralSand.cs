
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralSand : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSand[Type] = true;
            Main.tileBrick[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeDesertTiles(Type);
            TileMerge.MergeAstralTiles(Type);

            dustType = 108;
            drop = ModContent.ItemType<Items.AstralSand>();

            AddMapEntry(new Color(149, 156, 155));

            TileID.Sets.TouchDamageSands[Type] = 15;
            TileID.Sets.Conversion.Sand[Type] = true;

            SetModCactus(new AstralCactus());

            SetModPalmTree(new AstralPalmTree());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (j < Main.maxTilesY && !Main.tile[i, j + 1].active())
            {
                Main.tile[i, j].active(false);
                Projectile.NewProjectile(new Vector2(i * 16f + 8f, j * 16f + 8f), Vector2.Zero, ModContent.ProjectileType<AstralFallingSand>(), 15, 0f);
                WorldGen.SquareTileFrame(i, j);
                return false;
            }
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, ModContent.TileType<AstralDirt>());
            return false;
        }

        public override bool HasWalkDust()
        {
            return Main.rand.Next(3) == 0;
        }

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = 108;
        }

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AstralPalmSapling>();
        }
    }

    public class AstralCactus : ModCactus
    {
        public override Texture2D GetTexture()
        {
            return CalamityMod.AstralCactusTexture;
        }
    }
}
