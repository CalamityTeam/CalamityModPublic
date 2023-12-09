using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PristineSecondary : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Magic/RancorFog";

        public ref float ScaleFactor => ref Projectile.ai[0];
        public ref float LightPower => ref Projectile.ai[1];

        public Color FogColor = new Color(255, 220, 100);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 2;
        }

        public override void AI()
        {
            // Add some degree of variation to the fog with scale/rotation/color
            if (Projectile.ai[2] == 0f)
            {
                Projectile.scale = Main.rand.NextFloat(0.15f, 0.75f);
                Projectile.ai[2] = Main.rand.NextFloat(MathHelper.TwoPi);
                FogColor.G = (byte)Main.rand.Next(160, 230 + 1);
            }
            ScaleFactor += 0.01f;
            ScaleFactor = MathHelper.Clamp(ScaleFactor, 0f, Projectile.scale);
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 1f, 0.25f) * ScaleFactor);
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[2];

            Projectile.velocity *= 0.98f;
            Projectile.Opacity = Utils.GetLerpValue(150f, 135f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 90f, Projectile.timeLeft, true);

            // 08DEC2023: Ozzatron: All below code does not run on dedicated servers as it requires clientside lighting information.
            if (Main.netMode == NetmodeID.Server)
                return;

            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * ScaleFactor * 0.5f, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Utils.GetLerpValue(0f, 0.08f, LightPower, true) * Projectile.Opacity * 0.5f;
            Color drawColor = FogColor * opacity;
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, Projectile.rotation, texture.Size() * 0.5f, ScaleFactor, SpriteEffects.None);

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
