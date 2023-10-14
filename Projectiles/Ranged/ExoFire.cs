using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator left click main projectile (the flamethrower itself)
    public class ExoFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Magic/RancorFog";

        public ref float ColorType => ref Projectile.ai[0];
        public ref float ScaleFactor => ref Projectile.ai[1];
        public ref float LightPower => ref Projectile.ai[2];

        public float FogRot = 0f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = 180;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
        }

        public override void AI()
        {
            // Add some degree of variation to the fog with scale/rotation/color
            if (FogRot == 0f)
            {
                Projectile.scale = Main.rand.NextFloat(0.4f, 0.8f);
                FogRot = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            ColorType += Main.rand.NextFloat(0.02f, 0.06f);
            ScaleFactor += 0.02f;
            ScaleFactor = MathHelper.Clamp(ScaleFactor, 0f, Projectile.scale);
            Lighting.AddLight(Projectile.Center, CloudColor(ColorType % 3f).ToVector3() * ScaleFactor);
            Projectile.rotation = Projectile.velocity.ToRotation() + FogRot;

            Projectile.Opacity = Utils.GetLerpValue(0f, 12f, Projectile.timeLeft, true);
            if (Projectile.timeLeft < 60)
                Projectile.velocity *= 0.99f;

            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * ScaleFactor * 0.5f, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Utils.GetLerpValue(0f, 0.08f, LightPower, true) * Projectile.Opacity;
            Color drawColor = CloudColor(ColorType % 3f) * opacity;
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, Projectile.rotation, texture.Size() * 0.5f, ScaleFactor, SpriteEffects.None);

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }

        public static Color CloudColor(float type)
        {
            type = MathHelper.Clamp(type, 0f, 3f);
            Color cloud = Color.White;

            // Cycles with these 3 colors
            Color Purple = new Color(220, 120, 255);
            Color Green = new Color(120, 255, 120);
            Color Yellow = new Color(255, 255, 120);

            if (type >= 2f)
                cloud = Color.Lerp(Green, Yellow, type - 2f);
            else if (type >= 1f)
                cloud = Color.Lerp(Purple, Green, type - 1f);
            else
                cloud = Color.Lerp(Yellow, Purple, type);

            return cloud;
        }
    }
}
