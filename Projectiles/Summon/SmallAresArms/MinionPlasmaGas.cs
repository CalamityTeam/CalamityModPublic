using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class MinionPlasmaGas : ModProjectile, IAdditiveDrawer, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float LightPower => ref Projectile.ai[0];

        public ref float Time => ref Projectile.ai[1];

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Projectile.type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.scale = 1.5f;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        public override void AI()
        {
            // Decide scale and initial rotation on the first frame this projectile exists.
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale = Main.rand.NextFloat(1f, 1.7f);
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.localAI[0] = 1f;
            }

            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);
            Projectile.Opacity = Utils.GetLerpValue(0f, 15f, Time, true) * Utils.GetLerpValue(0f, 60f, Projectile.timeLeft, true);
            Projectile.rotation += Projectile.velocity.X * 0.004f;
            Projectile.velocity *= 0.97f;
            Time++;
        }

        public void AdditiveDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Utils.GetLerpValue(0f, 0.08f, LightPower, true) * Projectile.Opacity * 0.3f;
            Color drawColor = new Color(141, 255, 105) * opacity;
            Vector2 scale = Projectile.Size / texture.Size() * Projectile.scale * 1.35f;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
