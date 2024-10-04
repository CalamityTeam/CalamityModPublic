using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class GiantPlanteraBulb : ModTile
    {
        public static FramedGlowMask GlowMask { get; private set; }

        public override void Load()
        {
            GlowMask = new($"{Texture}Glow", 18, 18);
        }

        public override void SetStaticDefaults()
        {
            // Tile can provide light
            Main.tileLighted[Type] = true;

            Main.tileFrameImportant[Type] = true;

            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

            // Various data sets to protect this tile from premature death
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            // CalamityGlobalTile.PreventsAnchorTileChanges.Add(Type);

            // Object data
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.Origin = new Point16(2, 4);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            // Adds two map entries for the bulb, first is for Pre-Hardmode, and the latter is for Hardmode.
            AddMapEntry(new Color(107, 125, 33), CalamityUtils.GetText("Tiles.GiantBulb"));
            AddMapEntry(new Color(243, 82, 171), CalamityUtils.GetText("Tiles.GiantBulb"));

            AnimationFrameHeight = 90;
            MineResist = 3f;

            DustType = DustID.PlanteraBulb;
            HitSound = SoundID.Grass;
        }

        // Use the second map entry in Hardmode
        public override ushort GetMapOption(int i, int j)
        {
            return (ushort)(Main.hardMode ? 1 : 0);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = (!WorldGen.genRand.NextBool(3) && Main.hardMode) ? DustID.Plantera_Pink : DustID.Plantera_Green;
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return Main.hardMode;
        }

        public override bool CanExplode(int i, int j)
        {
            return Main.hardMode;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            float x = i * 16;
            float y = j * 16;
            float distanceFromPlayer = -1f;
            int player = 0;
            for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
            {
                float dist = Math.Abs(Main.player[playerIndex].position.X - x) + Math.Abs(Main.player[playerIndex].position.Y - y);
                if (dist < distanceFromPlayer || distanceFromPlayer == -1f)
                {
                    player = playerIndex;
                    distanceFromPlayer = dist;
                }
            }

            // Spawn Plantera if the bulb was broken within a distance of 50 tiles or less, otherwise leave
            if (distanceFromPlayer / 16f >= 50f)
            {
                return;
            }

            float projectileVelocity = 6f;
            int projType = ProjectileID.SporeCloud;
            int npcType = NPCID.Spore;
            Vector2 spawn = new Vector2((i + 2) * 16 + 8, (j + 4) * 16 + 8);
            Vector2 dustSpawn = new Vector2((i + 2) * 16, (j + 4) * 16);
            SoundEngine.PlaySound(SoundID.Item74, spawn);
            Vector2 destination = new Vector2((i + 2) * 16 + 8, j * 16 + 8) - spawn;
            destination.Normalize();
            destination *= projectileVelocity;
            int numProj = 30;
            int numNPCs = 10;
            float rotation = MathHelper.ToRadians(100);

            // TODO: Projs might be buggy in multiplayer?
            for (int projIndex = 0; projIndex < numProj; projIndex++)
            {
                Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, projIndex / (float)(numProj - 1))) * (Main.rand.NextFloat() + 0.25f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j), spawn, perturbedSpeed, projType, 0, 0f, Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16));

                perturbedSpeed *= 2f;
                Dust dust = Dust.NewDustDirect(dustSpawn, 16, 16, DustID.JungleSpore, perturbedSpeed.X, perturbedSpeed.Y, 250);
                dust.fadeIn = 0.7f;
                Dust.NewDustDirect(dustSpawn, 16, 16, (!WorldGen.genRand.NextBool(3) && Main.hardMode) ? DustID.Plantera_Pink : DustID.Plantera_Green, perturbedSpeed.X, perturbedSpeed.Y);
            }

            for (int npcIndex = 0; npcIndex < numNPCs; npcIndex++)
            {
                Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, npcIndex / (float)(numNPCs - 1))) * (Main.rand.NextFloat() + 0.5f) * 0.5f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int spore = NPC.NewNPC(new EntitySource_TileBreak(i, j), (int)spawn.X, (int)spawn.Y, npcType, 0, -1f);
                    Main.npc[spore].velocity.X = perturbedSpeed.X;
                    Main.npc[spore].velocity.Y = perturbedSpeed.Y;
                    Main.npc[spore].netUpdate = true;
                }

                perturbedSpeed *= 2f;
                Dust dust = Dust.NewDustDirect(dustSpawn, 16, 16, DustID.JungleSpore, perturbedSpeed.X, perturbedSpeed.Y, 250);
                dust.fadeIn = 0.7f;
                Dust.NewDustDirect(dustSpawn, 16, 16, DustID.Plantera_Pink, perturbedSpeed.X, perturbedSpeed.Y);
            }

            // Automatically handles multiplayer
            NPC.SpawnOnPlayer(player, NPCID.Plantera);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            // Set frame to 0 and leave if the world is not in hardmode.
            if (!Main.hardMode)
            {
                frame = 0;
                return;
            }

            // Otherwise do typical frame logic
            frameCounter++;
            if (frameCounter > 25)
            {
                frameCounter = 0;
                frame++;
                if (frame > 6)
                {
                    frame = 1;
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.hardMode)
            {
                r = 0.486f;
                g = 0.164f;
                b = 0.342f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];

            var xPos = tile.TileFrameX;
            var yPos = tile.TileFrameY + AnimationFrameHeight * Main.tileFrame[Type];

            if (GlowMask.HasContentInFramePos(xPos, yPos))
            {
                spriteBatch.Draw(
                    GlowMask.Texture,
                    new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + CalamityUtils.TileDrawOffset,
                    new Rectangle(xPos, yPos, 16, 16),
                    Color.Yellow
                );
            }
        }
    }
}
