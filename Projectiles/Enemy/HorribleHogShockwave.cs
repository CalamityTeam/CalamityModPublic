using System;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class HorribleHogShockwave : ModProjectile, ILocalizedModType
    {
        public ref float Timer => ref Projectile.ai[0];

        public ref float HeightMultiplier => ref Projectile.ai[1];

        public ref float HogIndex => ref Projectile.ai[2];

        public float CurrentHeight => 32f * MathHelper.Lerp(1f, HeightMultiplier, Timer / 9f);

        public new string LocalizationCategory => "Projectiles.Enemy";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center - Vector2.UnitY * CurrentHeight * Projectile.scale, Projectile.width * Projectile.scale, ref _);
        }

        public override bool? CanHitNPC(NPC target) => target.whoAmI != (int)HogIndex;

        // Player knockback is constant and applies whenever any damage is dealt, regardless of if a projectile spawned deals zero knockback or not.
        // Therefore, the knockback stat modifier here needs to be multiplied by zero in order for any custom knockback effect to take place.
        // This isn't the same for NPCs, so they don't need it so long as the projectile is spawned with zero knockback.
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.Knockback *= 0f;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 5f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.velocity.Y -= MathHelper.Clamp(8f + (CurrentHeight * 0.1f), 8f, 16f);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.velocity.Y -= MathHelper.Clamp(8f + (CurrentHeight * 0.1f) * target.knockBackResist, 8f, 16f);

        public override void AI()
        {
            if (Timer >= 9f)
            {
                Projectile.Kill();
                return;
            }

            if (Timer == 1f)
                SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);

            Projectile.velocity = Vector2.Zero;

            Point topLeft = Projectile.TopLeft.ToTileCoordinates();
            Point bottomRight = Projectile.BottomRight.ToTileCoordinates();
            int halfWidth = Projectile.width / 2;
            int dustSpawnInterval = (int)Projectile.ai[0] / 3;

            Timer++;
            if (Timer % 3 != 0f)
                return;

            float heightMulti = MathHelper.Lerp(1f, HeightMultiplier, Timer / 9f);
            int dustCloudAmt = Main.rand.Next(4, 7);
            for (int i = 0; i < dustCloudAmt; i++)
            {
                Vector2 dustCloudPosition = Projectile.Bottom + Main.rand.NextVector2Circular(24f, 0f);
                Vector2 dustVelocity = Vector2.UnitY * -1.3f * heightMulti * i * Main.rand.NextFloat(0.8f, 1.2f);
                Color dustCloudColor = Color.Lerp(Color.SandyBrown, Color.SandyBrown, Main.rand.NextFloat());
                float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();
                TimedSmokeParticle dustCloud = new(dustCloudPosition, dustVelocity, dustCloudColor, dustCloudColor, Main.rand.NextFloat(0.7f, 0.8f), Main.rand.NextFloat(0.72f, 0.84f), Main.rand.Next(30, 45), rotationSpeed);
                GeneralParticleHandler.SpawnParticle(dustCloud, true, Enums.GeneralDrawLayer.BeforeSolidTiles);
            }

            for (int i = topLeft.X; i <= bottomRight.X; i++)
            {
                for (int j = topLeft.Y; j <= bottomRight.Y; j++)
                {
                    if (Vector2.Distance(Projectile.Center, new Vector2(i * 16f, j * 16f)) > halfWidth)
                        continue;

                    Tile tile = Framing.GetTileSafely(i, j);
                    if (!tile.HasTile || !Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] || Main.tileFrameImportant[tile.TileType])
                        continue;

                    Tile tileAbove = Framing.GetTileSafely(i, j - 1);
                    if (tileAbove.HasTile && Main.tileSolid[tileAbove.TileType] && !Main.tileSolidTop[tileAbove.TileType])
                        continue;

                    int dustCountFromTile = WorldGen.KillTile_GetTileDustAmount(fail: true, tile, i, j);
                    for (int k = 0; k < dustCountFromTile; k++)
                    {
                        Dust dust = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tile)];
                        dust.velocity.Y -= 3f + dustSpawnInterval * 1.5f * HeightMultiplier;
                        dust.velocity.Y *= Main.rand.NextFloat();
                        dust.velocity.Y *= 0.75f;
                        dust.scale += dustSpawnInterval * 0.03f;
                    }

                    if (dustSpawnInterval >= 2)
                    {
                        for (int m = 0; m < dustCountFromTile - 1; m++)
                        {
                            Dust dust = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tile)];
                            dust.velocity.Y -= 1f + dustSpawnInterval * HeightMultiplier;
                            dust.velocity.Y *= Main.rand.NextFloat();
                            dust.velocity.Y *= 0.75f;
                        }
                    }
                }
            }
        }
    }
}
