using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Tiles.Astral;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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

        public override void RandomUpdate(int i, int j)
        {
            if (Main.rand.Next(7) <= 2)
            {
                Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, 156, Main.rand.NextFloat(-0.05f, 0.05f), Main.rand.NextFloat(-0.05f, -0.001f));
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.09f;
            g = 0.03f;
            b = 0.07f;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AstralDirt>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}
