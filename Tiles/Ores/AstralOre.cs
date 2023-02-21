using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Tiles.Astral;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Ores
{
    public class AstralOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 900;
            Main.tileSpelunker[Type] = true;
            Main.tileShine2[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeAstralTiles(Type);

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            MinPick = 210;
            DustType = 173;
            ItemDrop = ModContent.ItemType<Items.Placeables.Ores.AstralOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Astral Ore");
            AddMapEntry(new Color(255, 153, 255), name);
            MineResist = 5f;
            HitSound = SoundID.Tink;

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.DoesntGetReplacedWithTileReplacement[Type] = true;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Main.tile[i, j];
            Tile up = Main.tile[i, j - 1];
            Tile up2 = Main.tile[i, j - 2];
            if (closer && Main.rand.NextBool(60) && !up.HasTile && !up2.HasTile && j < Main.worldSurface)
            {
                //Cyan Cinders below
                Dust dust;
                dust = Main.dust[Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, ModContent.DustType<AstralBlue>(), -0.4651165f, 0f, 17, new Color(0, 255, 244), 1.5f)];

                //Orange Cinders below
                dust = Main.dust[Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, ModContent.DustType<AstralOrange>(), -0.4651165f, 0f, 17, new Color(255, 255, 255), 1.5f)];

            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return DownedBossSystem.downedAstrumDeus;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float brightness = 0.7f;
            brightness *= (float)MathF.Sin(-j / 40f + Main.GameUpdateCount * 0.007f);
            brightness *= (float)MathF.Sin(-i / 40f + Main.GameUpdateCount * 0.005f);
            brightness *= (float)MathF.Sin(-j / 40f + Main.GameUpdateCount * 0.006f);
            brightness *= (float)MathF.Sin(-i / 40f + Main.GameUpdateCount * 0.009f);
            brightness += 0.5f;
            r = Main.DiscoR / 255f * 0.5f;
            g = 0.5f;
            b = (255 - Main.DiscoR) / 255f * 0.5f;
            r *= brightness;
            g *= brightness;
            b *= brightness;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AstralDirt>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}
