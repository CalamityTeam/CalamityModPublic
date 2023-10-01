using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.DraedonsArsenal;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class AnomalysNanogunMPFBDevastator : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // All of AI is visual
            if (Main.dedServ)
                return;

            // Spawn dusts in a helix-style pattern
            float sine = (float)Math.Sin(Projectile.timeLeft * 0.375f / MathHelper.Pi);

            Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 16f;
            Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 267, Vector2.Zero);
            dust.color = Color.Cyan;
            dust.noGravity = true;

            dust = Dust.NewDustPerfect(Projectile.Center - offset, 267, Vector2.Zero);
            dust.color = Color.Cyan;
            dust.noGravity = true;

            // Add light
            Lighting.AddLight(Projectile.Center, 0, Projectile.Opacity * 0.7f / 255f, Projectile.Opacity);

            // Animate the projectile
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 3 % Main.projFrames[Projectile.type];
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int yPos = texture.Height / Main.projFrames[Type] * Projectile.frame;
            Main.EntitySpriteDraw(texture, 
                Projectile.position - Main.screenPosition, 
                new Rectangle(0, yPos, texture.Width, texture.Height / Main.projFrames[Type]), 
                lightColor, 
                0f, 
                Vector2.Zero, 
                1f, 
                SpriteEffects.None, 
                0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(AnomalysNanogunMPFBBoom.MPFBExplosion, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AnomalysNanogunMPFBBoom>(), (int)(Projectile.damage * 0.2f), Projectile.knockBack, Projectile.owner);
                    explosion.ai[1] = Main.rand.NextFloat(110f, 200f) + i * 20f; // Randomize the maximum radius.
                    explosion.localAI[1] = Main.rand.NextFloat(0.18f, 0.3f); // And the interpolation step.
                    explosion.netUpdate = true;
                }
            }
        }
    }
}
