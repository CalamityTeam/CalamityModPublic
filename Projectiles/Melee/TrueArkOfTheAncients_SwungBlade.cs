using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Melee
{
    public class TrueArkoftheAncientsSwungBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TrueArkoftheAncients";

        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public float SwingDirection => Projectile.ai[0] * Math.Sign(direction.X);
        public ref float Charge => ref Projectile.ai[1];
        public ref float HasFired => ref Projectile.localAI[0];

        const float MaxTime = 35;
        private float SwingWidth = MathHelper.PiOver2 * 1.5f;
        public Vector2 DistanceFromPlayer => direction * 30;
        public float Timer => MaxTime - Projectile.timeLeft;
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = Projectile.height = 60;
            Projectile.width = Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = (int)MaxTime;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLength = 88f * Projectile.scale;
            Vector2 holdPoint = DistanceFromPlayer.Length() * Projectile.rotation.ToRotationVector2();

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + holdPoint, Owner.Center + holdPoint + Projectile.rotation.ToRotationVector2() * bladeLength, 24, ref collisionPoint);
        }


        //Swing animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.ExpOut, 0f, 0f, 0.15f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyInOut, 0.1f, 0.15f, 0.85f, 3);
        public CurveSegment hold = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0.2f);
        public CurveSegment retract = new CurveSegment(EasingType.PolyInOut, 0.7f, 0.9f, -0.9f, 3);
        internal float SwingRatio() => PiecewiseAnimation(Timer / MaxTime, new CurveSegment[] { anticipation, thrust, hold });

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                Projectile.timeLeft = (int)MaxTime;
                SoundStyle sound = (Charge > 0) ? CommonCalamitySounds.LouderPhantomPhoenix : SoundID.DD2_MonkStaffSwing;
                SoundEngine.PlaySound(sound, Projectile.Center);

                direction = Projectile.velocity;
                direction.Normalize();
                Projectile.rotation = direction.ToRotation();

                initialized = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            //Manage position and rotation
            Projectile.Center = Owner.Center + DistanceFromPlayer;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, SwingRatio());

            Projectile.scale = 1.4f + ((float)Math.Sin(SwingRatio() * MathHelper.Pi) * 0.6f) + (Charge / 10f) * 0.6f;

            if (Owner.whoAmI == Main.myPlayer && SwingRatio() > 0.5f && HasFired == 0f && Charge > 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + direction * 30f, Projectile.velocity * 2f, ProjectileType<TrueAncientBeam>(), (int)(Projectile.damage * TrueArkoftheAncients.beamDamageMultiplier), 2f, Owner.whoAmI) ;
                HasFired = 1f;
            }


            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
            Owner.itemRotation = Projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);

            if (Charge > 0 && Main.rand.Next(2) == 0)
            {
                Vector2 particleOrigin = Projectile.Center + Projectile.rotation.ToRotationVector2() * 85 * Projectile.scale;
                Vector2 particleSpeed = Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1.2f, 2f);
                Particle energyLeak = new CritSpark(particleOrigin, particleSpeed + Owner.velocity, Color.White, Color.Cyan, Main.rand.NextFloat(0.6f, 1.6f), 20 + Main.rand.Next(10), 0.1f, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D sword = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TrueArkoftheAncients").Value;
            Texture2D glowmask = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TrueArkoftheAncientsGlow").Value;

            SpriteEffects flip = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float extraAngle = Owner.direction < 0 ? MathHelper.PiOver2 : 0f;


            float drawAngle = Projectile.rotation;
            float drawRotation = Projectile.rotation + MathHelper.PiOver4 + extraAngle;
            Vector2 drawOrigin = new Vector2(Owner.direction < 0 ? sword.Width : 0f , sword.Height);
            Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

            if (CalamityConfig.Instance.Afterimages && Timer > ProjectileID.Sets.TrailCacheLength[Projectile.type])
            {
                for (int i = 0; i < Projectile.oldRot.Length; ++i)
                {
                    Color color = Main.hslToRgb((i / (float)Projectile.oldRot.Length) * 0.7f, 1, 0.6f + (Charge > 0 ? 0.3f : 0f));
                    float afterimageRotation = Projectile.oldRot[i] + MathHelper.PiOver4;
                    Main.EntitySpriteDraw(glowmask, drawOffset, null, color * 0.15f, afterimageRotation + extraAngle, drawOrigin, Projectile.scale - 0.2f * ((i / (float)Projectile.oldRot.Length)), flip, 0);
                }
            }

            Main.EntitySpriteDraw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, flip, 0);
            Main.EntitySpriteDraw(glowmask, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, Projectile.scale, flip, 0);

            if (Charge > 0 && Timer / MaxTime > 0.5f)
            {
                Texture2D smear = Request<Texture2D>("CalamityMod/Particles/TrientCircularSmear").Value;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                float opacity = (float)Math.Sin(Timer / MaxTime * MathHelper.Pi);
                float rotation = (-MathHelper.PiOver4 * 0.5f + MathHelper.PiOver4 * 0.5f * Timer / MaxTime) * SwingDirection;
                Color smearColor = Main.hslToRgb(((Timer - MaxTime * 0.5f ) / (MaxTime * 0.5f)) * 0.7f, 1, 0.6f);

                Main.EntitySpriteDraw(smear, Owner.Center - Main.screenPosition, null, smearColor * 0.5f * opacity, Projectile.velocity.ToRotation() + MathHelper.Pi + rotation, smear.Size() / 2f, Projectile.scale * 1.4f, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
        }
    }
}
