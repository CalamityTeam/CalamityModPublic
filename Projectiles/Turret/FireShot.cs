using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace CalamityMod.Projectiles.Turret
{
    public class FireShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public bool ableToHit = true;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fire Shot");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 52;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 10;
        }

        public override bool PreAI()
        {
            // If projectile knockback is set to 0 in the tile entity file, projectile hits players instead
            // This is used to check if the projectile came from the hostile version of the tile entity
            if (Projectile.knockBack == 0f)
                Projectile.hostile = true;
            else Projectile.friendly = true;
            return true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                // play a sound frame 1.
                var sound = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/FlamethrowerTurret") with { Volume = 0.65f }, Projectile.Center);
            }
            Projectile.localAI[0]++;
            Projectile.velocity.Y -= 0.065f;
            if (Projectile.timeLeft < 16) //stop moving at the end and remove the ability to deal damage
            {
                Projectile.velocity *= 0f;
                ableToHit = false;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 100);
        }

        public override bool? CanDamage() => ableToHit ? (bool?)null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            float rotation = 0f;
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleVortex").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos((Projectile.timeLeft - 16) / 7f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.35f + 0.65f;
                Color color = Color.Lerp(Color.Yellow, Color.OrangeRed, colorInterpolation) * 0.4f;
                color.A = 7;
                rotation += 0.35f;
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-186f, -186f); //last vector displaces the texture back to where the hitbox is
                float intensity = 0.6f;
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                intensity *= 1.5f * (Projectile.localAI[0] / 40f + 0.0375f); //become bigger the longer the projectile is alive
                if (Projectile.timeLeft < 22)
                {
                    color.A += 50;
                }
                if (Projectile.timeLeft < 16)
                    intensity *= Projectile.timeLeft / 15;
                // Become smaller the futher along the old positions we are.
                Vector2 scale = new Vector2(1f) * intensity;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, color, rotation, lightTexture.Size() * 0.5f, scale * 0.25f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
