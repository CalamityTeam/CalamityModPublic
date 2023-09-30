using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Damageable
{
    public class ArtifactOfResilienceBulwark : DamageableProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public int Reformations = 0;
        public const int MaxReformations = 3;
        public override int LifeMax => 1;
        public override int MaxDamageImmunityFrames => 5;
        public override DamageSourceType DamageSources => DamageSourceType.FriendlyProjectiles | DamageSourceType.HostileProjectiles | DamageSourceType.HostileNPCs;
        public override SoundStyle HitSound => SoundID.NPCHit3;
        public override SoundStyle DeathSound => SoundID.NPCDeath14;
        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 76;
            Projectile.friendly = true;
            Projectile.penetrate = 20;
        }

        public override void SafeAI()
        {
            Projectile.rotation += MathHelper.ToRadians(8f);
            Vector2 spawnPosition = Projectile.Center + Utils.NextVector2Circular(Main.rand, Projectile.width, Projectile.height) / 2f;
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(spawnPosition, (int)CalamityDusts.ProfanedFire);
                dust.velocity = Projectile.DirectionFrom(spawnPosition);
                dust.scale = Main.rand.NextFloat(1.1f, 1.5f);
                dust.noGravity = true;
            }
        }
        public override void DamageKillEffect()
        {
            Utils.PoofOfSmoke(Projectile.Center);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    float angle = MathHelper.TwoPi / 6f * (i - 1);
                    Projectile shard = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Mod.Find<ModProjectile>("ArtifactOfResilienceShard" + i.ToString()).Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    shard.ai[0] = angle;
                    shard.frameCounter = Projectile.frameCounter + 1; // FrameCounter is a stored value for the amount of times the bulwark has reformed.
                }
            }
        }
    }
}
