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
    public class ArkoftheAncientsSwungBlade : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/ArkoftheAncients";

        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public float SwingDirection => projectile.ai[0] * Math.Sign(direction.X);
        public ref float Charge => ref projectile.ai[1];
        public ref float HasFired => ref projectile.localAI[0];

        const float MaxTime = 35;
        private float SwingWidth = MathHelper.PiOver2 * 1.5f;
        public Vector2 DistanceFromPlayer => direction * 30;
        public float Timer => MaxTime - projectile.timeLeft;
        public Player Owner => Main.player[projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fractured Ark");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 60;
            projectile.width = projectile.height = 60;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = (int)MaxTime;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLenght = 78f * projectile.scale;
            Vector2 holdPoint = DistanceFromPlayer.Length() * projectile.rotation.ToRotationVector2();

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + holdPoint, Owner.Center + holdPoint + projectile.rotation.ToRotationVector2() * bladeLenght, 24, ref collisionPoint);
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
                projectile.timeLeft = (int)MaxTime;
                var sound = Main.PlaySound(Charge > 0 ? SoundID.DD2_PhantomPhoenixShot : SoundID.DD2_MonkStaffSwing, projectile.Center);
                if (Charge > 0)
                    CalamityUtils.SafeVolumeChange(ref sound, 2.5f);
                direction = projectile.velocity;
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            //Manage position and rotation
            projectile.Center = Owner.Center + DistanceFromPlayer;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, SwingRatio());

            projectile.scale = 1.4f + ((float)Math.Sin(SwingRatio() * MathHelper.Pi) * 0.6f) + (Charge / 10f) * 0.6f; 

            if (Owner.whoAmI == Main.myPlayer && SwingRatio() > 0.5f && HasFired == 0f && Charge > 0)
            {
                Projectile.NewProjectile(Owner.Center + direction * 30f, projectile.velocity * 1.25f, ProjectileType<AncientBeam>(), (int)(projectile.damage * ArkoftheAncients.beamDamageMultiplier), 2f, Owner.whoAmI) ;
                HasFired = 1f;
            }


            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(projectile.velocity.X);
            Owner.itemRotation = projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);

            if (Charge > 0 && Main.rand.Next(2) == 0)
            {
                Vector2 particleOrigin = projectile.Center + projectile.rotation.ToRotationVector2() * 75 * projectile.scale;
                Vector2 particleSpeed = projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1.2f, 2f);
                Particle energyLeak = new CritSpark(particleOrigin, particleSpeed + Owner.velocity, Color.White, Color.Cyan, Main.rand.NextFloat(0.6f, 1.6f), 20 + Main.rand.Next(10), 0.1f, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheAncients");
            Texture2D glowmask = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheAncientsGlow");

            SpriteEffects flip = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float extraAngle = Owner.direction < 0 ? MathHelper.PiOver2 : 0f;


            float drawAngle = projectile.rotation;
            float drawRotation = projectile.rotation + MathHelper.PiOver4 + extraAngle;
            Vector2 drawOrigin = new Vector2(Owner.direction < 0 ? sword.Width : 0f , sword.Height);
            Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

            if (CalamityConfig.Instance.Afterimages && Timer > ProjectileID.Sets.TrailCacheLength[projectile.type]) 
            {
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                {
                    Color color = Main.hslToRgb((i / (float)projectile.oldRot.Length) * 0.7f, 1, 0.6f + (Charge > 0 ? 0.3f : 0f));
                    float afterimageRotation = projectile.oldRot[i] + MathHelper.PiOver4;
                    spriteBatch.Draw(glowmask, drawOffset, null, color * 0.15f, afterimageRotation + extraAngle, drawOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), flip, 0f);
                }
            }

            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, flip, 0f);
            spriteBatch.Draw(glowmask, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, flip, 0f);

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