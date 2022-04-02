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


namespace CalamityMod.Projectiles.Melee
{
	public class ArkoftheElementsParryHoldout : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/RendingScissorsRight";

        private bool initialized = false;
        const float MaxTime = 340;
        static float ParryTime = 15;
        public Vector2 DistanceFromPlayer => projectile.velocity * 10 + projectile.velocity * 10 * ThrustDisplaceRatio();
        public float Timer => MaxTime - projectile.timeLeft;
        public float ParryProgress => (MaxTime - projectile.timeLeft) / ParryTime;

        public ref float AlreadyParried => ref projectile.ai[1];
        public Player Owner => Main.player[projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Elements");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 75;
            projectile.width = projectile.height = 75;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
        }

        public override bool CanDamage() => Timer <= ParryTime && AlreadyParried == 0f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLenght = 142f * projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + DistanceFromPlayer, Owner.Center + DistanceFromPlayer + (projectile.velocity * bladeLenght), 44, ref collisionPoint);
        }

        public void GeneralParryEffects()
        {
            ArkoftheElements sword = (Owner.HeldItem.modItem as ArkoftheElements);
            if (sword != null)
            {
                sword.Charge = 10f;
                sword.Combo = 0f;
            }
            Main.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);
            var chunder = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ScissorGuillotineSnap"), projectile.Center);
            SafeVolumeChange(ref chunder, 1.3f);

            CombatText.NewText(projectile.Hitbox, new Color(111, 247, 200), "Parry!", true);

            for (int i = 0; i < 5; i ++) //Don't loose your way
            {
                Vector2 particleDispalce = Main.rand.NextVector2Circular(Owner.Hitbox.Width * 2f, Owner.Hitbox.Height * 1.2f);
                float particleScale = Main.rand.NextFloat(0.5f, 1.4f);
                Particle shine = new FlareShine(Owner.Center + particleDispalce, particleDispalce * 0.01f, Color.White, Color.Red, 0f, new Vector2(0.6f, 1f) * particleScale, new Vector2(1.5f, 2.7f) * particleScale, 20 + Main.rand.Next(6), bloomScale: 3f, spawnDelay : Main.rand.Next(7) * 2);
                GeneralParticleHandler.SpawnParticle(shine);
            }

            AlreadyParried = 1f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (AlreadyParried > 0)
                return;

            GeneralParryEffects();

            //only get iframes if the enemy has contact damage :)
            if (target.damage > 0)
                Owner.GiveIFrames(35);

            Vector2 particleOrigin = target.Hitbox.Size().Length() < 140 ? target.Center : projectile.Center + projectile.rotation.ToRotationVector2() * 60f;
            Particle spark = new GenericSparkle(particleOrigin, Vector2.Zero, Color.White, Color.HotPink, 1.2f, 35, 0.1f, 2);
            GeneralParticleHandler.SpawnParticle(spark);

            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(2.6f, 4f);
                Particle energyLeak = new SquishyLightParticle(particleOrigin, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                projectile.timeLeft = (int)MaxTime;
                var sound = Main.PlaySound(SoundID.Item84, projectile.Center);
                CalamityUtils.SafeVolumeChange(ref sound, 0.3f);

                projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                projectile.velocity.Normalize();
                projectile.rotation = projectile.velocity.ToRotation();

                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            //Manage position and rotation
            projectile.Center = Owner.Center + DistanceFromPlayer ;
            projectile.scale = 1.4f + ThrustDisplaceRatio() * 0.2f;

            if (Timer > ParryTime)
                return;
            
            float collisionPoint = 0f;
            float bladeLenght = 142f * projectile.scale;

            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile proj = Main.projectile[k];

                if (proj.active && proj.hostile && proj.damage > 1 && //Only parry harmful projectiles
                   proj.velocity.Length() * (proj.extraUpdates + 1) > 1f && //Only parry projectiles that move semi-quickly
                   proj.Size.Length() < 300 && //Only parry projectiles that aren't too large
                   Collision.CheckAABBvLineCollision(proj.Hitbox.TopLeft(), proj.Hitbox.Size(), Owner.Center + DistanceFromPlayer, Owner.Center + DistanceFromPlayer + (projectile.velocity * bladeLenght), 24, ref collisionPoint))
                {
                    if (AlreadyParried == 0)
                    {
                        GeneralParryEffects();
                        if (Owner.velocity.Y != 0)
                            Owner.velocity += Utils.SafeNormalize(Owner.Center - proj.Center, Vector2.Zero) * 2;
                    }

                    //Reduce the projectile's damage by 200 for a second.
                    if (proj.Calamity().damageReduction < 200)
                        proj.Calamity().damageReduction = 200;
                    if (proj.Calamity().damageReductionTimer < 60)
                        proj.Calamity().damageReductionTimer = 60;
                    break;
                }
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

            if (AlreadyParried > 0)
            {
                AlreadyParried++;
            }
        }

        //Animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.SineBump, 0f, 0.2f, -0.05f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyInOut, 0.2f, 0.2f, 0.8f, 2);
        public CurveSegment retract = new CurveSegment(EasingType.CircIn, 0.7f, 1f, -0.1f);
        internal float ThrustDisplaceRatio() => PiecewiseAnimation(ParryProgress, new CurveSegment[] { anticipation, thrust, retract });

        public CurveSegment openMore = new CurveSegment(EasingType.SineBump, 0f, 0f, -0.15f);
        public CurveSegment close = new CurveSegment(EasingType.PolyIn, 0.3f, 0f, 1f, 4);
        public CurveSegment stayClosed = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0f);
        internal float RotationRatio() => PiecewiseAnimation(ParryProgress, new CurveSegment[] { openMore, close, stayClosed });

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Stop drawing the sword. Draw a recharge bar instead
            if (Timer > ParryTime)
            {
                if (Main.myPlayer == Owner.whoAmI)
                {
                    var barBG = GetTexture("CalamityMod/ExtraTextures/GenericBarBack");
                    var barFG = GetTexture("CalamityMod/ExtraTextures/GenericBarFront");

                    Vector2 drawPos = Owner.Center - Main.screenPosition + new Vector2(0, -36) - barBG.Size() / 2;
                    Rectangle frame = new Rectangle(0, 0, (int)((Timer - ParryTime) / (MaxTime - ParryTime) * barFG.Width), barFG.Height);

                    float opacity = Timer <= ParryTime + 25f ? (Timer - ParryTime) / 25f : (MaxTime - Timer <= 8) ? projectile.timeLeft / 8f : 1f;
                    Color color = Main.hslToRgb((float)Math.Sin(Main.GlobalTime * 1.2f) * 0.05f + 0.08f, 1, 0.65f + (float)Math.Sin(Main.GlobalTime * 7f) * 0.1f);

                    spriteBatch.Draw(barBG, drawPos, color * opacity);
                    spriteBatch.Draw(barFG, drawPos, frame, color * opacity * 0.8f);
                }
                return false;
            }

            Texture2D frontBlade = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRight");
            Texture2D frontBladeGlow = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRightGlow");
            Texture2D backBlade = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeft");
            Texture2D backBladeGlow = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeftGlow");

            float snippingRotation = projectile.rotation + MathHelper.PiOver4;
            float snippingRotationBack = projectile.rotation + MathHelper.PiOver4 * 1.75f;

            float drawRotation = MathHelper.Lerp(snippingRotation + MathHelper.PiOver4, snippingRotation, RotationRatio());
            float drawRotationBack = MathHelper.Lerp(snippingRotationBack - MathHelper.PiOver4, snippingRotationBack, RotationRatio());

            Vector2 drawOrigin = new Vector2(51, 86); //Right on the hole
            Vector2 drawOriginBack = new Vector2(22, 109); //Right on the hole
            Vector2 drawPosition = Owner.Center + projectile.velocity * 15 + projectile.velocity * ThrustDisplaceRatio() * 50f - Main.screenPosition;

            spriteBatch.Draw(backBlade, drawPosition, null, lightColor, drawRotationBack, drawOriginBack, projectile.scale, 0f, 0f);
            spriteBatch.Draw(backBladeGlow, drawPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotationBack, drawOriginBack, projectile.scale, 0f, 0f);

            spriteBatch.Draw(frontBlade, drawPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            spriteBatch.Draw(frontBladeGlow, drawPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            //Play a blip when it dies, to indicate to the player its ready to get used again
            if (Main.myPlayer == Owner.whoAmI)
            {
                var ding = Main.PlaySound(SoundID.Item35);
                CalamityUtils.SafeVolumeChange(ref ding, 2f);
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
}