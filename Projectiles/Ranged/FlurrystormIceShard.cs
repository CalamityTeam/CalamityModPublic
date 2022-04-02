using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlurrystormIceShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Shard");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 10;
            projectile.height = 10;
            projectile.ranged = true;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            //Rotation and gravity
            projectile.rotation += 0.6f * projectile.direction;
            projectile.velocity.Y = projectile.velocity.Y + 0.27f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.10f, 150, default, 0.9f);
                splash += 1;
            }
        }
    }
}
