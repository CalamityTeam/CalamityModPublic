using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SpearofDestinyStealth : ModProjectile, ILocalizedModType
    {
        public static readonly SoundStyle Hitsound = new("CalamityMod/Sounds/Item/BlazingCoreParry") { Volume = 0.7f, PitchVariance = 0.3f};
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Rogue/LanceofDestiny";
        public bool posthit = false;
        public int Time = 0;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 23;
            Projectile.height = 23;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Timer++;
            Time++;
            Projectile.scale = 1.5f;
            Projectile.velocity *= 1.02f;
            if (!Projectile.Calamity().stealthStrike)
                Projectile.extraUpdates = 1;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            float radiusFactor = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(10f, 50f, Time, true));
            for (int i = 0; i < 10; i++)
            {
                float offsetRotationAngle = Projectile.velocity.ToRotation() + Time / 20f;
                float radius = (50f + (float)Math.Cos(Time / 3f) * 12f) * radiusFactor;
                Vector2 dustPosition = Projectile.Center;
                dustPosition += offsetRotationAngle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi) * radius;
                Dust dust = Dust.NewDustPerfect(dustPosition, Main.rand.NextBool() ? 279 : 159);
                dust.noGravity = true;
                dust.velocity = Projectile.velocity * 0.5f;
                dust.scale = 1.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Hitsound, Projectile.position);
            posthit = true;
            float numberOfDusts = 35f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(18f, 0).RotatedBy(rot);
                Vector2 velOffset = new Vector2(9f, 0).RotatedBy(rot);
                SparkParticle spark = new SparkParticle(Projectile.position + offset, new Vector2(velOffset.X, velOffset.Y) * Main.rand.NextFloat( 1.5f, 2.3f), false, Main.rand.Next(23, 28), 1.9f, Main.rand.NextBool(5) ? Color.Gold : Color.PaleGoldenrod);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int i = 0; i <= 17; i++)
            {
                Dust dust2 = Main.dust[Dust.NewDust(target.position, Projectile.width, Projectile.height, 130, 0, 0, 0, default, 1.5f)];
                dust2.velocity.Y -= Main.rand.NextFloat(2.5f, 10.5f);
                dust2.velocity.X += Main.rand.NextFloat(-3f, 3f);
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0f, ModContent.ProjectileType<SpearofDestinyStealthExplosion>(), Projectile.damage / 2, Projectile.knockBack * 2, Projectile.owner, 0f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            float scale = Projectile.scale;
            float rotation = Projectile.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
