using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Projectiles.Summon.MirrorofKalandraMinions
{
    public class StarforgeBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 12000;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = MirrorofKalandra.Purple_BlastChargeTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = MirrorofKalandra.Purple_BlastChargeTime;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = MirrorofKalandra.Purple_BlastSize;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            GenericBloom boom = new GenericBloom(Projectile.Center,
                Vector2.Zero,
                Color.Lerp(Color.DarkMagenta, Color.White, MathF.Pow(Utils.GetLerpValue(MirrorofKalandra.Purple_BlastChargeTime, 0f, Projectile.timeLeft), 6)), // No Color Remap function, insert sad emote.
                Utils.Remap(Projectile.timeLeft, MirrorofKalandra.Purple_BlastChargeTime, 5f, .5f, 4.2f),
                MirrorofKalandra.Purple_BlastChargeTime);
            GeneralParticleHandler.SpawnParticle(boom);
        }

        public override void Kill(int timeLeft)
        {
            int dustAmount = 100;
            for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
            {
                float angle = MathHelper.TwoPi / dustAmount * dustIndex;
                Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(2f, 15f);
                Dust boomDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(MirrorofKalandra.Purple_BlastSize / 2, MirrorofKalandra.Purple_BlastSize / 2), 272, velocity);
                boomDust.noGravity = true;
                boomDust.velocity *= .8f;
                boomDust.scale = velocity.Length() * .08f;
            }

            // Electric boom sound.
            SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
        }
    }
}
