using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
	public class MidnightSunUFO : ModProjectile
    {
        public const float DistanceToCheck = 2600f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Midnight Sun UFO");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 58;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 9;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.SkyBlue.ToVector3());
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.velocity.Y = Main.rand.NextFloat(8f, 11f) * Main.rand.NextBool(2).ToDirectionInt();
                projectile.velocity.Y = Main.rand.NextFloat(3f, 5f) * Main.rand.NextBool(2).ToDirectionInt();
                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<MidnightSunUFO>();
            player.AddBuff(ModContent.BuffType<MidnightSunBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.midnightUFO = false;
                }
                if (modPlayer.midnightUFO)
                {
                    projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            if (potentialTarget != null)
            {
                projectile.ai[0]++;
                if (projectile.ai[0] % 360f < 180f)
                {
                    projectile.rotation = projectile.rotation.AngleTowards(0f, 0.2f);
                    float angle = MathHelper.ToRadians(2f * projectile.ai[0] % 180f);
                    Vector2 destination = potentialTarget.Center - new Vector2((float)Math.Cos(angle) * potentialTarget.width * 0.65f, 250f);
                    projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(destination) * 24f, 0.03f);

                    if (projectile.ai[0] % 8f == 7f && potentialTarget.Top.Y > projectile.Bottom.Y)
                    {
                        Vector2 laserVelocity = projectile.SafeDirectionTo(potentialTarget.Center, Vector2.UnitY).RotatedByRandom(0.15f) * 25f;
                        Projectile.NewProjectile(projectile.Bottom, laserVelocity, ModContent.ProjectileType<MidnightSunLaser>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
					projectile.MinionAntiClump(0.35f);
                    projectile.ai[1] = 0f;
                }
                else
                {
                    // Move very, very quickly above the target.
                    Vector2 hoverDestination = potentialTarget.Top - Vector2.UnitY * 40f + (projectile.minionPos + projectile.ai[0] / 7f).ToRotationVector2() * 40f;
                    projectile.Center = Vector2.Lerp(projectile.Center, hoverDestination, 0.1f).MoveTowards(hoverDestination, 20f);
                    projectile.velocity = projectile.velocity.MoveTowards(Vector2.Zero, 4f);
                    projectile.ai[1] = Math.Abs(hoverDestination.Y - potentialTarget.Bottom.Y) + MathHelper.Lerp(30f, 50f, projectile.identity % 7f / 7f);

                    if (projectile.ai[0] % 360f == 240f)
                    {
                        if (Main.myPlayer == projectile.owner)
                        {
                            Main.PlaySound(SoundID.Item122, projectile.Center);
                            Vector2 laserVelocity = projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.UnitY);
                            Projectile.NewProjectile(projectile.Center, laserVelocity, ModContent.ProjectileType<MidnightSunBeam>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, projectile.whoAmI);
                        }
                    }
                }
            }
            else
            {
                projectile.velocity = (projectile.velocity * 15f + projectile.SafeDirectionTo(player.Center - new Vector2(player.direction * -80f, 160f)) * 19f) / 16f;

                Vector2 distanceVector = player.Center - projectile.Center;
                if (distanceVector.Length() > DistanceToCheck * 1.5f)
                {
                    projectile.Center = player.Center;
                    projectile.netUpdate = true;
                }

				projectile.MinionAntiClump(0.35f);
                projectile.rotation = projectile.velocity.X * 0.03f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
