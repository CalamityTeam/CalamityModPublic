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
using Terraria.Audio;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Melee
{
    public class SanguineFuryWheel : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0];
        public float ShredRatio => MathHelper.Clamp(Shred / (SanguineFury.maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[projectile.owner];

        public float Timer => MaxTime - projectile.timeLeft;

        public const float MaxTime = 120;



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanguine Bladewheel");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 45;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;


            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= 0.95f;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 84 * projectile.scale;
            float bladeWidth = 76 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - direction * bladeLenght / 2, projectile.Center + direction * bladeLenght / 2, bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized)
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                initialized = true;

                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                projectile.timeLeft = (int)MaxTime;
                projectile.velocity = direction * 16f;

                projectile.scale = 1f + ShredRatio; //SWAGGER
                projectile.netUpdate = true;

            }

            projectile.velocity *= 0.96f;
            projectile.position += projectile.velocity;

        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Owner.HeldItem.modItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.SuperPogoAttunement_WheelProc)
                sword.OnHitProc = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++) //Draw extra copies
            {
                var tex = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra");

                float drawAngleWheel = MathHelper.WrapAngle(i * MathHelper.PiOver2 + (Main.GlobalTime * 6));
                Vector2 drawOrigin = new Vector2(0f, tex.Height);
                Vector2 drawPositionWheel = projectile.Center - drawAngleWheel.ToRotationVector2() * 20f - Main.screenPosition;

                //Vector2 drawPosition = Vector2.Lerp(drawPositionShot, drawPositionWheel, transition);

                float opacityFade = projectile.timeLeft > 15 ? 1 : projectile.timeLeft / 15f;

                spriteBatch.Draw(tex, drawPositionWheel, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f * opacityFade, drawAngleWheel, drawOrigin, projectile.scale * 0.85f, 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item60, projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.Crimson : Color.DarkRed, 0.4f + Main.rand.NextFloat(0f, 3.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }
        }

    }
}