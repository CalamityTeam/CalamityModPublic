using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
namespace CalamityMod.Projectiles.Damageable
{
    public class ArtifactOfResilienceBulwark : DamageableProjectile
    {
        public int Reformations = 0;
        public const int MaxReformations = 3;
        public override int LifeMax => 1;
        public override int MaxDamageImmunityFrames => 5;
        public override DamageSourceType DamageSources => DamageSourceType.FriendlyProjectiles | DamageSourceType.HostileProjectiles | DamageSourceType.HostileNPCs;
        public override LegacySoundStyle HitSound => SoundID.NPCHit3;
        public override LegacySoundStyle DeathSound => SoundID.NPCDeath14;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artifact of Resilience");
        }

        public override void SafeSetDefaults()
        {
            projectile.width = projectile.height = 76;
            projectile.friendly = true;
            projectile.penetrate = 20;
        }

        public override void SafeAI()
        {
            projectile.rotation += MathHelper.ToRadians(8f);
            Vector2 spawnPosition = projectile.Center + Utils.NextVector2Circular(Main.rand, projectile.width, projectile.height) / 2f;
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(spawnPosition, (int)CalamityDusts.ProfanedFire);
                dust.velocity = projectile.DirectionFrom(spawnPosition);
                dust.scale = Main.rand.NextFloat(1.1f, 1.5f);
                dust.noGravity = true;
            }
        }
        public override void DamageKillEffect()
        {
            Utils.PoofOfSmoke(projectile.Center);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    float angle = MathHelper.TwoPi / 6f * (i - 1);
                    Projectile shard = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, mod.ProjectileType("ArtifactOfResilienceShard" + i.ToString()), projectile.damage, projectile.knockBack, projectile.owner);
                    shard.ai[0] = angle;
                    shard.frameCounter = projectile.frameCounter + 1; // FrameCounter is a stored value for the amount of times the bulwark has reformed.
                }
            }
        }
    }
}
