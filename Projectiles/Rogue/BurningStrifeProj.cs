using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BurningStrifeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Flame Spiky Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.timeLeft = 720;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 40;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            //Rotation code
            projectile.rotation += projectile.velocity.X * 0.05f * projectile.direction;
            //Gravity
            projectile.velocity.Y += 0.05f;
            if (projectile.velocity.Y > 16f)
                projectile.velocity.Y = 16f;
            //Dust
            if (projectile.ai[0] >= 25f)
            {
                Dust.NewDust(projectile.Center, 1, 1, DustID.Shadowflame, -projectile.velocity.X * 0.3f, -projectile.velocity.Y * 0.3f, 0, default, 1.1f);
                projectile.ai[0] = 0f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y * 0.7f;
            projectile.velocity.X *= 0.9f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
            if (projectile.Calamity().stealthStrike && projectile.penetrate != 1)
            {
                Main.PlaySound(SoundID.Item, projectile.Center, 103);
                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowflameExplosionBig>(), (int)(projectile.damage * 0.33), projectile.knockBack, projectile.owner);
                Main.projectile[proj].timeLeft += 20;
                Main.projectile[proj].Center = projectile.Center;
                Main.projectile[proj].Calamity().rogue = true;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
            if (projectile.Calamity().stealthStrike && projectile.penetrate != 1)
            {
                Main.PlaySound(SoundID.Item, projectile.Center, 103);
                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowflameExplosionBig>(), (int)(projectile.damage * 0.33), projectile.knockBack, projectile.owner);
                Main.projectile[proj].timeLeft += 20;
                Main.projectile[proj].Center = projectile.Center;
                Main.projectile[proj].Calamity().rogue = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            int proj;
            Main.PlaySound(SoundID.Item, projectile.Center, 103);
            if(projectile.Calamity().stealthStrike)
                proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowflameExplosionBig>(), (int)(projectile.damage * 0.33), projectile.knockBack, projectile.owner);
            else
                proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowflameExplosion>(), (int)(projectile.damage * 0.33), projectile.knockBack, projectile.owner);
            Main.projectile[proj].Center = projectile.Center;
            Main.projectile[proj].Calamity().rogue = true;
        }
    }
}
