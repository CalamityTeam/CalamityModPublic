using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LuminousStrikerProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/LuminousStriker";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Luminous Striker");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.localNPCHitCooldown = 13;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f && Projectile.Calamity().stealthStrike)
            {
                Projectile.timeLeft = 600;
                Projectile.ai[0] = 1f;
            }

            if (Main.rand.NextBool(4))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 176, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver4;

            if (Projectile.timeLeft % 4 == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Projectile.Calamity().stealthStrike)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + Main.rand.NextFloat(-15f, 15f), Projectile.Center.Y + Main.rand.NextFloat(-15f, 15f), Projectile.velocity.X, Projectile.velocity.Y, ModContent.ProjectileType<LuminousShard>(), (int)(Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Projectile.owner);
                    }
                    else
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * 0f, -2f, ModContent.ProjectileType<LuminousShard>(), (int)(Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                    while (speed.X == 0f && speed.Y == 0f)
                    {
                        speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                    }
                    speed.Normalize();
                    speed *= Main.rand.Next(30, 61) * 0.1f * 2f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<LuminousShard>(), (int)(Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Projectile.owner);
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                    while (speed.X == 0f && speed.Y == 0f)
                    {
                        speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                    }
                    speed.Normalize();
                    speed *= Main.rand.Next(30, 61) * 0.1f * 2f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<LuminousShard>(), (int)(Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Projectile.owner);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 176, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
