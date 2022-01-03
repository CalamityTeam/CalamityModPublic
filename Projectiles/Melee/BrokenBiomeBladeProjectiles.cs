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

namespace CalamityMod.Projectiles.Melee
{
    public class PurityProjection : ModProjectile //The boring plain one
    {
        private int dustType = 3;

        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_PurityProjection";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purity Projection");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.aiStyle = 27;
            aiType = ProjectileID.LightBeam;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI() //Dust doesnt look great
        {
            if (projectile.timeLeft < 95)
                projectile.tileCollide = true;
            int dustParticle = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.9f);
            Main.dust[dustParticle].noGravity = true;
            Main.dust[dustParticle].velocity *= 0.5f;
            Main.dust[dustParticle].velocity += projectile.velocity * 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 295)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            //Explode into dust or whatever,
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int debuffTime = 90;
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), debuffTime);
        }
    }

    public class DecaysRetort : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_DecaysRetort";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float MaxTime => ref projectile.ai[0];
        public ref float CanLunge => ref projectile.ai[1];
        public float Timer => MaxTime - projectile.timeLeft;
        public Player Owner => Main.player[projectile.owner];
        public const float LungeSpeed = 16;
        public ref float CanBounce => ref projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Decay's retort");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 84;
            projectile.width = projectile.height = 84;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 120f * projectile.scale;
            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace + (direction * bladeLenght), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                CanBounce = 1f;
                projectile.timeLeft = (int)MaxTime;
                //Take the direction the sword is swung. FUCK not controlling the swing direction more than just left/right :|
                //The direction to mouseworld may need to be turned into the custom synced player mouse variables . not on the branch currently tho
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();
                if (CanLunge == 1f)
                    Lunge();
                Main.PlaySound(SoundID.Item103, projectile.Center);
                initialized = true;
            }

            //Manage position and rotation
            projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER
            projectile.Center = Owner.Center + (direction * ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 60));

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }

        public void Lunge()
        {
            if (Main.myPlayer != projectile.owner)
                return;
            //Check if the owner is on the ground
            Owner.velocity = direction.SafeNormalize(Vector2.UnitX * Owner.direction) * LungeSpeed;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => OnHitEffects(!target.canGhostHeal || Main.player[projectile.owner].moonLeech);
        public override void OnHitPvp(Player target, int damage, bool crit) => OnHitEffects(Main.player[projectile.owner].moonLeech);

        private void OnHitEffects(bool cannotLifesteal)
        {
            if (!cannotLifesteal) //trolled
            {
                Owner.statLife += 3;
                Owner.HealEffect(3); //Idk if its too much or what but at the same time its close range as fuck
            }
            if (Main.myPlayer != Owner.whoAmI || CanBounce == 0f)
                return;
            // Bounce off
            float bounceStrength = Math.Max((LungeSpeed / 2f), Owner.velocity.Length());
            bounceStrength *= Owner.velocity.Y == 0 ? 0.2f : 1f; //Reduce the bounce if the player is on the ground 
            Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(bounceStrength, 0f, 22f);
            CanBounce = 0f;
            Owner.GiveIFrames(10); // 10 i frames for free!
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/BiomeBlade");
            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_DecaysRetort");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);
            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset + displace, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, tex.Height);
            drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 24f * projectile.scale) - Main.screenPosition;

            spriteBatch.Draw(tex, drawOffset + displace, null, lightColor * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

    public class BitingEmbrace : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbraceSmall";

        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public float rotation;
        public ref float SwingMode => ref projectile.ai[0]; //0 = Up-Down small slash, 1 = Down-Up large slash, 2 = Thrust
        public ref float MaxTime => ref projectile.ai[1];
        public float Timer => MaxTime - projectile.timeLeft;

        //Animation key constants
        public const float anticipationTime = 0.2f; //Get ready to thrust
        public const float thrustBurstTime = 0.15f; //Thrust in a quick burst
        public const float heldDownTime = 0.35f; //Remain extended
        public float retractTime => 1f - anticipationTime - thrustBurstTime - heldDownTime; //Retract. Its simply all the time left from the other stuff


        public int SwingDirection
        {
            get
            {
                switch (SwingMode)
                {
                    case 0:
                        return -1 * Math.Sign(direction.X);
                    case 1:
                        return 1 * Math.Sign(direction.X);
                    default:
                        return 0;

                }
            }
        }
        public float SwingWidth
        {
            get
            {
                switch (SwingMode)
                {
                    case 0:
                        return 2.3f;
                    default:
                        return 1.8f;
                }
            }
        }

        public Player Owner => Main.player[projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biting Embrace");
            Main.projFrames[projectile.type] = 6; //The true trolling is that we only really use this for the third swing.
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 75;
            projectile.width = projectile.height = 75;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLenght = 0f;
            Vector2 displace = Vector2.Zero;
            switch (SwingMode)
            {
                case 0:
                    bladeLenght = 90f * projectile.scale;
                    break;
                case 1:
                    bladeLenght = 110f * projectile.scale;
                    break;
                case 2:
                    bladeLenght = projectile.frame <= 2 ? 85f : 150f; //Only use the extended hitbox after the blade actually extends. For realism.
                    bladeLenght *= projectile.scale;
                    displace = direction * ThrustDisplaceRatio() * 60f;
                    break;

            }
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace + (rotation.ToRotationVector2() * bladeLenght), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                projectile.timeLeft = (int)MaxTime;
                switch (SwingMode)
                {
                    case 0:
                        projectile.width = projectile.height = 100;
                        Main.PlaySound(SoundID.DD2_MonkStaffSwing, projectile.Center);
                        projectile.damage = (int)(projectile.damage * 1.5);
                        break;
                    case 1:
                        projectile.width = projectile.height = 140;
                        Main.PlaySound(SoundID.DD2_OgreSpit, projectile.Center);
                        projectile.damage = (int)(projectile.damage * 1.8);
                        break;
                    case 2:
                        projectile.width = projectile.height = 130;
                        Main.PlaySound(SoundID.DD2_PhantomPhoenixShot, projectile.Center);
                        projectile.damage *= 3;
                        break;
                }

                //Take the direction the sword is swung. FUCK not controlling the swing direction more than just left/right :|
                //The direction to mouseworld may need to be turned into the custom synced player mouse variables . not on the branch currently tho
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                initialized = true;
            }
            //Manage position and rotation
            projectile.Center = Owner.Center + (direction * 30);
            //rotation = projectile.rotation + MathHelper.SmoothStep(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, Timer / MaxTime); 
            float factor = 1 - (float)Math.Pow((double)-(Timer / MaxTime) + 1, 2d);
            rotation = projectile.rotation + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, factor);
            projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER

            //Add the thrust motion & animation for the third combo state
            if (SwingMode == 2)
            {
                projectile.scale = 1f + (ThrustScaleRatio() * 0.6f);
                projectile.Center = Owner.Center + (direction * ThrustDisplaceRatio() * 60);

                projectile.frameCounter++;
                if (projectile.frameCounter % 5 == 0 && projectile.frame + 1 < Main.projFrames[projectile.type])
                    projectile.frame++;

            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(rotation.ToRotationVector2().X);
            Owner.itemRotation = rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }

        internal float ThrustDisplaceRatio()
        {
            float linearRatio = (Timer / MaxTime); //How far along the animation are you
            float ratio; //L + Ratio
            float displaceRatio = 1f;

            if (linearRatio <= anticipationTime) //Anticipation phase. A small bump in the negatives
            {
                ratio = MathHelper.Lerp(0f, 1f, linearRatio / anticipationTime); //Turn that mf into a workable 0 - 1 range
                displaceRatio = (float)(Math.Sin(ratio * MathHelper.Pi) * -0.15f);
            }
            else if (linearRatio <= anticipationTime + thrustBurstTime) //Burst phase. Quickly step into almost a full ratio
            {
                ratio = linearRatio - anticipationTime;
                ratio = MathHelper.Lerp(0f, 1f, ratio / thrustBurstTime); //Turn that mf into a workable 0 - 1 range
                displaceRatio = MathHelper.SmoothStep(0f, 0.9f, ratio);
            }
            else if (linearRatio <= anticipationTime + thrustBurstTime + heldDownTime) //Holding down phase. Small bump up to reach a full ratio and then start going down
            {
                ratio = linearRatio - anticipationTime - thrustBurstTime;
                ratio = MathHelper.Lerp(0f, 1f, ratio / heldDownTime); //Turn that mf into a workable 0 - 1 range
                displaceRatio = 0.9f + (float)(Math.Sin(ratio * MathHelper.Pi) * 0.1f);
            }
            else //Retraction phase. Smooth step back to 0
            {
                ratio = linearRatio - (1f - retractTime);
                ratio = MathHelper.Lerp(0f, 1f, ratio / retractTime); //Turn that mf into a workable 0 - 1 range
                displaceRatio = MathHelper.SmoothStep(0.9f, 0, ratio);
            }

            return displaceRatio;
        }

        //The ratio rapidly increases from 0 to 1 (reaches 1 at the midway point of the anticipation.). It remains at 1 for the rest of the thrust, and then decreases down to 0 at the midway point of the retraction)
        internal float ThrustScaleRatio()
        {
            float linearRatio = (Timer / MaxTime); //How far along the animation are you
            float ratio; //L + Ratio
            float scaleRatio = 1f;

            if (linearRatio <= anticipationTime / 2f) //Growth phase
            {
                ratio = MathHelper.Lerp(0f, 1f, linearRatio / (anticipationTime / 2f)); //Turn that mf into a workable 0 - 1 range
                scaleRatio = (float)(1f - Math.Exp(-ratio + 1));
            }
            if (linearRatio >= 1f - retractTime / 2f) //Retract phase
            {
                ratio = linearRatio - (1f - retractTime / 2f);
                ratio = MathHelper.Lerp(0f, 1f, ratio / (retractTime / 2f)); //Turn that mf into a workable 0 - 1 range
                scaleRatio = (float)(1f - Math.Exp(ratio));
            }

            return scaleRatio;
        }

        internal float PrimitiveWidthFunction(float completionRatio)
        {
            var tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbrace" + (SwingMode == 0 ? "Small" : "Big"));
            return Vector2.Distance(Vector2.Zero, tex.Size());
        }

        internal Color PrimitiveColorFunction(float completionRatio)
        {
            float opacity = Utils.InverseLerp(0.8f, 0.52f, completionRatio, true) * Utils.InverseLerp(1f, 0.81f, Timer / MaxTime, true);
            Color startingColor = Color.White;
            Color endingColor = Color.Lerp(Color.DarkCyan, Color.DarkBlue, 0.77f);
            return Color.Lerp(startingColor, endingColor, (float)Math.Pow(completionRatio, 0.37f)) * opacity;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {


            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/BiomeBlade");

            if (SwingMode != 2)
            {
                Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbrace" + (SwingMode == 0 ? "Small" : "Big"));
                float drawAngle = rotation;
                float drawRotation = rotation + MathHelper.PiOver4;
                Vector2 drawOrigin = new Vector2(0f, handle.Height);
                Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

                spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Turn on additive blending
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Update the parameters
                drawOrigin = new Vector2(0f, tex.Height);
                drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 28f * projectile.scale) - Main.screenPosition;
                spriteBatch.Draw(tex, drawOffset, null, lightColor * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            else
            {
                Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbraceThrust");
                Vector2 thrustDisplace = direction * (ThrustDisplaceRatio() * 60);

                float drawAngle = rotation;
                float drawRotation = rotation + MathHelper.PiOver4;
                Vector2 drawOrigin = new Vector2(0f, handle.Height);
                Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

                spriteBatch.Draw(handle, drawOffset + thrustDisplace, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Turn on additive blending
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Update the parameters

                drawOrigin = new Vector2(0f, 114f);
                drawOffset = Owner.Center + (direction * 28f * projectile.scale) - Main.screenPosition;
                //Anim stuff
                Rectangle frameRectangle = new Rectangle(0, 0 + (116 * projectile.frame), 114, 114);

                spriteBatch.Draw(tex, drawOffset + thrustDisplace, frameRectangle, lightColor * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

    }

    public class AridGrandeur : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeur";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0]; //How much the attack is, attacking
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public ref float PogoCooldown => ref projectile.ai[1]; //Cooldown for the pogo
        public Player Owner => Main.player[projectile.owner];
        public bool CanPogo => Owner.velocity.Y != 0 && PogoCooldown <= 0; //Only pogo when in the air and if the cooldown is zero
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public const float pogoStrenght = 16f; //How much the player gets pogoed up
        public const float maxShred = 500; //How much shred you get

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arid Grandeur");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 84 * projectile.scale;
            float bladeWidth = 76 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public void Pogo()
        {
            if (CanPogo && Main.myPlayer == Owner.whoAmI)
            {
                Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * pogoStrenght; //Bounce
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                PogoCooldown = 30; //Cooldown
                Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.position);

                if (Owner.HeldItem.type == ItemType<BiomeBlade>())
                    (Owner.HeldItem.modItem as BiomeBlade).CanLunge = 1; // Reset the lunge counter on pogo. This should make for more interesting and fun synergies
            }
        }

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                initialized = true;
            }

            if (!OwnerCanShoot)
            {
                projectile.Kill();
                return;
            }


            if (Shred >= maxShred)
                Shred = maxShred;
            if (Shred < 0)
                Shred = 0;

            //Manage position and rotation
            direction = Owner.DirectionTo(Main.MouseWorld);
            direction.Normalize();
            projectile.rotation = direction.ToRotation();
            projectile.Center = Owner.Center + (direction * 60);

            //Scaling based on shred
            projectile.localNPCHitCooldown = 16 - (int)(MathHelper.Lerp(0, 8, ShredRatio)); //Increase the hit frequency
            projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER


            if (Collision.SolidCollision(Owner.Center + (direction * 84 * projectile.scale) - Vector2.One * 5f, 10, 10))
            {
                Pogo();
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Shred--;
            PogoCooldown--;
            projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => ShredTarget(target);
        public override void OnHitPvp(Player target, int damage, bool crit) => ShredTarget(target);

        private void ShredTarget(Entity target)
        {
            Main.PlaySound(SoundID.NPCHit30, projectile.Center); //Sizzle
            Shred += 62; //Augment the shredspeed
            if (Main.myPlayer != Owner.whoAmI)
                return;
            // get lifted up
            if (PogoCooldown <= 0)
            {
                if (Owner.velocity.Y > 0)
                    Owner.velocity.Y = -2f; //Get "stuck" into the enemy partly

                Owner.GiveIFrames(5); // i framez. Do 5 iframes even matter? idk but you get a lot of em so lol...
                PogoCooldown = 20;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit43, projectile.Center);
            //if (ShredRatio > 0.5 && Owner.whoAmI == Main.myPlayer) //Keep this for the True biome blade/Repaired biome blade.
            //{
            //    Projectile.NewProjectile(projectile.Center, direction * 16f, ProjectileType<AridGrandeurShot>(), projectile.damage, projectile.knockBack, Owner.whoAmI, Shred);
            //sharticles
            //}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/BiomeBlade");
            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeur");

            int bladeAmount = 4;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, tex.Height);
            drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 32f * projectile.scale) - Main.screenPosition;

            spriteBatch.Draw(tex, drawOffset, null, lightColor * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);


            for (int i = 0; i < bladeAmount; i++) //Draw extra copies
            {
                tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeurExtra");

                drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 7f) * ShredRatio);

                drawOrigin = new Vector2(0f, tex.Height);


                Vector2 drawOffsetStraight = Owner.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                spriteBatch.Draw(tex, drawOffsetStraight + drawDisplacementAngle, null, lightColor * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

    public class AridGrandeurShot : ModProjectile //Only use this for the upgrade actually lol
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeurExtra";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0];
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[projectile.owner];

        public const float pogoStrenght = 16f; //How much the player gets pogoed up
        public const float maxShred = 500; //How much shred you get

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arid Shredder");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
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
                projectile.timeLeft = (int)(30f + ShredRatio * 30f);
                initialized = true;

                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                projectile.velocity = direction * 6f;
                projectile.damage *= 10;

                projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER
                projectile.netUpdate = true;

            }

            projectile.position += projectile.velocity;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++) //Draw extra copies
            {
                var tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeurExtra");

                float drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                float drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 7f) * ShredRatio);

                Vector2 drawOrigin = new Vector2(0f, tex.Height);


                Vector2 drawOffsetStraight = projectile.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                spriteBatch.Draw(tex, drawOffsetStraight + drawDisplacementAngle, null, lightColor * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

    public class GrovetendersTouch : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchBlade";
        private bool initialized = false;

        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[projectile.owner];
        public float Timer => MaxTime - projectile.timeLeft;
        public ref float HasSnapped => ref projectile.ai[0];
        public ref float SnapCoyoteTime => ref projectile.ai[1];

        const float MaxTime = 90;
        const int coyoteTimeFrames = 15; //How many frames does the whip stay extended 
        const int MaxReach = 400;
        const float SnappingPoint = 0.7f; //When does the snap occur.
        const float ReelBackStrenght = 14f;
        const float ChainDamageReduction = 0.5f;

        public BezierCurve curve;
        private Vector2 controlPoint1;
        private Vector2 controlPoint2;
        internal bool ReelingBack => Timer / MaxTime > SnappingPoint;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grovetender's Touch");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 36;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override bool? CanCutTiles() => false; //Itd be quite counterproductive to make the whip cut the tiles it just grew

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, controlPoint1, controlPoint2, projectile.Center });

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

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);
            Vector2 projectileHalfLenght = (projectile.Size / 2f) * projectile.rotation.ToRotationVector2();
            float collisionPoint = 0;
            //If you hit the enemy during the coyote time with the blade of the whip, guarantee a crit
            if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), projectile.Center - projectileHalfLenght, projectile.Center + projectileHalfLenght, 32, ref collisionPoint))
            {
                if (SnapCoyoteTime > 0f)
                    crit = true;
            }
            else
                damage = (int)(damage * ChainDamageReduction); //If the enemy is hit with the chain of the whip, the damage gets reduced
        }

        public override void AI()
        {
            if (!initialized) //Initialization. create control points & shit)
            {
                projectile.velocity = Owner.DirectionTo(Main.MouseWorld);
                Main.PlaySound(SoundID.DD2_OgreSpit, projectile.Center);
                controlPoint1 = projectile.Center;
                controlPoint2 = projectile.Center;
                projectile.timeLeft = (int)MaxTime;
                initialized = true;
            }

            if (ReelingBack && HasSnapped == 0f) //Snap & also small coyote time for the hook
            {
                Main.PlaySound(SoundID.Item41, projectile.Center); //Snap
                HasSnapped = 1f;
                SnapCoyoteTime = coyoteTimeFrames;
            }

            if (SnapCoyoteTime > 0) //keep checking for the tile hook
            {
                HookToTile();
                SnapCoyoteTime--;
            }

            Owner.direction = Math.Sign(projectile.velocity.X);
            projectile.rotation = projectile.AngleFrom(Owner.Center); //Point away from playah

            float ratio = GetSwingRatio();
            projectile.Center = Owner.MountedCenter + SwingPosition(ratio);
            projectile.direction = projectile.spriteDirection = -Owner.direction;

            //MessWithTiles(); 

            Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Main.MouseWorld) - (Owner.direction < 0 ? MathHelper.Pi : 0));
        }
        public void HookToTile()
        {
            if (Main.myPlayer == Owner.whoAmI)
            {
                //Shmoove the player if a tile is hit. This movement always happens if the owner isnt on the ground, but will only happen if the projectile is above the player if they are standing on the ground)
                if (Collision.SolidCollision(projectile.position, 32, 32) && (Owner.velocity.Y != 0 || projectile.position.Y < Owner.position.Y))
                {
                    Owner.velocity = Owner.DirectionTo(projectile.Center) * ReelBackStrenght;
                    SnapCoyoteTime = 0f;
                }
                Main.PlaySound(SoundID.Item65, projectile.position);
            }
        }

        public void MessWithTiles() //This DESTROYS the grass growing meta and shall remain sealed away
        {
            BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, controlPoint1, controlPoint2, projectile.Center });
            int numPoints = 30;
            Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();

            for (int i = 0; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                Tile targetTile = Main.tile[(int)position.X / 16, (int)position.Y / 16]; //Grab the tile

                if ((targetTile.type == TileID.Dirt || targetTile.type == TileID.Mud) && targetTile.active())
                {
                    if (targetTile.type == TileID.Dirt)
                    {
                        targetTile.active(true);
                        targetTile.type = TileID.Grass;

                        if (Owner.ZoneCorrupt)
                            targetTile.type = TileID.CorruptGrass;
                        if (Owner.ZoneCrimson)
                            targetTile.type = TileID.FleshGrass;
                        if (Owner.ZoneHoly)
                            targetTile.type = TileID.HallowedGrass;
                    }
                    if (targetTile.type == TileID.Mud)
                    {
                        targetTile.active(true);
                        targetTile.type = TileID.JungleGrass;
                        if (Owner.ZoneGlowshroom)
                        {
                            targetTile.type = TileID.MushroomGrass;
                        }
                    }

                    WorldGen.SquareTileFrame((int)position.X / 16, (int)position.Y / 16, true); //Update the tile framing
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendTileSquare(-1, (int)position.X / 16, (int)position.Y / 16, 1, TileChangeType.None);
                }

                Tile targetTileAbove = Main.tile[(int)position.X / 16, (int)position.Y / 16 - 1];
                if (!targetTileAbove.active() && ((targetTile.type == TileID.Grass || targetTile.type == TileID.HallowedGrass || targetTile.type == TileID.JungleGrass) && targetTile.active())) //If theres air above
                {
                    if (targetTile.type == TileID.Grass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            targetTileAbove.active(true);
                            targetTileAbove.type = TileID.Plants;
                            targetTileAbove.frameX = (short)(18 * Main.rand.Next(0, 11));
                            while (targetTileAbove.frameX == 144) //Exclude the mushroom
                            {
                                targetTileAbove.frameX = (short)(18 * Main.rand.Next(0, 11));
                            }
                        }
                        else
                        {
                            targetTileAbove.active(true);
                            targetTileAbove.type = TileID.Plants2;
                            targetTileAbove.frameX = (short)(18 * Main.rand.Next(0, 21));
                            while (targetTileAbove.frameX == 144)
                            {
                                targetTileAbove.frameX = (short)(18 * Main.rand.Next(0, 21));
                            }
                        }
                    }
                    //On hallowed grass, grow hallowed plants
                    else if (targetTile.type == TileID.Grass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            targetTileAbove.active(true);
                            targetTileAbove.type = TileID.HallowedPlants;
                            targetTileAbove.frameX = (short)(18 * Main.rand.Next(0, 7));
                        }
                        else
                        {
                            targetTileAbove.active(true);
                            targetTileAbove.type = TileID.HallowedPlants2;
                            targetTileAbove.frameX = (short)(18 * Main.rand.Next(0, 8));
                            while (targetTileAbove.frameX == 90)
                            {
                                targetTileAbove.frameX = (short)(18 * Main.rand.Next(0, 8));
                            }
                        }
                    }
                    //On jungle grass, grow jungle flowers
                    else if (targetTile.type == TileID.JungleGrass)
                    {
                        targetTileAbove.active(true);
                        targetTileAbove.type = TileID.JunglePlants2;
                        targetTileAbove.frameX = (short)(18 * Main.rand.Next(0, 17));
                    }

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendTileSquare(-1, (int)position.X / 16, (int)position.Y / 16 - 1, 1, TileChangeType.None);
                }

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
                float distance = MaxReach * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.PiOver2), 1, 0.04f); //half arc
                distance = Math.Max(distance, 65); //Dont be too close to player

                float angleDeviation = MathHelper.Pi / 1.2f;
                float angleOffset = Owner.direction * MathHelper.Lerp(-angleDeviation, 0, progress); //Go from very angled to straight at the zenith of the attack
                return projectile.velocity.RotatedBy(angleOffset) * distance;
            }
            else
            {
                float distance = MathHelper.Lerp(MaxReach, 0f, progress); //Quickly zip back to the player . No angles or minimum distance from player.
                return projectile.velocity * distance;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Timer == 0)
                return false;
            Texture2D handle = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchGlow");

            Vector2 projBottom = projectile.Center + new Vector2(-handle.Width / 2, handle.Height / 2).RotatedBy(projectile.rotation + MathHelper.PiOver4) * 0.75f;
            DrawChain(spriteBatch, projBottom, out Vector2[] chainPositions);

            float drawRotation = (projBottom - chainPositions[chainPositions.Length - 2]).ToRotation() + MathHelper.PiOver4; //Face away from the last point of the bezier curve
            drawRotation += SnapCoyoteTime > 0 ? MathHelper.Pi : 0; //During coyote time the blade flips for some reason. Prevent that from happening
            drawRotation += projectile.spriteDirection < 0 ? 0f : 0f;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            SpriteEffects flip = (projectile.spriteDirection < 0) ? SpriteEffects.None : SpriteEffects.None;
            lightColor = Lighting.GetColor((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f));

            spriteBatch.Draw(handle, projBottom - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, flip, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Only update the origin for once
            drawOrigin = new Vector2(0f, blade.Height);
            spriteBatch.Draw(blade, projBottom - Main.screenPosition, null, lightColor * 0.9f, drawRotation, drawOrigin, projectile.scale, flip, 0f);
            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        private void DrawChain(SpriteBatch spriteBatch, Vector2 projBottom, out Vector2[] chainPositions)
        {
            Texture2D chainTex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchChain");

            float ratio = GetSwingRatio();

            if (!ReelingBack) //Make the curve be formed from points slightly ahead of the projectile, but clamped to the max rotation (straight line ahead)
            {
                controlPoint1 = Owner.MountedCenter + SwingPosition(MathHelper.Clamp(ratio + 0.5f, 0f, 1f)) * 0.5f;
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
                spriteBatch.Draw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }
    }
}
