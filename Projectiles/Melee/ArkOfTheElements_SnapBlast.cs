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
    public class ArkoftheElementsSnapBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/RendingScissorsRight";

        private bool initialized = false;

        const float MaxTime = 60;
        const float SnapTime = 25f;
        const float HoldTime = 15f;

        public float SnapTimer => MaxTime - projectile.timeLeft;
        public float HoldTimer => MaxTime - projectile.timeLeft - SnapTime;
        public float StitchTimer => MaxTime - projectile.timeLeft - SnapTime - HoldTime;

        public float SnapProgress => MathHelper.Clamp(SnapTimer / SnapTime, 0, 1);
        public float HoldProgress => MathHelper.Clamp(HoldTimer / HoldTime, 0, 1);
        public float StitchProgress => MathHelper.Clamp(StitchTimer / (MaxTime - (SnapTime + HoldTime)), 0, 1);

        public int CurrentAnimation => (MaxTime - projectile.timeLeft) <= SnapTime ? 0 : (MaxTime - projectile.timeLeft) <= SnapTime + HoldTime ? 1 : 2;

        public Player Owner => Main.player[projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rending Scissors");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 300;
            projectile.width = projectile.height = 300;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLenght = 142f * projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + (projectile.velocity * bladeLenght), 64, ref collisionPoint);
        }


        public override void AI()
        {
            if (!initialized) //Initialization
            {
                projectile.timeLeft = (int)MaxTime;
                var sound = Main.PlaySound(SoundID.Item84, projectile.Center);
                SafeVolumeChange(ref sound, 0.3f);

                projectile.velocity.Normalize();
                projectile.rotation = projectile.velocity.ToRotation();


                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            //Manage position and rotation
            projectile.Center -= projectile.velocity;
            projectile.scale = 1.4f;

        }

        //Animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.SineBump, 0f, 0.2f, -0.1f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyOut, 0.3f, 0.2f, 3f, 3);
        internal float ThrustDisplaceRatio() => PiecewiseAnimation(SnapProgress, new CurveSegment[] { anticipation, thrust });


        public CurveSegment openMore = new CurveSegment(EasingType.SineBump, 0f, 0f, -0.15f);
        public CurveSegment close = new CurveSegment(EasingType.PolyIn, 0.35f, 0f, 1f, 4);
        public CurveSegment stayClosed = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0f);
        internal float RotationRatio() => PiecewiseAnimation(SnapProgress, new CurveSegment[] { openMore, close, stayClosed });



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            if (HoldProgress <= 0.4f)
            {
                Texture2D frontBlade = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRight");
                Texture2D backBlade = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeft");

                float snippingRotation = projectile.rotation + MathHelper.PiOver4;
                float snippingRotationBack = projectile.rotation + MathHelper.PiOver4 * 1.75f;

                float drawRotation = MathHelper.Lerp(snippingRotation + MathHelper.PiOver4, snippingRotation, RotationRatio());
                float drawRotationBack = MathHelper.Lerp(snippingRotationBack - MathHelper.PiOver4, snippingRotationBack, RotationRatio());

                Vector2 drawOrigin = new Vector2(51, 86); //Right on the hole
                Vector2 drawOriginBack = new Vector2(22, 109); //Right on the hole
                Vector2 drawPosition = projectile.Center + ThrustDisplaceRatio() * projectile.velocity * 200f - Main.screenPosition;


                float opacity = (0.4f - HoldProgress) / 0.4f;

                Color drawColor = Color.CornflowerBlue * opacity * 0.5f;

                spriteBatch.Draw(backBlade, drawPosition, null, drawColor, drawRotationBack, drawOriginBack, projectile.scale, 0f, 0f);

                spriteBatch.Draw(frontBlade, drawPosition, null, drawColor * opacity, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
        }
    }
}