using CalamityMod.DataStructures;
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
using Terraria.Audio;


namespace CalamityMod.Projectiles.Melee
{
    public class GrovetendersTouch : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchBlade";
        private bool initialized = false;
        public Player Owner => Main.player[Projectile.owner];
        public float Timer => MaxTime - Projectile.timeLeft;
        public ref float HasSnapped => ref Projectile.ai[0];
        public ref float SnapCoyoteTime => ref Projectile.ai[1];
        public ref float Reach => ref Projectile.localAI[0];

        const float MaxTime = 90;
        const int coyoteTimeFrames = 15; //How many frames does the whip stay extended
        const int MaxReach = 400;
        const int MinReach = 300;
        const float SnappingPoint = 0.7f; //When does the snap occur.
        const float ReelBackStrenght = 14f;

        public BezierCurve curve;
        private Vector2 controlPoint1;
        private Vector2 controlPoint2;
        internal bool ReelingBack => Timer / MaxTime > SnappingPoint;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = BrokenBiomeBlade.TropicalAttunement_LocalIFrames;
        }

        public override bool? CanCutTiles() => false; //Itd be quite counterproductive to make the whip cut the tiles it just grew

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, controlPoint1, controlPoint2, Projectile.Center });

            int numPoints = 32;
            Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();
            float collisionPoint = 0;
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                Vector2 previousPosition = chainPositions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            Vector2 projectileHalfLength = (Projectile.Size / 2f) * Projectile.rotation.ToRotationVector2();
            float collisionPoint = 0;
            //If you hit the enemy during the coyote time with the blade of the whip, guarantee a crit & get some bonus damage
            if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), Projectile.Center - projectileHalfLength, Projectile.Center + projectileHalfLength, 32, ref collisionPoint))
            {
                if (SnapCoyoteTime > 0f)
                {
                    modifiers.SourceDamage *= BrokenBiomeBlade.TropicalAttunement_SweetSpotDamageMultiplier;
                    modifiers.SetCrit();
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 sparkSpeed = Owner.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 9f;
                        Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.LimeGreen, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                        GeneralParticleHandler.SpawnParticle(Spark);
                    }
                }
            }
            else
                modifiers.SourceDamage *= BrokenBiomeBlade.TropicalAttunement_ChainDamageReduction; //If the enemy is hit with the chain of the whip, the damage gets reduced
        }

        public override void AI()
        {
            if (!initialized) //Initialization. create control points & shit)
            {
                Projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                Reach = MathHelper.Clamp((Owner.Center - Owner.Calamity().mouseWorld).Length(), MinReach, MaxReach);
                SoundEngine.PlaySound(SoundID.DD2_OgreSpit, Projectile.Center);
                controlPoint1 = Projectile.Center;
                controlPoint2 = Projectile.Center;
                Projectile.timeLeft = (int)MaxTime;
                initialized = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            if (ReelingBack && HasSnapped == 0f) //Snap & also small coyote time for the hook
            {
                SoundEngine.PlaySound(SoundID.Item41, Projectile.Center); //Snap
                HasSnapped = 1f;
                SnapCoyoteTime = coyoteTimeFrames;
            }

            if (SnapCoyoteTime > 0) //keep checking for the tile hook
            {
                Lighting.AddLight(Projectile.Center, 0.8f, 1f, 0.35f);
                HookToTile();
                SnapCoyoteTime--;
            }

            Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
            Projectile.rotation = Projectile.AngleFrom(Owner.Center); //Point away from playah

            float ratio = GetSwingRatio();
            Projectile.Center = Owner.MountedCenter + SwingPosition(ratio);
            Projectile.direction = Projectile.spriteDirection = -Owner.direction;

            Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Owner.Calamity().mouseWorld) - (Owner.direction < 0 ? MathHelper.Pi : 0));
        }
        public void HookToTile()
        {
            if (Main.myPlayer == Owner.whoAmI)
            {
                //Shmoove the player if a tile is hit. This movement always happens if the owner isnt on the ground, but will only happen if the projectile is above the player if they are standing on the ground)
                if (Collision.SolidCollision(Projectile.position, 32, 32) && (Owner.velocity.Y != 0 || Projectile.position.Y < Owner.position.Y))
                {
                    Owner.velocity = Owner.SafeDirectionTo(Projectile.Center, Vector2.Zero) * ReelBackStrenght;
                    SnapCoyoteTime = 0f;
                }
                SoundEngine.PlaySound(SoundID.Item65, Projectile.position);
            }
        }

        internal float EaseInFunction(float progress) => progress == 0 ? 0f : (float)Math.Pow(2, 10 * progress - 10); //Potion seller i need your strongest easeIns

        private float GetSwingRatio()
        {
            float ratio = (Timer - SnappingPoint * MaxTime) / (MaxTime * (1 - SnappingPoint)); //The ratio for the last section of the snap is a new curve.
            if (!ReelingBack)
                ratio = EaseInFunction(Timer / (MaxTime * SnappingPoint));  //Watch this ratio get eased
            if (SnapCoyoteTime > 0)
                ratio = 0f;
            return ratio;
        }

        private Vector2 SwingPosition(float progress)
        {
            //Whip windup and snap part
            if (!ReelingBack)
            {
                float distance = Reach * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.PiOver2), 1, 0.04f); //half arc
                distance = Math.Max(distance, 65); //Dont be too close to player

                float angleDeviation = MathHelper.Pi / 1.2f;
                float angleOffset = Owner.direction * MathHelper.Lerp(-angleDeviation, 0, progress); //Go from very angled to straight at the zenith of the attack
                return Projectile.velocity.RotatedBy(angleOffset) * distance;
            }
            else
            {
                float distance = MathHelper.Lerp(Reach, 0f, progress); //Quickly zip back to the player . No angles or minimum distance from player.
                return Projectile.velocity * distance;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer == 0)
                return false;
            Texture2D handle = Request<Texture2D>("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchBlade").Value;
            Texture2D blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchGlow").Value;

            Vector2 projBottom = Projectile.Center + new Vector2(-handle.Width / 2, handle.Height / 2).RotatedBy(Projectile.rotation + MathHelper.PiOver4) * 0.75f;
            DrawChain(projBottom, out Vector2[] chainPositions);

            float drawRotation = (projBottom - chainPositions[^2]).ToRotation() + MathHelper.PiOver4; //Face away from the last point of the bezier curve
            drawRotation += SnapCoyoteTime > 0 ? MathHelper.Pi : 0; //During coyote time the blade flips for some reason. Prevent that from happening
            drawRotation += Projectile.spriteDirection < 0 ? 0f : 0f;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            SpriteEffects flip = (Projectile.spriteDirection < 0) ? SpriteEffects.None : SpriteEffects.None;
            lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));

            Main.EntitySpriteDraw(handle, projBottom - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, Projectile.scale, flip, 0);

            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Only update the origin for once
            drawOrigin = new Vector2(0f, blade.Height);
            Main.EntitySpriteDraw(blade, projBottom - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, flip, 0);
            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        private void DrawChain(Vector2 projBottom, out Vector2[] chainPositions)
        {
            Texture2D chainTex = Request<Texture2D>("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchChain").Value;

            float ratio = GetSwingRatio();

            if (!ReelingBack) //Make the curve be formed from points slightly ahead of the projectile, but clamped to the max rotation (straight line ahead)
            {
                controlPoint1 = Owner.MountedCenter + SwingPosition(MathHelper.Clamp(ratio + 0.5f, 0f, 1f)) * 0.2f;
                controlPoint2 = Owner.MountedCenter + SwingPosition(MathHelper.Clamp(ratio + 0.2f, 0f, 1f)) * 0.5f;
            }
            else //After the whip snaps, make the curve be a wave
            {
                Vector2 perpendicular = SwingPosition(ratio).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
                controlPoint1 = Owner.MountedCenter + SwingPosition(MathHelper.Lerp(ratio, 1f, ratio)) + perpendicular * MathHelper.SmoothStep(0f, 1f, ratio) * -155f;
                controlPoint2 = Owner.MountedCenter + SwingPosition(MathHelper.Lerp(ratio, 1f, ratio / 2)) + perpendicular * MathHelper.SmoothStep(0f, 1f, ratio) * 100f;
            }

            BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, controlPoint1, controlPoint2, projBottom });
            int numPoints = 30; //"Should make dynamic based on curve length, but I'm not sure how to smoothly do that while using a bezier curve" -Graydee, from the code i referenced. I do agree.
            chainPositions = curve.GetPoints(numPoints).ToArray();

            //Draw each chain segment bar the very first one
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                if (ReelingBack)
                    chainLightColor *= 1 - EaseInFunction(ratio); //Make the chain fade when reeling it back
                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                Main.EntitySpriteDraw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.Write(Reach);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            Reach = reader.ReadSingle();
        }
    }
}
