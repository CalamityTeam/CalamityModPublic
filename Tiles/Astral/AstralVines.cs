using System;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralVines : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
			TileID.Sets.IsVine[Type] = true;
			TileID.Sets.ReplaceTileBreakDown[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.DrawFlipMode[Type] = 1;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

            DustType = ModContent.DustType<AstralBasic>();

            HitSound = SoundID.Grass;

            AddMapEntry(new Color(65, 56, 83));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            //GIVE VINE ROPE IF SPECIAL VINE BOOK
            if (WorldGen.genRand.NextBool() && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].cordage)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16 + 8f, j * 16 + 8f), ItemID.VineRope);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float brightness = 0.9f;
            brightness += 0.2f;
            brightness = MathHelper.Clamp(brightness, 0.5f, 0.9f);
            Color orange = new Color(237, 93, 83);
            Color cyan = new Color(66, 189, 181);
            Color value = Color.Lerp(orange, cyan, (MathF.Sin(j / 30f + Main.GameUpdateCount * 0.017f + -i / 40f) + 1f) / 2f);
            Color value1 = Color.Lerp(orange, cyan, (MathF.Sin((-j - 100) / 40f + Main.GameUpdateCount * 0.014f + i / 20f) + 1f) / 2f);
            r = (value.R + value1.R) / 600f;
            g = (value.G + value1.G) / 600f;
            b = (value.B + value1.B) / 600f;
            r *= brightness;
            g *= brightness;
            b *= brightness;
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
