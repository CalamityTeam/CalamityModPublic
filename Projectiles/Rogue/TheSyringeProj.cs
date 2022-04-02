using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class TheSyringeProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TheSyringe";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Syringe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(8))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, Main.rand.Next(2) == 1 ? 107 : 89, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            projectile.damage += projectile.Calamity().defDamage / 200;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 100);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item107, projectile.position);
            for (int k = 0; k < 7; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 89, projectile.oldVelocity.X, projectile.oldVelocity.Y);
            }
            int fireAmt = Main.rand.Next(1, 3);
            if (projectile.owner == Main.myPlayer)
            {
                for (int f = 0; f < fireAmt; f++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<TheSyringeCinder>(), (int)(projectile.damage * 0.5), 0f, Main.myPlayer);
                }
                for (int s = 0; s < 2; ++s)
                {
                    float SpeedX = -projectile.velocity.X * Main.rand.NextFloat(0.4f, 0.7f) + Main.rand.NextFloat(-8f, 8f);
                    float SpeedY = -projectile.velocity.Y * Main.rand.NextFloat(0.4f, 0.7f) + Main.rand.NextFloat(-8f, 8f);
                    Projectile.NewProjectile(projectile.Center.X + SpeedX, projectile.Center.Y + SpeedY, SpeedX, SpeedY, ModContent.ProjectileType<TheSyringeS1>(), (int)(projectile.damage * 0.25), 0f, Main.myPlayer, Main.rand.Next(3), 0f);
                }
            }
            if (projectile.owner == Main.myPlayer && projectile.ai[1] == 1)
            {
                for (int b = 0; b < 5; b++)
                {
                    float speedX = Main.rand.NextFloat(-0.7f, 0.7f);
                    float speedY = Main.rand.NextFloat(-0.7f, 0.7f);
                    int bee = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speedX, speedY, ModContent.ProjectileType<PlaguenadeBee>(), (int)(projectile.damage * 0.5), 0f, Main.myPlayer);
                    Main.projectile[bee].penetrate = 1;
                }
            }
        }
    }
}
