using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Tiles.Astral;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;
using ReLogic.Content;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace CalamityMod.Tiles.Ores
{
    public class AuricOre : ModTile
    {
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AuricMine", 3);
        public static bool Animate;
        internal static Texture2D GlowTexture;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Ores/AuricOreGlow", AssetRequestMode.ImmediateLoad).Value;
            AnimationFrameHeight = 270;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 1000;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            DustType = 55;
            ItemDrop = ModContent.ItemType<Items.Placeables.Ores.AuricOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Auric Ore");
            AddMapEntry(new Color(255, 200, 0), name);
            MineResist = 10f;
            MinPick = 250;
            HitSound = MineSound;
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
            if (!Animate)
            {return;}
            r = 0.48f;
            g = 0.80f;
            b = 0.93f;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (!Animate)
            {return;}
                frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 7)
                {
                    Animate = false;
                    frame = 0;
                }
            }
        }
        /*public override void FloorVisuals(Player player)
        {
            //player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 2);
            player.AddBuff(BuffID.Electrified, 300);
            base.FloorVisuals(player);
            Animate = true;
            player.RemoveAllGrapplingHooks();
        }*/

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/Ores/AuricOreGlow").Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = GetDrawColour(i, j, new Color(225, 255, 255, 255));
            Tile trackTile = Main.tile[i, j];
            double num6 = Main.time * 0.08;
            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }
        private Color GetDrawColour(int i, int j, Color colour)
        {
            int colType = Main.tile[i, j].TileColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType >= 13 && colType <= 24)
            {
                colour.R = (byte)(paintCol.R / 255f * colour.R);
                colour.G = (byte)(paintCol.G / 255f * colour.G);
                colour.B = (byte)(paintCol.B / 255f * colour.B);
            }
            return colour;
        }
    }
}
