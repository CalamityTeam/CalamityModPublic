using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class MercurialTidesBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTidesShockwave";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => (60f - projectile.timeLeft) / 100f;
        public ref float Variant => ref projectile.ai[0]; //Yes
        public ref float Size => ref projectile.ai[0]; //Yes

        public Particle BloomRing;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercurial Blast");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 170;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
        }

        public override bool CanDamage() => projectile.timeLeft < 40;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - (projHitbox.Size() * projectile.scale * 0.5f), projHitbox.Size() * projectile.scale);
        }

        public override void AI()
        {
            projectile.velocity = Vector2.Zero;
            projectile.scale = (1 + (float)Math.Sin(projectile.timeLeft / 60f * MathHelper.Pi) * 0.2f) * Size;

            if (projectile.timeLeft == 60)
            {
                Main.PlaySound(SoundID.Item79, projectile.Center);
                Particle Sparkle = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White, Main.rand.NextBool() ? Color.Aqua : Color.SpringGreen, projectile.scale, 20, 0.2f, 2);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }



            if (projectile.timeLeft == 40)
            {
                Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode, projectile.Center);

                BloomRing = new BloomRing(projectile.Center, Vector2.Zero, Color.Aqua, projectile.scale, 40);
                GeneralParticleHandler.SpawnParticle(BloomRing);

                Particle Bloom = new StrongBloom(projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.Aqua * 0.6f : Color.SpringGreen * 0.6f, projectile.scale * (1f + Main.rand.NextFloat(0f, 1.5f)), 20);
                GeneralParticleHandler.SpawnParticle(Bloom);
                for (int i = 0; i < 10; i++)
                {
                    Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.DarkSlateBlue : Color.Chocolate, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

                for (float i = 0f; i < 1; i += 0.05f)
                {
                    float rotation = i * MathHelper.TwoPi;
                    Particle Sparkle = new CritSpark(projectile.Center + rotation.ToRotationVector2() * 65f * projectile.scale, rotation.ToRotationVector2() * 10f, Color.White, Main.rand.NextBool() ? Color.Aqua : Color.SpringGreen, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }
            }

            if (BloomRing != null)
            {
                BloomRing.Scale = projectile.scale;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Owner.HeldItem.modItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.ShockwaveAttunement_BlastProc)
                sword.OnHitProc = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTidesShockwave");

            float drawAngle = projectile.rotation;
            int animFrame = 6 - (int)Math.Ceiling(projectile.timeLeft / 10f);
            Rectangle frame = new Rectangle(0, 0 + animFrame * 168, 170, 166);

            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = frame.Size() / 2f;

            spriteBatch.Draw(tex, drawPosition, frame, Color.White, drawAngle, drawOrigin, projectile.scale, 0f, 0f);


            return false;
        }
    }
}