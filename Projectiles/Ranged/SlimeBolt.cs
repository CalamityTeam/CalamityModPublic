using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SlimeBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/ExtraTextures/TinyGreyscaleCircle";

        public static int Lifetime => 270;
        public static float Fadetime => 225f;
        public static float EmpowerTime => 135f;
        public static Color SlimeColor => new Color (133, 133, 224);

        public ref float Time => ref Projectile.ai[0];
        public bool Empowered => Projectile.ai[0] >= EmpowerTime;
        public ref float BloomPower => ref Projectile.ai[1];
        public bool Bounced => Projectile.ai[2] >= 1f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Time++;
            if (Time == EmpowerTime)
            {
                Projectile.penetrate = 1;
                Projectile.damage *= 2;
                Projectile.velocity *= 0f;
                Projectile.rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);

                float numberOfDusts = 10;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(3, 3.1f), 0).RotatedBy(rot);
                    Vector2 velOffset = new Vector2(Main.rand.NextFloat(3, 3.1f), 0).RotatedBy(rot);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(3) ? 59 : 20, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = Main.rand.NextFloat(1.2f, 1.9f);
                }
            }
            else if (Time >= 90f)
                Projectile.velocity *= 0.97f;

            if (Empowered)
            {
                Projectile.rotation += MathHelper.ToRadians(2f);
                if (Time >= Fadetime)
                    BloomPower = Utils.Remap(Time, Fadetime, Lifetime, 1.5f, 0f);
                else
                    BloomPower = Utils.Remap(Time, EmpowerTime, Fadetime, 0f, 1.5f);
            }
            else if (Main.rand.NextBool(Bounced ? 2 : 7))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextBool(3) ? 16 : 20);
                dust.scale = Main.rand.NextFloat(0.3f, 0.7f);
                dust.velocity = -Projectile.velocity * 0.7f;
            }

            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.5f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * (Bounced ? 1f : Utils.Remap(Time, 0f, EmpowerTime, 1.5f, 4f));
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * (Bounced ? 1f : Utils.Remap(Time, 0f, EmpowerTime, 1.5f, 4f));
            }
            Projectile.ai[2]++;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, SlimeColor, Vector2.One, 0f, 0f, 0.4f, 25);
            GeneralParticleHandler.SpawnParticle(pulse);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, Empowered ? 480 : 180);
            if (Empowered)
            {
                Particle pulse2 = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, SlimeColor, Vector2.One, 0f, 0f, 0.65f, 35);
                GeneralParticleHandler.SpawnParticle(pulse2);
                float numberOfDusts = 12;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot);
                    Vector2 velOffset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(3) ? 34 : 59, velOffset);
                    dust.noGravity = false;
                    dust.alpha = 130;
                    dust.velocity = velOffset;
                    dust.scale = dust.type == 20 ? Main.rand.NextFloat(0.9f, 1.9f) : Main.rand.NextFloat(1.6f, 2.2f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, SlimeColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            if (Empowered)
            {
                Texture2D shineTex = ModContent.Request<Texture2D>("CalamityMod/Particles/Sparkle").Value;
                Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, SlimeColor * 0.5f, Projectile.rotation, bloomTex.Size() * 0.5f, BloomPower * Projectile.scale * 0.3f, SpriteEffects.None);
                Main.EntitySpriteDraw(shineTex, drawPosition, null, SlimeColor, Projectile.rotation, shineTex.Size() * 0.5f, BloomPower * Projectile.scale, SpriteEffects.None);
            }
            else
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    float completionRatio = i / (float)Projectile.oldPos.Length;
                    Vector2 trailPos = Projectile.oldPos[i] + texture.Size() * 0.5f - Main.screenPosition;

                    // The further the smaller
                    Color trailColor = Color.Lerp(SlimeColor, Color.Black, completionRatio);
                    float trailScale = MathHelper.Lerp(0.15f, 1f, 1f - completionRatio);

                    Main.EntitySpriteDraw(texture, trailPos, null, trailColor, 0f, texture.Size() * 0.5f, Projectile.scale * trailScale, SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
