using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class MidnightSunUFO : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const float DistanceToCheck = 2600f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.SentryShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 58;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3());
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.velocity.Y = Main.rand.NextFloat(8f, 11f) * Main.rand.NextBool().ToDirectionInt();
                Projectile.velocity.Y = Main.rand.NextFloat(3f, 5f) * Main.rand.NextBool().ToDirectionInt();

                // This AI variable doubles as the random frame on which this UFO chooses to shoot its machine gun.
                Projectile.localAI[0] = Main.rand.Next(1, (int)MidnightSunBeacon.MachineGunRate);
            }

            bool isProperProjectile = Projectile.type == ModContent.ProjectileType<MidnightSunUFO>();
            player.AddBuff(ModContent.BuffType<MidnightSunBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.midnightUFO = false;
                }
                if (modPlayer.midnightUFO)
                {
                    Projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = Projectile.Center.MinionHoming(DistanceToCheck, player);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            if (potentialTarget != null)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] % 360f < 180f)
                {
                    Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.2f);
                    float angle = MathHelper.ToRadians(2f * Projectile.ai[0] % 180f);
                    Vector2 destination = potentialTarget.Center - new Vector2((float)Math.Cos(angle) * potentialTarget.width * 0.65f, 250f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(destination) * 24f, 0.03f);

                    if (Projectile.ai[0] % MidnightSunBeacon.MachineGunRate == Projectile.localAI[0] && potentialTarget.Top.Y > Projectile.Bottom.Y)
                    {
                        Vector2 laserVelocity = Projectile.SafeDirectionTo(potentialTarget.Center, Vector2.UnitY).RotatedByRandom(0.15f) * 25f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Bottom, laserVelocity, ModContent.ProjectileType<MidnightSunShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    Projectile.MinionAntiClump(0.35f);
                    Projectile.ai[1] = 0f;
                }
                else
                {
                    // Move very, very quickly above the target.
                    Vector2 hoverDestination = potentialTarget.Top - Vector2.UnitY * 40f + (Projectile.minionPos + Projectile.ai[0] / 7f).ToRotationVector2() * 40f;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.1f).MoveTowards(hoverDestination, 20f);
                    Projectile.velocity = Projectile.velocity.MoveTowards(Vector2.Zero, 4f);
                    Projectile.ai[1] = Math.Abs(hoverDestination.Y - potentialTarget.Bottom.Y) + MathHelper.Lerp(30f, 50f, Projectile.identity % 7f / 7f);

                    if (Projectile.ai[0] % 360f == 240f)
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
                            Vector2 laserVelocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.UnitY);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, laserVelocity, ModContent.ProjectileType<MidnightSunBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Projectile.whoAmI);
                        }
                    }
                }
            }
            else
            {
                Projectile.velocity = (Projectile.velocity * 15f + Projectile.SafeDirectionTo(player.Center - new Vector2(player.direction * -80f, 160f)) * 19f) / 16f;

                Vector2 distanceVector = player.Center - Projectile.Center;
                if (distanceVector.Length() > DistanceToCheck * 1.5f)
                {
                    Projectile.Center = player.Center;
                    Projectile.netUpdate = true;
                }

                Projectile.MinionAntiClump(0.35f);
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
