using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ThePackMissile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 targetCenter = Projectile.Center;
            float minTargetDistance = 2500f;
            bool homeIn = false;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, n.Center, 1, 1))
                {
                    float distanceFromTarget = Projectile.Center.ManhattanDistance(n.Center);
                    if (distanceFromTarget < 200f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f, 12f);
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<ThePackMinissile>(), (int)(Projectile.damage * 0.25), Projectile.knockBack, Projectile.owner, 0f, 0f);
                            }
                        }
                        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                        Projectile.Kill();
                        return;
                    }
                    else if (distanceFromTarget < minTargetDistance)
                    {
                        minTargetDistance = distanceFromTarget;
                        targetCenter = n.Center;
                        homeIn = true;
                    }
                }
            }
            if (homeIn)
            {
                Projectile.velocity = (Projectile.velocity * 15f + Projectile.SafeDirectionTo(targetCenter) * 30f) / 16f;
                return;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 40; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CrystalPulse2, 0f, 0f, 0, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 60; j++)
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CrystalPulse2, 0f, 0f, 0, default, 2f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CrystalPulse2, 0f, 0f, 0, default, 1.5f);
                Main.dust[dust2].velocity *= 2f;
            }
            Projectile.Damage();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
