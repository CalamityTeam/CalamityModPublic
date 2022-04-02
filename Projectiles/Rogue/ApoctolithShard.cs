using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class ApoctolithShard : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/AbyssalMirrorProjectile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apoctolith Shard");
            Main.projFrames[projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.width = 13;
            projectile.height = 13;
            projectile.Calamity().rogue = true;
        }
        public override void AI()
        {
            //Rotation and gravity
            projectile.rotation += 0.6f * projectile.direction;
            projectile.velocity.Y += 0.27f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 67, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.10f, 150, default, 0.9f);
                splash += 1;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }
    }
}
