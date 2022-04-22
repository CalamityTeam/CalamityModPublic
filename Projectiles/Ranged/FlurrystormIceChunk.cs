using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using Terraria.Audio;

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
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.coldDamage = true;
            Projectile.penetrate = 1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            //icicle dust
            if (Main.rand.NextBool(2))
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 68, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.1f);
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
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 68, 0f, 0f, 0, default, 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }
            int split = 0;
            while (split < 3)
            {
                //Calculate the velocity of the projectile
                float shardspeedX = -Projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                float shardspeedY = -Projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                //Prevents the projectile speed from being too low
                if (shardspeedX < 2f && shardspeedX > -2f)
                {
                    shardspeedX += -Projectile.velocity.X;
                }
                if (shardspeedY > 2f && shardspeedY < 2f)
                {
                    shardspeedY += -Projectile.velocity.Y;
                }

                //Spawn the projectile
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + shardspeedX, Projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<FlurrystormIceShard>(), (int)(Projectile.damage * 0.3), 2f, Projectile.owner);
                split += 1;
            }
        }
    }
}
