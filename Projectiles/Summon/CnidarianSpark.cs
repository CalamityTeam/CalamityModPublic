using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CnidarianSpark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float Target => ref Projectile.ai[0];
        public Vector2 initialVelocity;

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 15;
        }


        public override bool? CanHitNPC(NPC target)
        {
            if (Main.npc[(int)Target] != target)
                return false;

            return base.CanHitNPC(target);
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
                initialVelocity = Projectile.velocity;

            Projectile.velocity = Vector2.Zero;

            if (Main.npc[(int)Target] != null)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Main.npc[(int)Target].Center, 0.4f);
                Projectile.rotation = (Projectile.Center - Main.npc[(int)Target].Center).ToRotation() - MathHelper.PiOver2;
            }

            Color bloomColor = Main.rand.NextBool() ? (Main.rand.NextBool() ? Color.Gold : Color.Cyan) : Color.SpringGreen;
            CritSpark particle = new CritSpark(Projectile.Center, (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 4f, Color.LightSkyBlue, bloomColor, 0.8f, 10, bloomScale: 2f);
            GeneralParticleHandler.SpawnParticle(particle);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CnidarianJellyfishOnTheString.ZapSound, Projectile.Center);

            for (int i = 0; i < 4; i++)
            {
                Color bloomColor = Main.rand.NextBool() ? (Main.rand.NextBool() ? Color.Gold : Color.Cyan) : Color.SpringGreen;
                CritSpark particle = new CritSpark(Projectile.Center, initialVelocity * i, Color.LightSkyBlue, bloomColor, 0.8f, 10, bloomScale: 2f);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }
    }
}
