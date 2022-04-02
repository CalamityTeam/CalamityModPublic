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
    public class TrueAncientBlast : ModProjectile 
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Energy Blast");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        const float MaxTime = 20;
        public float Timer => MaxTime - projectile.timeLeft;

        public Player Owner => Main.player[projectile.owner];

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 8;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
            projectile.timeLeft = 20;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity, 20f, ref collisionPoint);
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Timer == 0)
            {
                Particle Star = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White, Color.HotPink, Main.rand.NextFloat(2f, 2.5f), 20, 0.1f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);

                Star = new GenericSparkle(projectile.Center + projectile.velocity, Vector2.Zero, Color.White, Color.PaleGoldenrod, Main.rand.NextFloat(2f, 2.5f), 20, 0.1f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);

                Main.PlaySound(SoundID.Item84, projectile.Center);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Color pulseColor = Main.rand.NextBool() ? (Main.rand.NextBool() ? Color.CornflowerBlue : Color.Coral) : (Main.rand.NextBool() ? Color.GreenYellow : Color.Gold);
            Particle pulse = new PulseRing(target.Center, Vector2.Zero, pulseColor, 0.05f, 0.2f + Main.rand.NextFloat(0f, 1f), 30);
            GeneralParticleHandler.SpawnParticle(pulse);

            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Utils.SafeNormalize(projectile.velocity, Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(2.6f, 4f);
                Particle energyLeak = new SquishyLightParticle(target.Center, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            //Add some damage falloff
            damage = (int)(damage * Math.Pow((1 - TrueArkoftheAncients.blastFalloffStrenght), projectile.numHits * TrueArkoftheAncients.blastFalloffSpeed));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/TrueArkoftheAncients");
            Texture2D glowmask = GetTexture("CalamityMod/Items/Weapons/Melee/TrueArkoftheAncientsGlow");

            float displace = 10 * (1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.8f));

            float angle = projectile.velocity.ToRotation();

            float drawRotation = angle + MathHelper.PiOver4;
            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = Owner.Center + angle.ToRotationVector2() * displace - Main.screenPosition;

            float swordSize = 1.4f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); 

            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, swordSize, 0f, 0f);
            spriteBatch.Draw(glowmask, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, swordSize, 0f, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //Draw the laser
            Texture2D tex = GetTexture("CalamityMod/Particles/BloomLine");
            float rot = angle + MathHelper.PiOver2;
            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height);
            float size = Timer / MaxTime > 0.5f ? (float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.2f + 0.8f : (float)Math.Sin(Timer / MaxTime * MathHelper.Pi);
            size *= 3;
            Vector2 scale = new Vector2(size, projectile.velocity.Length() / tex.Height);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.LightPink, rot, origin, scale, SpriteEffects.None, 0);

            //Cap the lines
            Texture2D cap = GetTexture("CalamityMod/Particles/BloomLineCap");
            scale = new Vector2(size, size);
            origin = new Vector2(cap.Width / 2f, cap.Height);
            spriteBatch.Draw(cap, projectile.Center - Main.screenPosition, null, Color.LightPink, rot + MathHelper.Pi, origin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(cap, projectile.Center + projectile.velocity - Main.screenPosition, null, Color.LightPink, rot, origin, scale, SpriteEffects.None, 0);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}