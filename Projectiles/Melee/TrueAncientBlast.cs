using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;


namespace CalamityMod.Projectiles.Melee
{
    public class TrueAncientBlast : ModProjectile //The boring plain one
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

        public ref float angle => ref projectile.ai[0];
        public ref float lenght => ref projectile.ai[1];

        public Player Owner => Main.player[projectile.owner];

        public Vector2 lineVector => angle.ToRotationVector2() * lenght;

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
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + lineVector, 20f, ref collisionPoint);
        }

        public override void AI()
        {
            if (Timer == 0)
            {
                Particle Star = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White, Color.HotPink, Main.rand.NextFloat(1f, 1.5f), 20, 0.1f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);

                Star = new GenericSparkle(projectile.Center + lineVector, Vector2.Zero, Color.White, Color.PaleGoldenrod, Main.rand.NextFloat(1f, 1.5f), 20, 0.1f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);

                Main.PlaySound(SoundID.Item84, projectile.Center);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/TrueArkoftheAncients");
            Texture2D glowmask = GetTexture("CalamityMod/Items/Weapons/Melee/TrueArkoftheAncientsGlow");

            float drawRotation = angle + MathHelper.PiOver4;
            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = Owner.Center + angle.ToRotationVector2() * 30 - Main.screenPosition;


            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            spriteBatch.Draw(glowmask, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, 0f, 0f);




            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = GetTexture("CalamityMod/Particles/BloomLine");
            float rot = angle + MathHelper.PiOver2;
            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height);

            float size = Timer / MaxTime > 0.5f ? (float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.2f + 0.8f : (float)Math.Sin(Timer / MaxTime * MathHelper.Pi);

            size *= 3;

            Vector2 scale = new Vector2(size, lenght / tex.Height);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.LightPink, rot, origin, scale, SpriteEffects.None, 0);


            Texture2D cap = GetTexture("CalamityMod/Particles/BloomLineCap");
            scale = new Vector2(size, size);
            origin = new Vector2(cap.Width / 2f, cap.Height);

            spriteBatch.Draw(cap, projectile.Center - Main.screenPosition, null, Color.LightPink, rot + MathHelper.Pi, origin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(cap, projectile.Center + lineVector - Main.screenPosition, null, Color.LightPink, rot, origin, scale, SpriteEffects.None, 0);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}