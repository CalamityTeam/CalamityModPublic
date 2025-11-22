using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Items.Weapons.Summon.HarvestStaff;

namespace CalamityMod.Projectiles.Summon
{
    public class HarvestStaffSentry : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public ref float Timer => ref Projectile.ai[0];
        public static float Gravity = 0.8f;
        public static float MaxGravity = 20f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.tileCollide = true;
            Projectile.width = 68;
            Projectile.height = 32;
        }

        public override void AI()
        {
            if (PumpkinAmount() < PumpkinsPerSentry * SentryAmount())
                Timer++;

            if (Timer > TimePerPumpkin)
            {
                float randomOffset = 160f;
                Vector2 spawnPosition = Projectile.Center + new Vector2(Main.rand.NextFloat(-randomOffset, randomOffset), -80f);
                MakeSpawnValid(ref spawnPosition);

                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    Vector2.Zero,
                    ModContent.ProjectileType<HarvestStaffMinion>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    Main.rand.Next(3));

                Timer = 0f;

                Projectile.netUpdate = true;
            }

            float speed = Projectile.velocity.Y;
            if (speed < MaxGravity)
                speed = MathF.Min(speed + Gravity, MaxGravity);
            Projectile.velocity.Y = speed;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 10)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
        }

        #region AI Methods

        public int PumpkinAmount()
        {
            int amount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj == null || !proj.active || proj.type != ModContent.ProjectileType<HarvestStaffMinion>() || proj.owner != Projectile.owner)
                    continue;
                amount++;
            }

            return amount;
        }

        public int SentryAmount()
        {
            int amount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj == null || !proj.active || proj.type != ModContent.ProjectileType<HarvestStaffSentry>() || proj.owner != Projectile.owner)
                    continue;
                amount++;
            }

            return amount;
        }

        public void MakeSpawnValid(ref Vector2 spawnpoint)
        {
            Point spawnpointTileCoords = spawnpoint.ToSafeTileCoordinates();
            Point sentryTileCoords = Projectile.Top.ToSafeTileCoordinates();

            if (!Main.tile[spawnpointTileCoords].IsTileSolid())
                return;
            else
            {
                while (Main.tile[spawnpoint.ToSafeTileCoordinates()].IsTileSolid())
                {
                    for (int coordY = spawnpointTileCoords.Y; coordY < sentryTileCoords.Y; coordY++)
                    {
                        Point tileCoords = new Point(spawnpointTileCoords.X, coordY);
                        if (!Main.tile[tileCoords].IsTileSolid())
                        {
                            spawnpoint = tileCoords.ToVector2();
                            break;
                        }
                    }

                    spawnpoint.X += Main.rand.NextFloat(-160f, 160f);
                }
            }
        }

        #endregion

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Color drawColor = Projectile.GetAlpha(lightColor);
            Vector2 rotationPoint = frame.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, drawPosition, frame, drawColor, Projectile.rotation, rotationPoint, Projectile.scale, SpriteEffects.None);

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindProjectiles.Add(index);
    }
}
