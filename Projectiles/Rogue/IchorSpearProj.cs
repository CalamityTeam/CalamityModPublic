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
    public class IchorSpearProj : ModProjectile, ILocalizedModType
    {
        public static readonly SoundStyle Hitsound = new("CalamityMod/Sounds/Item/WulfrumKnifeTileHit2") { PitchVariance = 0.3f, Volume = 0.5f };
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/IchorSpear";
        public bool posthit = false;
        public int framesInAir = 0;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
        }

        public override void AI()
        {
            framesInAir++;
            if (framesInAir > 90 && !Projectile.Calamity().stealthStrike)
            {
                Projectile.velocity.X *= 0.998f;
                Projectile.velocity.Y += 0.3f;
            }

            Projectile.scale = 1.2f;
            if (!Projectile.Calamity().stealthStrike)
                Projectile.extraUpdates = 1;

            if (Main.rand.NextBool() && !posthit)
            {
                Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next(169, 170), 0f, 0f, 0, default, Projectile.Calamity().stealthStrike ? Main.rand.NextFloat(2.1f, 3.2f) : Main.rand.NextFloat(1.2f, 1.5f))];
                dust.position = position;
                dust.velocity = Projectile.velocity.RotatedBy(1.9707963705062866, default) * 0.1f + Projectile.velocity / 8f;
                dust.position += Projectile.velocity.RotatedBy(0.3, default);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
                dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next(169, 170), 0f, 0f, 0, default, Projectile.Calamity().stealthStrike ? Main.rand.NextFloat(2.1f, 3.2f) : Main.rand.NextFloat(1.2f, 1.5f))];
                dust.position = position;
                dust.velocity = Projectile.velocity.RotatedBy(-1.9707963705062866, default) * 0.1f + Projectile.velocity / 8f;
                dust.position += Projectile.velocity.RotatedBy(-0.3, default);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            bool defaultsonce = true;
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.tileCollide = false;
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
                Projectile.StickyProjAI(10);
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
            Vector2 bloodSpawnPosition = target.Center + Main.rand.NextVector2Circular(target.width, target.height) * 0.04f;
            Vector2 splatterDirection = (Projectile.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);
            int sparkCount = Main.rand.Next(4, 6);
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity = Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.6f, 1.1f);
                int sparkLifetime = Main.rand.Next(23, 25);
                float sparkScale = Main.rand.NextFloat(0.8f, 1f) * 0.955f;
                Color sparkColor = Color.Lerp(Color.Gold, Color.Goldenrod, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.Gold, Main.rand.NextFloat());
                SparkParticle spark = new SparkParticle(Projectile.Center, sparkVelocity, true, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            SoundEngine.PlaySound(Hitsound, Projectile.position);
            target.AddBuff(BuffID.Ichor, Projectile.Calamity().stealthStrike ? 900 : 180);
            if (Projectile.Calamity().stealthStrike)
            {
                posthit = true;
                for (int i = 0; i <= 17; i++)
                {
                    Dust dust2 = Main.dust[Dust.NewDust(target.position, Projectile.width, Projectile.height, Main.rand.Next(169, 170), 0, 0, 0, default, 3.2f)];
                    dust2.noGravity = true;
                    dust2.velocity.Y -= Main.rand.NextFloat(2.5f, 10.5f);
                    dust2.velocity.X += Main.rand.NextFloat(-3f, 3f);
                }
                SoundEngine.PlaySound(SoundID.NPCHit18, Projectile.position);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.Calamity().stealthStrike && !posthit)
            {
                Projectile.ModifyHitNPCSticky(2);
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
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Ichor, Projectile.Calamity().stealthStrike ? 600 : 120);
    }
}
