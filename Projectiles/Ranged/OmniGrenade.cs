using System.Runtime.CompilerServices;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class OmniGrenade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (Projectile.velocity.Y < 25f)
                Projectile.velocity.Y += 0.3f;
            int fire = Dust.NewDust(Projectile.position, Projectile.width, (int)(Projectile.height * 0.1f), DustID.Torch, 0f, 0f, 100, default);
            Main.dust[fire].velocity *= 3f;
            if (Main.rand.NextBool())
            {
                Main.dust[fire].scale = 0.3f;
                Main.dust[fire].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62 with { Volume = 7f }, Projectile.Center);
            Projectile.ExpandHitboxBy(250);
            Projectile.Damage();
            Player Owner = Main.player[Projectile.owner];
            Owner.SetScreenshake(3f);
            for (int i = 0; i < 2; i++)
            {
                Particle center = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 11, 0.95f, Color.OrangeRed * 0.8f, new Vector2(1f, 1f), true, true, glowOpacity: 0.9f);
                GeneralParticleHandler.SpawnParticle(center);
            }
            Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 0.8f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(-12, 12), 0f, 0.15f, 21, true);
            GeneralParticleHandler.SpawnParticle(blastRing);
            Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 0.8f, "CalamityMod/Particles/PlasmaExplosion", Vector2.One, Main.rand.NextFloat(-10, 10), 0f, 0.07f, 21);
            GeneralParticleHandler.SpawnParticle(blastRing2);
            Particle blastRing3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Red * 0.8f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(-12, 12), 0f, 0.1f, 21, true);
            GeneralParticleHandler.SpawnParticle(blastRing3);
            Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/RoarPulse", new Vector2(1f, 1f), 0, 0.01f, 0.4f, 15, true);
            GeneralParticleHandler.SpawnParticle(pulse);
            Particle pulse2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/SmallBloomRing", new Vector2(1f, 1f), 0, 0.01f, 4f, 15, true, 0.8f);
            GeneralParticleHandler.SpawnParticle(pulse2);
            for (int i = 0; i < 12; i++)
            {
                Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                SparkParticle spark = new SparkParticle(Projectile.Center, sparkVelocity, false, Main.rand.Next(10, 15), Main.rand.NextFloat(1f, 1.2f), Main.rand.NextBool() ? Color.OrangeRed : Color.Red);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int i = 1; i < 21; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(9f, 14f);
                Dust spark = Dust.NewDustPerfect(Projectile.Center, 278, velocity);
                spark.color = Main.rand.NextBool() ? Color.OrangeRed : Color.Gold;
            }
            for (int j = 0; j < 25; j++)
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dust2].velocity *= 2f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            Player Owner = Main.player[Projectile.owner];
            if (target == Owner)
            {
                modifiers.SourceDamage *= 0f;
                if (Main.masterMode) modifiers.SourceDamage.Flat += 240f;
                else if (Main.expertMode) modifiers.SourceDamage.Flat += 180f;
                else modifiers.SourceDamage.Flat += 120f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, position, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}
