using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Tiles.Astral;
using Microsoft.Xna.Framework;
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
        internal static Texture2D GlowTexture;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Ores/AstralOreGlow", AssetRequestMode.ImmediateLoad).Value;
            }
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

        public override void FloorVisuals(Player player)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 2);
            base.FloorVisuals(player);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AstralDirt>(), false, false, false, false, resetFrame);
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // If the cached textures don't exist for some reason, don't bother using them.
            if (GlowTexture is null)
                return;

            Tile tile = CalamityUtils.ParanoidTileRetrieval(i, j);
            int xPos = tile.TileFrameX;
            int frameOffset = (i + j) % 2 * AnimationFrameHeight;
            int yPos = tile.TileFrameY + frameOffset;
            Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + drawOffset;

            if (!tile.IsHalfBlock && tile.Slope == 0)
            {
                spriteBatch.Draw(GlowTexture, drawPosition, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else if (tile.IsHalfBlock)
            {
                spriteBatch.Draw(GlowTexture, drawPosition + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
