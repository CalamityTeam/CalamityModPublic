using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class MercurialTidesBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTidesShockwave";
        public Player Owner => Main.player[Projectile.owner];
        public float Timer => (60f - Projectile.timeLeft) / 100f;
        public ref float Variant => ref Projectile.ai[0]; //Yes
        public ref float Size => ref Projectile.ai[0]; //Yes

        public Particle BloomRing;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercurial Blast");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 170;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }

        public override bool? CanDamage() => Projectile.timeLeft < 40;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - (projHitbox.Size() * Projectile.scale * 0.5f), projHitbox.Size() * Projectile.scale);
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.scale = (1 + (float)Math.Sin(Projectile.timeLeft / 60f * MathHelper.Pi) * 0.2f) * Size;

            if (Projectile.timeLeft == 60)
            {
                SoundEngine.PlaySound(SoundID.Item79, Projectile.Center);
                Particle Sparkle = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.White, Main.rand.NextBool() ? Color.Aqua : Color.SpringGreen, Projectile.scale, 20, 0.2f, 2);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }



            if (Projectile.timeLeft == 40)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

                BloomRing = new BloomRing(Projectile.Center, Vector2.Zero, Color.Aqua, Projectile.scale, 40);
                GeneralParticleHandler.SpawnParticle(BloomRing);

                Particle Bloom = new StrongBloom(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.Aqua * 0.6f : Color.SpringGreen * 0.6f, Projectile.scale * (1f + Main.rand.NextFloat(0f, 1.5f)), 20);
                GeneralParticleHandler.SpawnParticle(Bloom);
                for (int i = 0; i < 10; i++)
                {
                    Particle Sparkle = new CritSpark(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * Projectile.scale, Color.White, Main.rand.NextBool() ? Color.DarkSlateBlue : Color.Chocolate, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

                for (float i = 0f; i < 1; i += 0.05f)
                {
                    float rotation = i * MathHelper.TwoPi;
                    Particle Sparkle = new CritSpark(Projectile.Center + rotation.ToRotationVector2() * 65f * Projectile.scale, rotation.ToRotationVector2() * 10f, Color.White, Main.rand.NextBool() ? Color.Aqua : Color.SpringGreen, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }
            }

            if (BloomRing != null)
            {
                BloomRing.Scale = Projectile.scale;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Owner.HeldItem.ModItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.ShockwaveAttunement_BlastProc)
                sword.OnHitProc = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTidesShockwave").Value;

            float drawAngle = Projectile.rotation;
            int animFrame = 6 - (int)Math.Ceiling(Projectile.timeLeft / 10f);
            Rectangle frame = new Rectangle(0, 0 + animFrame * 168, 170, 166);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = frame.Size() / 2f;

            Main.EntitySpriteDraw(tex, drawPosition, frame, Color.White, drawAngle, drawOrigin, Projectile.scale, 0f, 0);

            return false;
        }
    }
}
