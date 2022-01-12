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
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class TruePurityProjection : ModProjectile //The boring plain one. I need to find something different to make it do compared to the original one.
    {
        public NPC target;
        public Player Owner => Main.player[projectile.owner];
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
            projectile.timeLeft = 40;
            projectile.extraUpdates = 1;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {

            if (target == null)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == Owner.whoAmI)
                    {
                        target = Main.npc[(int)proj.ai[0]];
                        break;
                    }
                }
            }
            else if ((projectile.Center - target.Center).Length() >= (projectile.Center + projectile.velocity - target.Center).Length() && CalamityUtils.AngleBetween(projectile.velocity, target.Center - projectile.Center) < MathHelper.PiOver4 * 0.5f) //Home in
            {
                projectile.timeLeft = 30; //Remain alive
                float angularTurnSpeed = MathHelper.ToRadians(MathHelper.Lerp(12.5f, 2.5f, MathHelper.Clamp(projectile.Distance(target.Center) / 10f, 0f, 1f)));
                float idealDirection = projectile.AngleTo(target.Center);
                float updatedDirection = projectile.velocity.ToRotation().AngleTowards(idealDirection, angularTurnSpeed);
                projectile.velocity = updatedDirection.ToRotationVector2() * projectile.velocity.Length();
            }

            if (projectile.timeLeft < 35)
                projectile.tileCollide = true;

            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);
            int dustParticle = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 0.9f);
            Main.dust[dustParticle].noGravity = true;
            Main.dust[dustParticle].velocity *= 0.5f;
            Main.dust[dustParticle].velocity += projectile.velocity * 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 35)
                return false;

            DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item43, projectile.Center);
            for (int i = 0; i <= 15; i++)
            {
                Vector2 displace = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (-0.5f + (i / 15f)) * 88f;
                int dustParticle = Dust.NewDust(projectile.Center + displace, projectile.width, projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dustParticle].noGravity = true;
                Main.dust[dustParticle].velocity = projectile.oldVelocity;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int debuffTime = 90;
            target.AddBuff(BuffType<ArmorCrunch>(), debuffTime);
        }
    }

    public class PurityProjectionSigil : ModProjectile
    {
        private NPC target => Main.npc[(int)projectile.ai[0]];

        public Player Owner => Main.player[projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_PurityProjectionSigil";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purity Sigil");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AI()
        {

            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);
            int dustParticle = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 0.9f);
            Main.dust[dustParticle].noGravity = true;
            Main.dust[dustParticle].velocity *= 0.5f;

            if (target.active)
            {
                projectile.Center = target.Center;
            }
            else
                projectile.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Main.myPlayer != projectile.owner) // don't show for other players
                return false;
            DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }

    public class TrueDecaysRetort : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_DecaysRetort";
        private bool initialized = false;
        public Vector2 direction = Vector2.Zero;
        public ref float MaxTime => ref projectile.ai[0];
        public ref float CanLunge => ref projectile.ai[1];
        public float Timer => MaxTime - projectile.timeLeft;
        public bool ChargedUp;
        public Player Owner => Main.player[projectile.owner];
        public const float LungeSpeed = 16;
        public const float PowerLungeDistance = 500;
        public ref float CanBounce => ref projectile.localAI[0];
        public ref float dashTimer => ref projectile.localAI[1];
        public const float maxDash = 20f;

        private Vector2 PowerLungeStart;
        private Vector2 PowerLungeEnd;

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
            float bladeLenght = 140f * projectile.scale;
            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace + (direction * bladeLenght), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                CanBounce = 1f;
                projectile.timeLeft = (int)MaxTime;
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();
                if (CanLunge == 1f && !ChargedUp)
                    Lunge();
                Main.PlaySound(SoundID.Item103, projectile.Center);
                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            if (ChargedUp && Timer / MaxTime > 0.5 && dashTimer == 0f)
            {
                PowerLunge();
            }

            if (dashTimer >= 1f)
            {
                if (dashTimer < maxDash)
                {
                    Owner.fallStart = (int)(Owner.position.Y / 16f);
                    Owner.velocity = direction * 60f;
                    projectile.timeLeft = (int)(MaxTime / 2f);
                    dashTimer++;
                }

                if (dashTimer == maxDash)
                {
                    Owner.velocity *= 0.1f; //Abrupt stop
                    if (Owner.HeldItem.modItem is TrueBiomeBlade blade)
                        blade.strongLunge = false;
                    Projectile proj = Projectile.NewProjectileDirect(Owner.Center + (PowerLungeEnd - PowerLungeStart) / 2f, Vector2.Zero, ProjectileType<DecaysRetortDash>(), projectile.damage * 2, 0, Owner.whoAmI);
                    if (proj.modProjectile is DecaysRetortDash dash)
                    {
                        dash.DashStart = PowerLungeStart;
                        dash.DashEnd = Owner.Center;
                    }

                    dashTimer = maxDash + 1;
                }
            }

            //Manage position and rotation
            projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER
            projectile.Center = Owner.Center + (direction * ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 60));

            Lighting.AddLight(projectile.Center, new Vector3(0.9f, 0f, 0.35f) * (float)Math.Sin(Timer / MaxTime * MathHelper.Pi));

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
            Owner.velocity = direction.SafeNormalize(Vector2.UnitX * Owner.direction) * LungeSpeed;
        }

        public void PowerLunge()
        {
            if (Owner.HeldItem.modItem is TrueBiomeBlade blade)
                blade.strongLunge = true;
            PowerLungeStart = Owner.Center;
            dashTimer = 1f;
            Owner.GiveIFrames(60);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => OnHitEffects(!target.canGhostHeal || Main.player[projectile.owner].moonLeech);
        public override void OnHitPvp(Player target, int damage, bool crit) => OnHitEffects(Main.player[projectile.owner].moonLeech);

        private void OnHitEffects(bool cannotLifesteal)
        {
            if (ChargedUp)
                return;

            projectile.netUpdate = true;
            projectile.netSpam = 0;

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
            if (Owner.whoAmI == Main.myPlayer)
            {
                if (Owner.HeldItem.modItem is TrueBiomeBlade sword)
                {
                    sword.PowerLungeCounter++;
                    if (sword.PowerLungeCounter == 3)
                        Main.PlaySound(SoundID.Item79); //indicate the charge with a sound effect
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade");
            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_DecaysRetort");

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

            spriteBatch.Draw(tex, drawOffset + displace, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            if (dashTimer > 0f && dashTimer < maxDash)
            {
                float thrustRatio = (float)Math.Sin(dashTimer / maxDash * MathHelper.Pi);
                spriteBatch.Draw(tex, drawOffset + displace, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale * (1 + thrustRatio * 0.2f), 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
            writer.Write(CanBounce);
            writer.Write(ChargedUp);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
            CanBounce = reader.ReadSingle();
            ChargedUp = reader.ReadBoolean();
        }
    }

    public class DecaysRetortDash : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => 20 - projectile.timeLeft;

        public Vector2 DashStart;
        public Vector2 DashEnd;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evisceration Lunge");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 8;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
            projectile.timeLeft = 20;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), DashStart, DashEnd);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 sparkSpeed = target.DirectionTo(Owner.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 9f;
                Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.Crimson, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                GeneralParticleHandler.SpawnParticle(Spark);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) //OMw to reuse way too much code from the entangling vines
        {

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D chainTex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_DecaysRetortDash");

            Vector2 Shake = projectile.timeLeft < 15 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (15 - projectile.timeLeft / 5f) * 0.5f;

            Vector2 lineDirection = Vector2.Normalize(DashEnd - DashStart);
            int dist = (int)Vector2.Distance(DashEnd, DashStart) / 16;
            Vector2[] Nodes = new Vector2[dist + 1];
            Nodes[0] = DashStart;
            Nodes[dist] = DashEnd;


            for (int i = 1; i < dist + 1; i++)
            {
                Vector2 positionAlongLine = Vector2.Lerp(DashStart, DashEnd, i / (float)dist); //Get the position of the segment along the line

                Nodes[i] = positionAlongLine + Shake * (float)Math.Sin(i / (float)dist * MathHelper.PiOver2);

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                float xScale = (i / (float)dist) * 5f;
                Vector2 scale = new Vector2(xScale, yScale);

                float opacity = MathHelper.Clamp((float)Math.Sin(i / (float)dist * MathHelper.PiOver2) - (i / (float)dist * ((20f - projectile.timeLeft) / 25f)), 0f, 1f);

                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                spriteBatch.Draw(chainTex, Nodes[i] - Main.screenPosition, null, Color.Crimson * opacity, rotation, origin, scale, SpriteEffects.None, 0);
            }

            Texture2D sparkTexture = GetTexture("CalamityMod/Particles/CritSpark");
            Texture2D bloomTexture = GetTexture("CalamityMod/Particles/BloomCircle");
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)sparkTexture.Width / (float)bloomTexture.Height;

            float bump = (float)Math.Sin(((20f - projectile.timeLeft) / 20f) * MathHelper.Pi);
            float raise = (float)Math.Sin(((20f - projectile.timeLeft) / 20f) * MathHelper.PiOver2);
            Rectangle frame = new Rectangle(0, 0, 14, 14);

            spriteBatch.Draw(bloomTexture, DashEnd - Main.screenPosition, null, Color.Crimson * bump * 0.5f, 0, bloomTexture.Size() / 2f, bump * 6f * properBloomSize, SpriteEffects.None, 0);
            spriteBatch.Draw(sparkTexture, DashEnd - Main.screenPosition, frame, Color.Lerp(Color.White, Color.Crimson, raise) * bump, raise * MathHelper.TwoPi, frame.Size() / 2f, bump * 3f, SpriteEffects.None, 0);

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class TrueBitingEmbrace : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_BitingEmbrace";

        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public float rotation;
        public ref float SwingMode => ref projectile.ai[0]; //0 = Up-Down small slash, 1 = Down-Up large slash, 2 = Thrust
        public ref float MaxTime => ref projectile.ai[1];
        public float Timer => MaxTime - projectile.timeLeft;
        const float MistDamageReduction = 0.2f;

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
                case 1:
                    bladeLenght = 150 * projectile.scale;
                    break;
                case 2:
                    bladeLenght = 225f; //In awe e
                    bladeLenght *= projectile.scale;
                    displace = direction * ThrustDisplaceRatio() * 60f;
                    break;

            }
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace + (rotation.ToRotationVector2() * bladeLenght), 26, ref collisionPoint);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            if (SwingMode == 2)
                target.AddBuff(BuffType<GlacialState>(), 40);
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
                        projectile.width = projectile.height = 100;
                        projectile.width = projectile.height = 100;
                        Main.PlaySound(SoundID.DD2_OgreSpit, projectile.Center);
                        projectile.damage = (int)(projectile.damage * 1.8);
                        break;
                    case 2:
                        projectile.width = projectile.height = 170;
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
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            //Manage position and rotation
            projectile.Center = Owner.Center + (direction * 30);
            //rotation = projectile.rotation + MathHelper.SmoothStep(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, Timer / MaxTime); 
            float factor = 1 - (float)Math.Pow((double)-(Timer / MaxTime) + 1, 2d);
            rotation = projectile.rotation + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, factor);
            projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER

            Lighting.AddLight(Owner.MountedCenter, new Vector3(0.75f, 1f, 1f) * (float)Math.Sin(Timer / MaxTime * MathHelper.Pi));

            //Add the thrust motion & animation for the third combo state
            if (SwingMode == 2)
            {
                projectile.scale = 1f + (ThrustScaleRatio() * 0.3f);
                projectile.Center = Owner.Center + (direction * ThrustDisplaceRatio() * 60);

                projectile.frameCounter++;
                if (projectile.frameCounter % 5 == 0 && projectile.frame + 1 < Main.projFrames[projectile.type])
                    projectile.frame++;

                if (Main.rand.NextBool())
                {
                    Projectile mist = Projectile.NewProjectileDirect(Owner.Center + direction * 40 + Main.rand.NextVector2Circular(30f, 30f), Vector2.Zero, ProjectileType<BitingEmbraceMist>(), (int)(projectile.damage * MistDamageReduction), 0f, Owner.whoAmI);
                    mist.velocity = (mist.Center - Owner.Center) * 0.2f + Owner.velocity;
                }

            }

            else
            {
                if (Main.rand.NextFloat(0f, 1f) > 0.75f)
                {
                    Projectile.NewProjectile(Owner.Center + direction * 40, rotation.ToRotationVector2() * 5, ProjectileType<BitingEmbraceMist>(), (int)(projectile.damage * MistDamageReduction), 0f, Owner.whoAmI);

                    Vector2 particlePosition = Owner.Center + (rotation.ToRotationVector2() * 100f * projectile.scale);
                    Particle snowflake = new SnowflakeSparkle(particlePosition, rotation.ToRotationVector2() * 3f, Color.White, new Color(75, 177, 250), Main.rand.NextFloat(0.3f, 1.5f), 40, 0.5f);
                    GeneralParticleHandler.SpawnParticle(snowflake);
                }
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

        //Animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.SineBump, 0f, 0f, -0.15f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyInOut, 0.2f, 0f, 0.9f, 3);
        public CurveSegment hold = new CurveSegment(EasingType.SineBump, 0.35f, 0.9f, 0.1f);
        public CurveSegment retract = new CurveSegment(EasingType.PolyInOut, 0.7f, 0.9f, -0.9f, 3);
        internal float ThrustDisplaceRatio() => PiecewiseAnimation(Timer / MaxTime, new CurveSegment[] { anticipation, thrust, hold, retract });

        //Animation keys
        public CurveSegment expandSize = new CurveSegment(EasingType.ExpIn, 0f, 0f, 1f);
        public CurveSegment holdSize = new CurveSegment(EasingType.Linear, 0.1f, 1f, 0f);
        public CurveSegment shrinkSize = new CurveSegment(EasingType.ExpIn, 0.85f, 1f, -1f);
        internal float ThrustScaleRatio() => PiecewiseAnimation(Timer / MaxTime, new CurveSegment[] { expandSize, holdSize, shrinkSize });

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade");

            if (SwingMode != 2)
            {
                Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_BitingEmbrace");
                float drawAngle = rotation;
                float drawRotation = rotation + MathHelper.PiOver4;
                Vector2 drawOrigin = new Vector2(0f, handle.Height);
                Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

                spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Turn on additive blending
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Update the parameters
                drawOrigin = new Vector2(0f, blade.Height);
                spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            else
            {
                Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_BitingEmbraceThrust");
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

                drawOrigin = new Vector2(0f, blade.Height);
                //Anim stuff

                spriteBatch.Draw(blade, drawOffset + thrustDisplace, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
            writer.Write(rotation);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
            rotation = reader.ReadSingle();
        }
    }

    public class BitingEmbraceMist : ModProjectile
    {
        public override string Texture => "CalamityMod/Particles/MediumMist";
        public Player Owner => Main.player[projectile.owner];
        public Color mistColor;
        public int variant = -1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacial Mist");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 34;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 300;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 size = projectile.Size * projectile.scale;

            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - size / 2f, size);
        }

        public override void AI()
        {
            if (variant == -1)
                variant = Main.rand.Next(3);

            if (Main.rand.Next(15) == 0 && projectile.alpha <= 140) //only try to spawn your particles if you're not close to dying
            {
                Vector2 particlePosition = projectile.Center + Main.rand.NextVector2Circular(projectile.width * projectile.scale * 0.5f, projectile.height * projectile.scale * 0.5f);
                Particle snowflake = new SnowflakeSparkle(particlePosition, Vector2.Zero, Color.White, new Color(75, 177, 250), Main.rand.NextFloat(0.3f, 1.5f), 40, 0.5f);
                GeneralParticleHandler.SpawnParticle(snowflake);
            }

            projectile.velocity *= 0.85f;
            projectile.position += projectile.velocity;
            projectile.rotation += 0.02f * projectile.timeLeft / 300f * ((projectile.velocity.X > 0) ? 1f : -1f);

            if (projectile.alpha < 165)
            {
                projectile.scale += 0.05f;
                projectile.alpha += 2;
            }
            else
            {
                projectile.scale *= 0.975f;
                projectile.alpha += 1;
            }
            if (projectile.alpha >= 170)
                projectile.Kill();

            mistColor = Color.Lerp(new Color(172, 238, 255), new Color(145, 170, 188), MathHelper.Clamp((float)(projectile.alpha - 100) / 80, 0f, 1f)) * (255 - projectile.alpha / 255f);
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, null, null, Main.GameViewMatrix.ZoomMatrix);

            var tex = GetTexture("CalamityMod/Particles/MediumMist");
            Rectangle frame = tex.Frame(1, 3, 0, variant);
            spriteBatch.Draw(tex, projectile.position - Main.screenPosition, frame, mistColor * 0.5f * ((255f - projectile.alpha) / 255f), projectile.rotation, frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

    public class TrueAridGrandeur : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_AridGrandeur";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0]; //How much the attack is, attacking
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public ref float PogoCooldown => ref projectile.ai[1]; //Cooldown for the pogo
        public ref float BounceTime => ref projectile.localAI[0];
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
            float bladeLenght = 100 * projectile.scale;
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

                Vector2 hitPosition = Owner.Center + (direction * 100 * projectile.scale);
                BounceTime = 20f; //Used only for animation

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
                    Particle smoke = new SmallSmokeParticle(hitPosition + hitPositionDisplace, flyDirection * 9f, Color.OrangeRed, new Color(130, 130, 130), Main.rand.NextFloat(1.8f, 2.6f), 115 - Main.rand.Next(30));
                    GeneralParticleHandler.SpawnParticle(smoke);

                    Particle Glow = new StrongBloom(hitPosition - hitPositionDisplace * 3, -direction * 6 * Main.rand.NextFloat(0.5f, 1f), Color.Orange * 0.5f, 0.01f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(40));
                    GeneralParticleHandler.SpawnParticle(Glow);
                }
                for (int i = 0; i < 3; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));

                    Particle Rock = new StoneDebrisParticle(hitPosition - hitPositionDisplace * 3, flyDirection * Main.rand.NextFloat(3f, 6f), Color.Beige, 1f + Main.rand.NextFloat(0f, 0.4f), 30 + Main.rand.Next(50), 0.1f);
                    GeneralParticleHandler.SpawnParticle(Rock);
                }

                if (Owner.HeldItem.type == ItemType<TrueBiomeBlade>())
                    (Owner.HeldItem.modItem as TrueBiomeBlade).StoredLunges = 2; // Reset the lunge counter on pogo. This should make for more interesting and fun synergies
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

            Lighting.AddLight(projectile.Center, new Vector3(1f, 0.56f, 0.56f) * ShredRatio);

            //Manage position and rotation
            direction = Owner.DirectionTo(Main.MouseWorld);
            direction.Normalize();
            projectile.rotation = direction.ToRotation();
            projectile.Center = Owner.Center + (direction * 60);

            //Scaling based on shred
            projectile.localNPCHitCooldown = 16 - (int)(MathHelper.Lerp(0, 8, ShredRatio)); //Increase the hit frequency
            projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER


            if (Collision.SolidCollision(Owner.Center + (direction * 100 * projectile.scale) - Vector2.One * 5f, 10, 10))
            {
                Pogo();
                projectile.netUpdate = true;
                projectile.netSpam = 0;
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
            BounceTime--;
            projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => ShredTarget();
        public override void OnHitPvp(Player target, int damage, bool crit) => ShredTarget();

        private void ShredTarget()
        {
            if (Main.myPlayer != Owner.whoAmI)
                return;
            // get lifted up
            if (PogoCooldown <= 0)
            {
                Main.PlaySound(SoundID.NPCHit30, projectile.Center); //Sizzle
                Shred += 62; //Augment the shredspeed
                if (Owner.velocity.Y > 0)
                    Owner.velocity.Y = -2f; //Get "stuck" into the enemy partly
                Owner.GiveIFrames(5); // i framez. Do 5 iframes even matter? idk but you get a lot of em so lol...
                PogoCooldown = 20;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit43, projectile.Center);
            if (ShredRatio > 0.5 && Owner.whoAmI == Main.myPlayer) //Keep this for the True biome blade/Repaired biome blade.
            {
                Projectile.NewProjectile(projectile.Center, direction * 16f, ProjectileType<TrueAridGrandeurShot>(), projectile.damage, projectile.knockBack, Owner.whoAmI, Shred);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_AridGrandeur");

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
            drawOrigin = new Vector2(0f, blade.Height);

            spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);


            for (int i = 0; i < bladeAmount; i++) //Draw extra copies
            {
                blade = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_AridGrandeurExtra");

                drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio);

                drawOrigin = new Vector2(0f, blade.Height);

                Vector2 drawOffsetStraight = Owner.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly
                Vector2 drawOffsetFromBounce = direction * MathHelper.Clamp(BounceTime, 0f, 20f) / 20f * 20f;

                spriteBatch.Draw(blade, drawOffsetStraight + drawDisplacementAngle + drawOffsetFromBounce, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

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

    public class TrueAridGrandeurShot : ModProjectile //Only use this for the upgrade actually lol
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_AridGrandeurExtra";
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
                var tex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_AridGrandeurExtra");

                float drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                float drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio);

                Vector2 drawOrigin = new Vector2(0f, tex.Height);


                Vector2 drawOffsetStraight = projectile.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                float opacityFade = projectile.timeLeft > 15 ? 1 : projectile.timeLeft / 15f;

                spriteBatch.Draw(tex, drawOffsetStraight + drawDisplacementAngle, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f * opacityFade, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item60, projectile.Center);
            for (int i = 0; i < 4; i++) //Particel
            {
                float drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                float drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio);


                Vector2 drawOffsetStraight = projectile.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                for (int j = 0; j < 4; j++)
                {
                    Particle Sparkle = new GenericSparkle(drawOffsetStraight + (drawRotation - MathHelper.PiOver4).ToRotationVector2() * (60 + j * 50f) + drawDisplacementAngle, direction * projectile.velocity.Length() * Main.rand.NextFloat(0.9f, 1.1f), Color.Lerp(Color.Cyan, Color.Orange, (j + 1) / 4f), Color.OrangeRed, 0.5f + Main.rand.NextFloat(-0.2f, 0.2f), 20 + Main.rand.Next(30), 1, 2f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

            }

        }

    }

    public class TrueGrovetendersTouch : ModProjectile
    {
        private NPC[] excludedTargets = new NPC[4];
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_GrovetendersTouchBlade";
        private bool initialized = false;
        public Player Owner => Main.player[projectile.owner];
        public float Timer => MaxTime - projectile.timeLeft;
        public ref float HasSnapped => ref projectile.ai[0];
        public ref float SnapCoyoteTime => ref projectile.ai[1];

        public float flipped;

        const float MaxTime = 90;
        const int coyoteTimeFrames = 15; //How many frames does the whip stay extended 
        const int MaxReach = 400;
        const float SnappingPoint = 0.55f; //When does the snap occur.
        const float ReelBackStrenght = 14f;
        const float ChainDamageReduction = 0.5f;

        const float MaxTangleReach = 400f; //How long can tangling vines from crits be

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
            projectile.width = projectile.height = 80;
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
                if (i == numPoints - 1) //Extra lenght collision for the blade itself
                {
                    Vector2 projectileHalfLenght = 85 * projectile.rotation.ToRotationVector2();
                    return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - projectileHalfLenght, projectile.Center + projectileHalfLenght, 32, ref collisionPoint));
                }

            }

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);
            Vector2 projectileHalfLenght = 85f * projectile.rotation.ToRotationVector2();
            float collisionPoint = 0;
            //If you hit the enemy during the coyote time with the blade of the whip, guarantee a crit
            if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), projectile.Center - projectileHalfLenght, projectile.Center + projectileHalfLenght, 32, ref collisionPoint))
            {
                if (SnapCoyoteTime > 0f)
                {
                    crit = true;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 sparkSpeed = Owner.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 9f;
                        Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.LimeGreen, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                        GeneralParticleHandler.SpawnParticle(Spark);
                    }

                }
            }
            else
                damage = (int)(damage * ChainDamageReduction); //If the enemy is hit with the chain of the whip, the damage gets reduced
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                bool boing = false;
                excludedTargets[0] = target;
                for (int i = 0; i < 3; i++)
                {
                    NPC potentialTarget = TargetNext(target.Center, i);
                    if (potentialTarget == null)
                        break;
                    if (!boing)
                    {
                        boing = true;
                        Main.PlaySound(SoundID.Item56);
                    }
                    Projectile.NewProjectile(target.Center, Vector2.Zero, ProjectileType<GrovetendersEntanglingVines>(), damage / 2, 0, Owner.whoAmI, target.whoAmI, potentialTarget.whoAmI);
                }
                Array.Clear(excludedTargets, 0, 3);
            }
        }

        public NPC TargetNext(Vector2 hitFrom, int index)
        {
            float longestReach = MaxTangleReach;
            NPC target = null;
            for (int i = 0; i < 200; ++i)
            {
                NPC npc = Main.npc[i];
                if (!excludedTargets.Contains(npc) && npc.CanBeChasedBy() && !npc.friendly && !npc.townNPC)
                {
                    float distance = Vector2.Distance(hitFrom, npc.Center);
                    if (distance < longestReach)
                    {
                        longestReach = distance;
                        target = npc;
                    }
                }
            }
            if (index < 3)
                excludedTargets[index + 1] = target;
            return target;
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
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            if (ReelingBack && HasSnapped == 0f) //Snap & also small coyote time for the hook
            {
                Main.PlaySound(SoundID.Item41, projectile.Center); //Snap
                HasSnapped = 1f;
                SnapCoyoteTime = coyoteTimeFrames;
            }

            if (SnapCoyoteTime > 0) //keep checking for the tile hook
            {
                Lighting.AddLight(projectile.Center, 0.8f, 1f, 0.35f);
                HookToTile();
                SnapCoyoteTime--;
            }

            Owner.direction = Math.Sign(projectile.velocity.X);
            projectile.rotation = projectile.AngleFrom(Owner.Center); //Point away from playah

            float ratio = GetSwingRatio();
            projectile.Center = Owner.MountedCenter + SwingPosition(ratio);
            projectile.direction = projectile.spriteDirection = -Owner.direction * (int)flipped;

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
                float angleOffset = Owner.direction * flipped * MathHelper.Lerp(-angleDeviation, 0, progress); //Go from very angled to straight at the zenith of the attack
                if (flipped == -1)
                    distance *= MathHelper.Lerp(0.1f, 0.3f, (float)Math.Sin(progress * MathHelper.Pi));
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
            Texture2D handle = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GrovetendersTouchBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GrovetendersTouchGlow");

            Vector2 projBottom = projectile.Center + new Vector2(-handle.Width / 2, handle.Height / 2).RotatedBy(projectile.rotation + MathHelper.PiOver4) * 0.75f;
            DrawChain(spriteBatch, projBottom, out Vector2[] chainPositions);

            float drawRotation = (projBottom - chainPositions[chainPositions.Length - 2]).ToRotation() + MathHelper.PiOver4; //Face away from the last point of the bezier curve
            drawRotation += SnapCoyoteTime > 0 ? MathHelper.Pi : 0; //During coyote time the blade flips for some reason. Prevent that from happening
            drawRotation += projectile.spriteDirection < 0 ? 0f : 0f;

            if (ReelingBack)
                drawRotation = Utils.AngleLerp(drawRotation, (projectile.Center - Owner.Center).ToRotation(), GetSwingRatio());

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            SpriteEffects flip = (projectile.spriteDirection < 0) ? SpriteEffects.None : SpriteEffects.None;
            lightColor = Lighting.GetColor((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f));

            Vector2 nitpickCorrection = (flipped == -1 && (Timer / MaxTime < 0.35f)) ? drawRotation.ToRotationVector2() * 16f + (Owner.direction == -1f ? Vector2.UnitX * -12f : Vector2.Zero) : Vector2.Zero;

            spriteBatch.Draw(handle, projBottom - nitpickCorrection - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, flip, 0f);

            if ((!ReelingBack || SnapCoyoteTime != 0f) && (Timer / MaxTime > 0.35f))
            {
                //Turn on additive blending
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Only update the origin for once
                drawOrigin = new Vector2(0f, blade.Height);
                spriteBatch.Draw(blade, projBottom - nitpickCorrection - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, flip, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }

        private void DrawChain(SpriteBatch spriteBatch, Vector2 projBottom, out Vector2[] chainPositions)
        {
            Texture2D chainTex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchChain");

            float ratio = GetSwingRatio();

            if (!ReelingBack) //Make the curve be formed from points slightly ahead of the projectile, but clamped to the max rotation (straight line ahead)
            {
                controlPoint1 = Owner.MountedCenter + SwingPosition(MathHelper.Clamp(ratio + 0.5f, 0f, 1f)) * 0.2f;
                controlPoint2 = Owner.MountedCenter + SwingPosition(MathHelper.Clamp(ratio + 0.2f, 0f, 1f)) * 0.5f;
            }
            else //After the whip snaps, make the curve be a wave 
            {
                Vector2 perpendicular = SwingPosition(ratio).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
                controlPoint1 = Owner.MountedCenter + SwingPosition(MathHelper.Lerp(ratio, 1f, ratio)) + perpendicular * MathHelper.SmoothStep(0f, 1f, ratio) * 155f * Owner.direction;
                controlPoint2 = Owner.MountedCenter + SwingPosition(MathHelper.Lerp(ratio, 1f, ratio / 2)) + perpendicular * MathHelper.SmoothStep(0f, 1f, ratio) * -100f * Owner.direction;
            }

            BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, controlPoint1, controlPoint2, projBottom });
            int numPoints = 30;
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
        }
    }

    public class GrovetendersEntanglingVines : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchChain";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => 20 - projectile.timeLeft;
        public NPC NPCfrom
        {
            get => Main.npc[(int)projectile.ai[0]];
            set => projectile.ai[0] = value.whoAmI;
        }
        public NPC Target
        {
            get => Main.npc[(int)projectile.ai[1]];
            set => projectile.ai[1] = value.whoAmI;
        }

        const float curvature = 16f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Entangling Growth");
        }
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

        public override bool? CanHitNPC(NPC target) => target == Target;

        public override void AI()
        {
            projectile.Center = Target.Center;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D chainTex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchChain");

            float opacity = projectile.timeLeft > 10 ? 1 : projectile.timeLeft / 10f;
            Vector2 Shake = projectile.timeLeft < 15 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (15 - projectile.timeLeft / 5f) * 0.5f;

            Vector2 lineDirection = Vector2.Normalize(Target.Center - NPCfrom.Center);
            int dist = (int)Vector2.Distance(Target.Center, NPCfrom.Center) / 16;
            Vector2[] Nodes = new Vector2[dist + 1];
            Nodes[0] = NPCfrom.Center;
            Nodes[dist] = Target.Center;
            float pointUp = Target.Center.X > NPCfrom.Center.X ? -MathHelper.PiOver2 : MathHelper.PiOver2;

            for (int i = 1; i < dist + 1; i++)
            {
                Vector2 positionAlongLine = Vector2.Lerp(NPCfrom.Center, Target.Center, i / (float)dist); //Get the position of the segment along the line, as if it were a flat line
                float elevation = (float)Math.Sin(i / (float)dist * MathHelper.Pi) * curvature * dist / 10f;
                Nodes[i] = positionAlongLine + lineDirection.RotatedBy(pointUp) * elevation + Shake * (float)Math.Sin(i / (float)dist * MathHelper.Pi);

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);

                Color chainLightColor = Lighting.GetColor((int)Nodes[i].X / 16, (int)Nodes[i].Y / 16); //Lighting of the position of the chain segment

                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                spriteBatch.Draw(chainTex, Nodes[i] - Main.screenPosition, null, chainLightColor * opacity, rotation, origin, scale, SpriteEffects.None, 0);
            }
            return false;
        }

    }

    public class HeavensMight : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_HeavensMight";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        Vector2 oldDirection = Vector2.Zero;
        public ref float CurrentState => ref projectile.ai[0];
        public Player Owner => Main.player[projectile.owner];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        public const float throwOutTime = 90f;
        public const float throwOutDistance = 440f;

        public static float snapPoint = 0.45f;
        public float snapTimer => (throwTimer / throwOutTime) < snapPoint ? 0 : ((throwTimer / throwOutTime) - snapPoint) / (1f - snapPoint);
        public static float retractionPoint = 0.6f;
        public float retractionTimer => (throwTimer / throwOutTime) < retractionPoint ? 0 : ((throwTimer / throwOutTime) - retractionPoint) / (1f - retractionPoint);
        public ref float Empowerment => ref projectile.ai[1];
        public ref float hasMadeSound => ref projectile.localAI[0];
        public ref float hasMadeChargeSound => ref projectile.localAI[1];

        public const float maxEmpowerment = 600f;
        //How much damage reduction applies on uncharged attacks
        public const float initialDamage = 0.2f;
        //How much extra damage applies when fully charged. This goes ontop of the initial damage, so a max empowerment boost of 2 with an initial damage of 0.1 makes the total damage multiplier be 1.9
        public float maxEmpowermentBoost = 2f;
        public float throwTimer => throwOutTime - projectile.timeLeft;

        public Particle smear;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heaven's Might");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 74;
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
            float bladeLenght = 100 * projectile.scale;
            float bladeWidth = 20 * projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }


        public CurveSegment launch = new CurveSegment(EasingType.CircOut, 0f, 0f, 1f, 4);
        public CurveSegment hold = new CurveSegment(EasingType.Linear, snapPoint, 1f, 0f);
        public CurveSegment retract = new CurveSegment(EasingType.PolyInOut, retractionPoint, 1f, -1.05f, 3);
        internal float ThrowCurve() => PiecewiseAnimation((throwOutTime - projectile.timeLeft) / throwOutTime, new CurveSegment[] { launch, hold, retract });

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                projectile.velocity = Vector2.Zero;
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                oldDirection = direction;
                initialized = true;
            }

            projectile.rotation = direction.ToRotation(); //Only done for afterimages

            if (!OwnerCanShoot)
            {
                if (CurrentState == 2f || (CurrentState == 0f && Empowerment / maxEmpowerment < 0.5))
                {
                    Main.PlaySound(SoundID.Item77, projectile.Center);
                    projectile.Kill();
                    return;
                }

                else if (CurrentState == 0f)
                {
                    CurrentState = 1f;
                    Main.PlaySound(SoundID.Item80, projectile.Center);
                    direction = Owner.DirectionTo(Main.MouseWorld);
                    //PARTICLES LOTS OF PARTICLES LOTS OF SPARKLES YES YES MH YES YES 
                    for (int i = 0; i <= 8; i++)
                    {
                        float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                        float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                        Particle Sparkle = new CritSpark(projectile.Center, Owner.velocity + direction.RotatedBy(variation) * (1 + strength) * 2f * Main.rand.NextFloat(7.5f, 20f), Color.White, Color.HotPink, 2f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                    for (int i = 0; i <= 8; i++)
                    {
                        float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                        float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                        Particle Sparkle = new CritSpark(projectile.Center, Owner.velocity + direction.RotatedBy(variation) * (1 + strength) * Main.rand.NextFloat(7.5f, 20f), Color.White, Color.Cyan, 2f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                }
            }

            if (CurrentState == 0f)
            {
                if (Owner.whoAmI == Main.myPlayer)
                {
                    if (hasMadeChargeSound == 0f && Empowerment / maxEmpowerment >= 0.5)
                    {
                        hasMadeChargeSound = 1f;
                        Main.PlaySound(SoundID.Item76);
                    }

                    float rotation = direction.ToRotation();
                    if (rotation > -MathHelper.PiOver2 - MathHelper.PiOver4 && rotation < -MathHelper.PiOver2 + MathHelper.PiOver4 && hasMadeSound == 1f)
                        hasMadeSound = 0f;

                    else if (rotation > MathHelper.PiOver2 - MathHelper.PiOver4 && rotation < MathHelper.PiOver2 + MathHelper.PiOver4 && hasMadeSound == 0f)
                    {
                        hasMadeSound = 1f;
                        Main.PlaySound(SoundID.Item71);
                    }
                }

                if (Empowerment / maxEmpowerment >= 0.75)
                {
                    if (smear == null)
                    {
                        smear = new CircularSmearVFX(Owner.Center, Color.HotPink, direction.ToRotation(), projectile.scale * 1.5f);
                        GeneralParticleHandler.SpawnParticle(smear);
                    }
                    //Update the variables of the smear
                    else
                    {
                        smear.Rotation = direction.ToRotation() + MathHelper.PiOver2;
                        smear.Time = 0;
                        smear.Position = Owner.Center;
                        smear.Scale = projectile.scale * 1.5f;
                        smear.Color = Color.Lerp(Color.HotPink, Color.Cyan, (float)Math.Sin(Main.GlobalTime * 2f)) * (((Empowerment / maxEmpowerment) - 0.75f) / 0.25f * 0.8f);
                    }
                }


                //Manage position and rotation
                projectile.scale = 1 + Empowerment / maxEmpowerment * 1.5f;

                oldDirection = direction;
                direction = direction.RotatedBy(MathHelper.Clamp(Empowerment / maxEmpowerment, 0.4f, 1f) * MathHelper.PiOver4 * 0.20f);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();
                projectile.Center = Owner.Center + (direction * projectile.scale * 10);
                projectile.timeLeft = (int)throwOutTime + 1;
                Empowerment++;
                if (Empowerment > maxEmpowerment)
                    Empowerment = maxEmpowerment;
            }

            if (CurrentState == 1f)
            {
                projectile.Center = Owner.Center + (direction * projectile.scale * 10) + (direction * throwOutDistance * ThrowCurve());
                projectile.scale = (1 + Empowerment / maxEmpowerment * 1.5f) * MathHelper.Clamp(1 - retractionTimer, 0.3f, 1f);
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
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * (initialDamage + (maxEmpowermentBoost * Empowerment / maxEmpowerment)));
        }

        public override void Kill(int timeLeft)
        {
            if (smear != null)
                smear.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/TrueBiomeBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_HeavensMight");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            //Afterimages
            if (CalamityConfig.Instance.Afterimages && CurrentState == 0f && Empowerment / maxEmpowerment > 0.4f)
            {
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                {
                    Color color = projectile.GetAlpha(lightColor) * (1f - (i / (float)projectile.oldRot.Length));
                    float afterimageRotation = projectile.oldRot[i] + MathHelper.PiOver4;
                    Main.spriteBatch.Draw(handle, drawOffset, null, color * MathHelper.Lerp(0f, 0.5f, MathHelper.Clamp(Empowerment / maxEmpowerment - 0.4f / 0.1f, 0f, 1f)), afterimageRotation, drawOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), 0f, 0f);
                }

                spriteBatch.End(); //Haha sup babe what if i restarted the spritebatch way too many times haha /blushes
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Vector2 afterimageDrawOrigin = new Vector2(0f, blade.Height);
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                {
                    Color color = projectile.GetAlpha(lightColor) * (1f - (i / (float)projectile.oldRot.Length));
                    float afterimageRotation = projectile.oldRot[i] + MathHelper.PiOver4;
                    Main.spriteBatch.Draw(blade, drawOffset, null, color * MathHelper.Lerp(0f, 0.5f, MathHelper.Clamp((Empowerment / maxEmpowerment - 0.4f) / 0.1f, 0f, 1f)), afterimageRotation, afterimageDrawOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), 0f, 0f);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }


            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            //Don't draw the glowing blade after the retraction
            float opacityFade = CurrentState == 0f ? 1f : 1f - retractionTimer;
            //Add basic tint (my beloved) during the snap
            if (snapTimer > 0 && retractionTimer <= 0)
            {
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(0.8f - snapTimer, 0f, 1f));
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Color.White);
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();
            }

            spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f * opacityFade, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            if (CurrentState == 1f && snapTimer > 0)
            {
                drawChain(spriteBatch, snapTimer, retractionTimer);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void drawChain(SpriteBatch spriteBatch, float snapProgress, float retractProgress)
        {
            Texture2D chainTex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_HeavensMightChain");

            float opacity = retractProgress < 0.5 ? 1 : (retractProgress - 0.5f) / 0.5f;

            Vector2 Shake = retractProgress > 0 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (1f - snapProgress) * 10f;

            int dist = (int)Vector2.Distance(Owner.Center, projectile.Center) / 16;
            Vector2[] Nodes = new Vector2[dist + 1];
            Nodes[0] = Owner.Center;
            Nodes[dist] = projectile.Center;

            for (int i = 1; i < dist + 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0 + 18 * (i % 2), 12, 18);
                Vector2 positionAlongLine = Vector2.Lerp(Owner.Center, projectile.Center, i / (float)dist); //Get the position of the segment along the line, as if it were a flat line
                Nodes[i] = positionAlongLine + Shake * (float)Math.Sin(i / (float)dist * MathHelper.Pi);

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / frame.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);

                Color chainLightColor = Lighting.GetColor((int)Nodes[i].X / 16, (int)Nodes[i].Y / 16); //Lighting of the position of the chain segment

                Vector2 origin = new Vector2(frame.Width / 2, frame.Height); //Draw from center bottom of texture
                spriteBatch.Draw(chainTex, Nodes[i] - Main.screenPosition, frame, chainLightColor * opacity * 0.7f, rotation, origin, scale, SpriteEffects.None, 0);
            }
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

    public class TheirAbhorrence : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_TheirAbhorrence";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[projectile.owner];
        public ref float Charge => ref projectile.ai[0]; //Charge
        public ref float State => ref projectile.ai[1]; //State 0 is "charging", State 1 is "thrusting"
        public ref float CurrentIndicator => ref projectile.localAI[0]; //What "indicator" stage are you on. 
        public ref float OverCharge => ref projectile.localAI[1];

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        const float MaxCharge = 500;

        public Vector2 lastDisplacement;
        public float dashDuration;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Their Abhorrence");
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

        public override bool CanDamage()
        {
            return (State == 1);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 120 * projectile.scale;
            float bladeWidth = 20 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public CurveSegment QuickOut = new CurveSegment(EasingType.PolyIn, 0f, 0f, 0.2f, 3);
        public CurveSegment Bump = new CurveSegment(EasingType.SineBump, 0.06f, 0.2f, 0.1f);
        public CurveSegment QuickDraw = new CurveSegment(EasingType.Linear, 0.25f, 0.2f, -0.45f);
        public CurveSegment SlowDrawOut = new CurveSegment(EasingType.PolyIn, 0.50f, -0.25f, -0.2f, 3);
        public CurveSegment OverShoot = new CurveSegment(EasingType.SineBump, 0.93f, -0.45f, -0.1f);

        internal float ChargeDisplacement() => PiecewiseAnimation(Charge / MaxCharge, new CurveSegment[] { QuickOut, Bump, QuickDraw, SlowDrawOut, OverShoot });

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                projectile.velocity = Vector2.Zero;
                Main.PlaySound(SoundID.Item101, projectile.Center);
                initialized = true;
            }

            if (!OwnerCanShoot)
            {
                if (State == 0f)
                {
                    if (Charge / MaxCharge < 0.25f)
                    {
                        Main.PlaySound(SoundID.Item109, projectile.Center);
                        projectile.Kill();
                        return;
                    }
                    else
                    {

                        var dashSound = Main.PlaySound(SoundID.Item120, projectile.Center);
                        if (dashSound != null)
                            dashSound.Volume *= 0.5f;
                        State = 1f;
                        projectile.timeLeft = (7 + (int)((Charge / MaxCharge - 0.25f) * 20)) * 2; //Keep that even, if its an odd number itll fuck off and wont reset the players velocity on death
                        dashDuration = projectile.timeLeft;
                        lastDisplacement = projectile.Center - Owner.Center;
                        projectile.netUpdate = true;
                        projectile.netSpam = 0;
                    }
                }
            }

            if (State == 0f)
            {
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.Center = Owner.Center + (direction * 70f * ChargeDisplacement());

                Charge++;
                OverCharge--;
                projectile.timeLeft = 2;

                if ((Charge / MaxCharge >= 0.25f && CurrentIndicator == 0f) || (Charge / MaxCharge >= 0.5f && CurrentIndicator == 1f) || (Charge / MaxCharge >= 0.75f && CurrentIndicator == 2f))
                {
                    Main.PlaySound(SoundID.DD2_WitherBeastAuraPulse, projectile.Center);
                    CurrentIndicator++;
                    OverCharge = 20f;
                }

                if (Charge >= MaxCharge)
                {
                    Charge = MaxCharge;
                    if (Owner.whoAmI == Main.myPlayer && CurrentIndicator < 4f)
                    {
                        OverCharge = 20f;
                        Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/CorvinaScream"), projectile.Center);
                        CurrentIndicator++;
                    }
                }
            }

            if (State == 1f)
            {
                projectile.Center = Owner.Center + Vector2.Lerp(lastDisplacement, direction * 40f, MathHelper.Clamp(((dashDuration - projectile.timeLeft) / dashDuration) * 2f, 0f, 1f));
                Owner.fallStart = (int)(Owner.position.Y / 16f);

                if (Owner.HeldItem.modItem is TrueBiomeBlade blade)
                {
                    blade.strongLunge = true; //Needed for the downwards velocity uncap.
                }

                if (Collision.SolidCollision(Owner.Center + (direction * 120 * projectile.scale) - Vector2.One * 5f, 10, 10))
                {
                    SlamDown();
                    projectile.timeLeft = 0;
                    projectile.active = false;
                    projectile.netUpdate = true;
                    projectile.netSpam = 0;
                }

                Owner.velocity = direction * 30f;

                float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                Particle Sparkle = new CritSpark(projectile.Center, Owner.velocity - direction.RotatedBy(variation) * (1 + strength) * 2f * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.MediumTurquoise : Color.DarkOrange, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }

            Lighting.AddLight(projectile.Center, new Vector3(1f, 0.56f, 0.56f) * Charge / MaxCharge);

            //Manage position and rotation
            projectile.rotation = direction.ToRotation();

            //Scaling based on charge
            projectile.scale = 1f + (Charge / MaxCharge * 0.3f);

            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();

            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }

            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public void SlamDown()
        {
            var slamSound = Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.Center);
            if (slamSound != null)
                slamSound.Volume *= 1.5f;

            if (Owner.whoAmI != Main.myPlayer || Owner.velocity.Y == 0f)
                return;

            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 5)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;

            //Only create the central monolith if over half charge
            if (Charge / MaxCharge < 0.5f)
                return;
            Projectile.NewProjectile(Owner.Center + (direction * 120 * projectile.scale), -direction, ProjectileType<TheirAbhorrenceMonolith>(), projectile.damage * 2, 10f, Owner.whoAmI, Main.rand.Next(4), 1f);

            //Only create the side monoliths if over 3/4th charge
            if (Charge / MaxCharge < 0.75f)
                return;

            if (SideSprouts(1, 150f, 0.8f) && Charge >= MaxCharge)
                SideSprouts(1, 250f, 0.5f);

            if (SideSprouts(-1, 150f, 0.8f) && Charge >= MaxCharge)
                SideSprouts(-1, 250f, 0.5f);

        }

        public bool SideSprouts(float facing, float distance, float projSize)
        {
            float widestAngle = 0f;
            float widestSurfaceAngle = 0f;
            bool validPositionFound = false;
            for (float i = 0f; i < 1; i += 1 / distance)
            {
                Vector2 positionToCheck = Owner.Center + (direction * 120 * projectile.scale) + direction.RotatedBy((i * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;

                if (Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolidGround())
                    widestAngle = i;

                else if (widestAngle != 0)
                {
                    validPositionFound = true;
                    widestSurfaceAngle = widestAngle;
                }
            }

            if (validPositionFound)
            {
                Vector2 projPosition = Owner.Center + (direction * 120 * projectile.scale) + direction.RotatedBy((widestSurfaceAngle * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;
                Vector2 monolithRotation = direction.RotatedBy(Utils.AngleLerp(widestSurfaceAngle * -facing, 0f, projSize));
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<TheirAbhorrenceMonolith>(), projectile.damage * 2, 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.modProjectile is TheirAbhorrenceMonolith monolith)
                    monolith.WaitTimer = (1 - projSize) * 34f;
            }

            return validPositionFound;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Owner.GiveIFrames(30);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * (1f + Charge / MaxCharge));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_TheirAbhorrence");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //Just in case
            if (OverCharge < 0)
                OverCharge = 0f;
            //When the blink is
            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(OverCharge / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(154, 244, 240));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            //Cut the velocity short if dashing
            if (State == 1f)
                Owner.velocity *= 0.33f;

            if (Owner.HeldItem.modItem is TrueBiomeBlade blade)
            {
                blade.strongLunge = false; //Needed for the downwards velocity uncap.
            }

            projectile.active = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
            writer.Write(CurrentIndicator);
            writer.WriteVector2(lastDisplacement);
            writer.Write(dashDuration);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
            CurrentIndicator = reader.ReadSingle();
            lastDisplacement = reader.ReadVector2();
            dashDuration = reader.ReadSingle();
        }
    }

    public class TheirAbhorrenceMonolith : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_TheirAbhorrenceMonolith";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => (100f - projectile.timeLeft) / 100f;
        public ref float Variant => ref projectile.ai[0]; //Yes
        public ref float Size => ref projectile.ai[1]; //Yes
        public float WaitTimer; //How long till it appears fr. GOtta remember to sync that one
        public const float BaseWidth = 90f;
        public const float BaseHeight = 420f;

        public CurveSegment StaySmall = new CurveSegment(EasingType.Linear, 0f, 0.2f, 0f);
        public CurveSegment GoBig = new CurveSegment(EasingType.SineOut, 0.25f, 0.2f, 1f);
        public CurveSegment GoNormal = new CurveSegment(EasingType.CircIn, 0.4f, 1.2f, -0.2f);
        public CurveSegment StayNormal = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0f);

        internal float Width() => PiecewiseAnimation(Timer, new CurveSegment[] { StaySmall, GoBig, GoNormal, StayNormal }) * BaseWidth * Size;

        public CurveSegment Anticipate = new CurveSegment(EasingType.CircIn, 0f, 0f, 0.15f);
        public CurveSegment Overextend = new CurveSegment(EasingType.SineOut, 0.2f, 0.15f, 1f);
        public CurveSegment Unextend = new CurveSegment(EasingType.CircIn, 0.25f, 1.15f, -0.15f);
        public CurveSegment Hold = new CurveSegment(EasingType.ExpOut, 0.70f, 1f, -0.1f);
        internal float Height() => PiecewiseAnimation(Timer, new CurveSegment[] { Anticipate, Overextend, Unextend, Hold }) * BaseHeight * Size;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Their Abhorrence");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 102;
            projectile.hide = true;
        }

        public override bool CanDamage() => WaitTimer <= 0;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + ((projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * Height()), Width(), ref collisionPoint);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            if (WaitTimer > 0)
            {
                projectile.timeLeft = 102;
                WaitTimer--;
            }

            if (projectile.timeLeft == 100)
            {
                SurfaceUp();
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                projectile.velocity = Vector2.Zero;

                if (Size >= 1) //Big monoliths create sparkles even before sprouting up
                {

                    for (int i = 0; i < 20; i++)
                    {
                        Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.MediumTurquoise : Color.DarkOrange, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                }
            }

            if (projectile.timeLeft == 80)
            {
                Vector2 particleDirection = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                Main.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, projectile.Center);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 10f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(5f, 15f);
                    Particle smoke = new SmallSmokeParticle(projectile.Center + hitPositionDisplace, flyDirection, Color.Lerp(Color.DarkOrange, Color.MediumTurquoise, Main.rand.NextFloat()), new Color(130, 130, 130), Main.rand.NextFloat(2.8f, 3.6f) * Size, 165 - Main.rand.Next(30), 0.1f);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
                for (int i = 0; i < 6; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(10f, 30f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));

                    Particle Rock = new StoneDebrisParticle(projectile.Center + hitPositionDisplace * 3, flyDirection * Main.rand.NextFloat(3f, 6f), Color.Lerp(Color.DarkSlateBlue, Color.LightSlateGray, Main.rand.NextFloat()), (1f + Main.rand.NextFloat(0f, 2.4f)) * Size, 30 + Main.rand.Next(50), 0.1f);
                    GeneralParticleHandler.SpawnParticle(Rock);
                }
            }
        }

        //Go up to the "surface" so you're not stuck in the middle of the ground like a complete moron.
        public void SurfaceUp()
        {
            for (float i = 0; i < 40; i += 0.5f)
            {
                Vector2 positionToCheck = projectile.Center + projectile.velocity * i;
                if (!Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolidGround())
                {
                    projectile.Center = projectile.Center + projectile.velocity * i;
                    return;
                }
            }
            projectile.Center = projectile.Center + projectile.velocity * 40f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (WaitTimer > 0)
                return false;

            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_TheirAbhorrenceMonolith");

            float drawAngle = projectile.rotation;
            Rectangle frame = new Rectangle(0 + (int)Variant * 94, 0, 94, 420);

            Vector2 drawScale = new Vector2(Width() / BaseWidth, Height() / BaseHeight);
            Vector2 drawPosition = projectile.Center - Main.screenPosition - (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 26f;
            Vector2 drawOrigin = new Vector2(frame.Width / 2f, frame.Height);

            float opacity = MathHelper.Clamp(1f - ((Timer - 0.85f) / 0.15f), 0f, 1f);

            spriteBatch.Draw(tex, drawPosition, frame, lightColor * opacity, drawAngle, drawOrigin, drawScale, 0f, 0f);

            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (WaitTimer > 0)
                return;

            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_TheirAbhorrenceMonolith_Glow");

            float drawAngle = projectile.rotation;
            Rectangle frame = new Rectangle(0 + (int)Variant * 94, 0, 94, 420);

            Vector2 drawScale = new Vector2(Width() / BaseWidth, Height() / BaseHeight);
            Vector2 drawPosition = projectile.Center - Main.screenPosition - (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 26f;
            Vector2 drawOrigin = new Vector2(frame.Width / 2f, frame.Height);

            float opacity = MathHelper.Clamp(1f - ((Timer - 0.85f) / 0.15f), 0f, 1f);

            spriteBatch.Draw(tex, drawPosition, frame, Color.White * opacity, drawAngle, drawOrigin, drawScale, 0f, 0f);
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(WaitTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            WaitTimer = reader.ReadSingle();
        }

    }

    public class GestureForTheDrowned : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TrueBiomeBlade";
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[projectile.owner];
        public float Timer => 40 - projectile.timeLeft;
        public float HalfTimer => (Timer % 20);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biome Blade"); //This is litterally just the biome blade without even any extra magic effects so
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 40;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override bool CanDamage() => false;

        public override void AI()
        {
            if (Math.Abs(HalfTimer / 20f) == 0.5f) //Why is this only playing on the first swing
            {
                var splash = new LegacySoundStyle(SoundID.Splash, 0).WithPitchVariance(Main.rand.NextFloat());
                Main.PlaySound(splash, projectile.Center);

                Projectile.NewProjectile(Owner.Center, direction * 0.1f + Owner.velocity , ProjectileType<GestureForTheDrownedOrb>(), projectile.damage, 0, Owner.whoAmI);

                Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.MediumTurquoise : Color.DarkOrange, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);

            }


            direction = Vector2.UnitY.RotatedBy((MathHelper.PiOver4 + Utils.AngleLerp(0f, MathHelper.PiOver4 * 1.5f, HalfTimer / 20f)) * Math.Sign(Timer-1 - 20f)) * (float)Math.Sin(HalfTimer / 20f * MathHelper.Pi) * 30;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }

            projectile.Center = Owner.Center + direction;
            projectile.rotation = direction.ToRotation() + MathHelper.PiOver2;
            projectile.spriteDirection = Math.Sign(Timer - 20f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D blade = GetTexture("CalamityMod/Items/Weapons/Melee/TrueBiomeBlade");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;
            Vector2 drawOrigin = new Vector2(0f, blade.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            spriteBatch.Draw(blade, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            return false;
        }

    }

    public class GestureForTheDrownedOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrowned";
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[projectile.owner];
        public ref float HasLanded => ref projectile.ai[0]; //Charge
        public float Timer => MaxTime - projectile.timeLeft;
        public const float MaxTime = 200f;
        public Particle[] FoamParticles = new Particle[5];

        public bool BounceX => projectile.oldPosition.X == projectile.position.X;



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gesture for the Drowned");

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;

            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = (int)MaxTime;
            projectile.melee = true;
            projectile.alpha = 90;
            projectile.ignoreWater = true;
            projectile.hide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            HasLanded = 1f;
            projectile.velocity.Y = -4f;
            return false;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {

            if (FoamParticles[0] == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 rot = projectile.velocity.RotatedByRandom(MathHelper.Pi).RotatedBy(-MathHelper.PiOver2);
                    Particle foam = new SeaFoamParticle(projectile.Center + rot, projectile.velocity, Color.LightSlateGray, Color.CornflowerBlue, Main.rand.NextFloat(0f, 0.5f), 150f, 0.1f);
                    GeneralParticleHandler.SpawnParticle(foam);
                    FoamParticles[i] = foam;
                }
            }

            if (projectile.timeLeft % 2 == 0)
            {
                int index = (projectile.timeLeft / 2) % 5;
                Particle foam = FoamParticles[index];
                Vector2 rotation = (Vector2.UnitX).RotatedByRandom(MathHelper.PiOver2).RotatedBy(-MathHelper.PiOver4) * 16f;
                rotation.X *= Math.Sign(projectile.velocity.X);
                foam.Position = projectile.Center + rotation;
                foam.Velocity = (-Vector2.UnitY * 5f).RotatedBy(MathHelper.PiOver4 * Math.Sign(projectile.velocity.X)) + new Vector2(projectile.velocity.X * -0.6f, 0f);
                //foam.Velocity = Vector2.Zero;
                (foam as SeaFoamParticle).Opacity = 150;
            }

            projectile.rotation += 0.1f * ((projectile.velocity.X > 0) ? 1f : -1f);

            if (Main.rand.NextBool())
            {
                Particle blob = new WaterGlobParticle(projectile.Center + Main.rand.NextVector2Circular(projectile.width / 2f, projectile.height / 2f), projectile.velocity / 2f, Main.rand.NextFloat(0.2f, 2f), 0.1f);
                GeneralParticleHandler.SpawnParticle(blob);
            }
            Particle foamSmall = new FakeGlowDust(projectile.Center + Main.rand.NextVector2Circular(projectile.width / 2f, projectile.height / 2f), projectile.velocity / 2f, Color.LightSlateGray, Main.rand.NextFloat(0.2f, 2f), 25, 0.1f, gravity: (-projectile.velocity + Vector2.UnitY) * 0.1f);
            GeneralParticleHandler.SpawnParticle(foamSmall);


            if (projectile.velocity.Y < 15f)
            {
                projectile.velocity.Y += 0.3f;
            }

            if (Math.Abs(projectile.velocity.X) < 5f)
            {
                projectile.velocity.X += 0.3f * ((projectile.velocity.X > 0) ? 1f : -1f);
            }

            if (BounceX)
            {
                //Die lmao
                projectile.Kill();
            }

            projectile.frameCounter++;
            if (projectile.frameCounter % 10 == 0)
                projectile.frame = (projectile.frame + 1) % 3;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            bool balled = (HasLanded == 0f);
            float? lowestGroundBelow = null;

            if (!balled)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 positionToCheck = projectile.Center;
                    if (Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16) + 1 * i].IsTileSolidGround() && lowestGroundBelow == null)
                    {
                        lowestGroundBelow = i * 16;
                        break;
                    }
                }
                if (lowestGroundBelow == null)
                {
                    balled = true;
                    HasLanded = 0f;
                }
            }

            if(!balled)
            { 
                Texture2D wave = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrownedWave");
                Rectangle frame = new Rectangle(0, 0 + 110 * projectile.frame, 40, 110);

                float drawRotation = 0f;
                Vector2 drawOrigin = new Vector2(0f, frame.Height);


                Vector2 drawOffset = projectile.Center + Vector2.UnitY * (float)lowestGroundBelow - Main.screenPosition;

                //Wobble
                Vector2 Scale = new Vector2(0.5f + 0.5f * (float)Math.Sin(Timer * 0.1), 0.5f + 0.5f * (1 - (float)Math.Sin(Timer * 0.1)));

                SpriteEffects flip = projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                spriteBatch.Draw(wave, drawOffset, frame, lightColor, drawRotation, drawOrigin, Scale, flip, 0f);
                return false;
            }

            else
            {
                Texture2D ball = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrowned");
                float drawRotation = projectile.rotation;
                Vector2 drawOrigin = projectile.Size / 2f;
                Vector2 drawOffset = projectile.Center - Main.screenPosition;

                spriteBatch.Draw(ball, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

                return false;
            }

        }

    }
}


