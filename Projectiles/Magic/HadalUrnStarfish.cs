using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class HadalUrnStarfish : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.timeLeft = 200;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 60)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 8)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                    Shards();
                    SoundEngine.PlaySound(SoundID.Item42, Projectile.position);
                    Projectile.ai[1]++;
                    Projectile.ai[0] = 0;
                }
            }
            else
            {
                Projectile.frame = 0;
            }
            if (Projectile.timeLeft <= 60)
            {
                Projectile.velocity *= 0.98f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] < 2) //Do not shoot more spikes if two rounds have already been shot
            Shards();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
        }

        public void Shards()
        {
            float variance = MathHelper.TwoPi / 5;
            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = new Vector2(0f, 10f);
                velocity = velocity.RotatedBy(variance * i + Projectile.rotation);
                int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ModContent.ProjectileType<HadalUrnStarfishShard>(), (int)(0.33f * Projectile.damage), 0, Projectile.owner);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Projectile.originalDamage;
            }
        }
    }
}
