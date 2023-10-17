using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AuroraFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Magic/RancorFog";

        public static int Lifetime => 450;
        public static int Fadetime => 420;
        public ref float Time => ref Projectile.ai[0];
        public ref float LightPower => ref Projectile.ai[1];

        // The Fog is Coming
        public Color OrangeFogColor = new Color(255, 160, 100);
        public float OrangeFogRot = 0f;
        public float OrangeFogScale = 1f;
        public Color BlueFogColor = new Color(150, 120, 255);
        public float BlueFogRot = 0f;
        public float BlueFogScale = 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Lifetime;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override void AI()
        {
            // Initialize random fog scale/rotation/color
            if (Time == 0f)
            {
                OrangeFogColor.B = (byte)Main.rand.Next(100, 180 + 1);
                OrangeFogScale = Main.rand.NextFloat(0.8f, 1f);
                OrangeFogRot = Main.rand.NextFloat(MathHelper.TwoPi);
                BlueFogColor.G = (byte)Main.rand.Next(120, 250 + 1);
                BlueFogScale = OrangeFogScale * Main.rand.NextFloat(0.9f, 1.1f);
                BlueFogRot = OrangeFogRot + MathHelper.ToRadians(Main.rand.NextFloat(30f, 330f));
            }
            Time++;

            // Turns around every once in a while
            int TurnRate = Lifetime / 5;
            if (Time % TurnRate == TurnRate - 1)
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(-216f));
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Determines particle size as well as hitbox
            if (Time >= Fadetime)
            {
                Projectile.scale = Utils.GetLerpValue(MathHelper.Lerp(Fadetime, Lifetime, 0.5f), Fadetime, Time, true);
                if (Projectile.scale <= 0.01f)
                    Projectile.Kill();
            }
            else if (Time >= 6f)
                Projectile.scale = Utils.GetLerpValue(6f, 36f, Time, true);
            else
                return; // Helps position it at the tip

            // Draw smokes over the whole thing
            float smokeRot = MathHelper.ToRadians(3f); // *Rate of rotation per frame, not a constant rotation
            Color smokeColor = Color.Lerp(OrangeFogColor, BlueFogColor, 0.6f + 0.4f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f));
            Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 8, Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f), 0.8f, smokeRot, required: true);
            GeneralParticleHandler.SpawnParticle(smoke);

            // Overlay the glow on top, which is on the brighter side
            if (Main.rand.NextBool(8))
            {
                Color glowColor = Color.Lerp(smokeColor, Color.White, 0.25f);
                Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, glowColor, 6, Projectile.scale * Main.rand.NextFloat(0.4f, 0.7f), 0.6f, smokeRot, true, 0.005f, true);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            // Lighting and fog
            Lighting.AddLight(Projectile.Center, smokeColor.ToVector3() * Projectile.scale);
            OrangeFogRot += MathHelper.ToRadians(1f);
            BlueFogRot -= MathHelper.ToRadians(1f);
            Projectile.Opacity = Utils.GetLerpValue(0f, 15f, Time, true) * Utils.GetLerpValue(450f, 360f, Time, true);
            
            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);

        // Circular hitbox adjusted for the size of the smoke particles (rough estimate minimally accounting for fog)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * Projectile.scale * 0.5f, targetHitbox);

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D fog = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Utils.GetLerpValue(0f, 0.08f, LightPower, true) * Projectile.Opacity * 0.3f;
            Main.EntitySpriteDraw(fog, drawPosition, null, OrangeFogColor * opacity, Projectile.rotation + OrangeFogRot, fog.Size() * 0.5f, Projectile.scale * OrangeFogScale, SpriteEffects.None);
            Main.EntitySpriteDraw(fog, drawPosition, null, BlueFogColor * opacity, Projectile.rotation + BlueFogRot, fog.Size() * 0.5f, Projectile.scale * BlueFogScale, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
