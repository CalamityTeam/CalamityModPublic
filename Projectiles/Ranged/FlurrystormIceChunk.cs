using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlurrystormIceChunk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Chunk");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.coldDamage = true;
            projectile.penetrate = 1;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            //icicle dust
            if (Main.rand.NextBool(2))
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.1f);
                Main.dust[index2].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0f, 0f, 0, default, 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }
            int split = 0;
            while (split < 3)
            {
                //Calculate the velocity of the projectile
                float shardspeedX = -projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                float shardspeedY = -projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                //Prevents the projectile speed from being too low
                if (shardspeedX < 2f && shardspeedX > -2f)
                {
                    shardspeedX += -projectile.velocity.X;
                }
                if (shardspeedY > 2f && shardspeedY < 2f)
                {
                    shardspeedY += -projectile.velocity.Y;
                }

                //Spawn the projectile
                Projectile.NewProjectile(projectile.position.X + shardspeedX, projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<FlurrystormIceShard>(), (int)(projectile.damage * 0.3), 2f, projectile.owner);
                split += 1;
            }
        }
    }
}
