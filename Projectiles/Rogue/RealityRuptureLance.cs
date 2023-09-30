using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RealityRuptureLance : ModProjectile, ILocalizedModType
    {
        public static readonly SoundStyle Hitsound = new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak2") { Volume = 0.6f, PitchVariance = 0.3f };
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RealityRupture";
        public int Time = 0;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 8;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            Timer++;
            Time++;
            Lighting.AddLight(Projectile.Center + Projectile.velocity * 0.6f, 0.6f, 0.2f, 0.9f);
            float radiusFactor = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(10f, 50f, Time, true));
            for (int i = 0; i < 9; i++)
            {
                float offsetRotationAngle = Projectile.velocity.ToRotation() + Time / 20f;
                float radius = (20f + (float)Math.Cos(Time / 3f) * 12f) * radiusFactor;
                Vector2 dustPosition = Projectile.Center;
                dustPosition += offsetRotationAngle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi) * radius;
                Dust dust = Dust.NewDustPerfect(dustPosition, Main.rand.NextBool() ? 234 : 310);
                dust.noGravity = true;
                dust.velocity = Projectile.velocity * 0.8f;
                dust.scale = Main.rand.NextFloat(1.1f, 1.7f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i <= 4; i++)
            {
                Dust dust2 = Main.dust[Dust.NewDust(target.position, Projectile.width, Projectile.height, Main.rand.NextBool(4) ? 242 : 310, Projectile.oldVelocity.X * Main.rand.NextFloat(1.1f, 1.3f), Projectile.oldVelocity.Y * Main.rand.NextFloat(1.1f, 1.3f), 0, default, 1.1f)];
            }
            SoundEngine.PlaySound(Hitsound, Projectile.position);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.95f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 9; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 272, Projectile.oldVelocity.X * 0.3f, Projectile.oldVelocity.Y * 0.3f, 0, default, Main.rand.NextFloat(1.2f, 1.6f));
            }
        }
    }
}
