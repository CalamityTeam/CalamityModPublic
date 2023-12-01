using CalamityMod.Particles;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LanceofDestiny : ModProjectile, ILocalizedModType
    {
        public static readonly SoundStyle Hitsound = new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak2") { Volume = 0.6f, PitchVariance = 0.3f};
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Rogue/LanceofDestiny";
        public bool posthit = false;
        public int Time = 0;
        public int hitsDust = 7;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 6;
        }

        public override void AI()
        {
            Time++;
            Vector3 DustLight = new Vector3(0.255f, 0.252f, 0.100f);
            Lighting.AddLight(Projectile.Center, DustLight * 3);
            Projectile.velocity *= 1.02f;
            if (!Projectile.Calamity().stealthStrike)
                Projectile.extraUpdates = 1;

            if (Projectile.timeLeft % 2 == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    CritSpark spark = new CritSpark(Projectile.Center + Main.rand.NextVector2Circular(10, 10), -Projectile.velocity, Color.PaleGoldenrod, Color.Goldenrod, Main.rand.NextFloat(0.65f, 0.95f), 8, 0, 2.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            bool defaultsonce = true;
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.aiStyle = 0;
                Projectile.extraUpdates = 2;
                if (Projectile.ai[0] == 0f)
                {
                    if (defaultsonce)
                    {
                        Projectile.penetrate = 10;
                        Projectile.localNPCHitCooldown = 60;
                        defaultsonce = false;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hitsDust > 1)
            {
                hitsDust--;
            }
            for (int i = 0; i <= hitsDust; i++)
            {
                Vector2 sparkVelocity = Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1.5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, Main.rand.NextBool(3) ? 130 : 133, sparkVelocity, 0, default, Main.rand.NextFloat(1.2f, 1.5f));
                dust.noGravity = true;
            }
            SoundEngine.PlaySound(Hitsound, Projectile.Center);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.Calamity().stealthStrike && !posthit)
            {
                posthit = true;
            }
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.9f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8 && Projectile.Calamity().stealthStrike)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 9; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.Next(169, 170), Projectile.oldVelocity.X * 0.3f, Projectile.oldVelocity.Y * 0.3f, 0, default, Main.rand.NextFloat(1.2f, 1.6f));
            }
        }
    }
}
