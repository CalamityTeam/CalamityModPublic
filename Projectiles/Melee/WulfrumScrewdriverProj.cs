using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class WulfrumScrewdriverProj : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<WulfrumScrewdriver>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/WulfrumScrewdriver";
        public float Timer => MaxTime - Projectile.timeLeft;
        public float LifetimeCompletion => Timer / (float)MaxTime;

        public static int MaxTime = 14;
        public ref float EndLag => ref Projectile.ai[0];
        public ref float TrueDirection => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public static Asset<Texture2D> SmearTex;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 14;
            Projectile.height = 50;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = MaxTime;
        }

        public override bool? CanDamage()
        {
            return Projectile.timeLeft <= (MaxTime - 5);
        }

        public override bool ShouldUpdatePosition() => false;
        public CurveSegment ThrustSegment = new CurveSegment(LinearEasing, 0f, 0f, 1f, 3);
        public CurveSegment HoldSegment = new CurveSegment(SineBumpEasing, 0.2f, 1f, 0.2f);
        public CurveSegment RetractSegment = new CurveSegment(PolyOutEasing, 0.76f, 1f, -0.8f, 3);
        public CurveSegment BumpSegment = new CurveSegment(SineBumpEasing, 0.9f, 0.2f, 0.15f);
        internal float DistanceFromPlayer => PiecewiseAnimation(LifetimeCompletion, new CurveSegment[] { ThrustSegment, HoldSegment,  RetractSegment, BumpSegment });
        public Vector2 OffsetFromPlayer => Projectile.velocity * DistanceFromPlayer * 12f;


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLength = 78f * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.MountedCenter + OffsetFromPlayer, Owner.MountedCenter + OffsetFromPlayer + (Projectile.velocity * bladeLength), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (EndLag == 0) //Initialization
            {
                EndLag = (float)Math.Max(Owner.ActiveItem().useTime - MaxTime, 1);
                TrueDirection = (Owner.Calamity().mouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.Zero).ToRotation(); //Store this for the screw hit
                Projectile.velocity = (Owner.Calamity().mouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.15f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            //Manage position and rotation
            Projectile.Center = Owner.MountedCenter + OffsetFromPlayer ;
            Projectile.scale = 1f + (float)Math.Sin(LifetimeCompletion * MathHelper.Pi) * 0.2f; //SWAGGER

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(MathF.Sign(Projectile.velocity.X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() * Owner.gravDir - MathHelper.PiOver2);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;


            //Check for launchable screws.
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.ModProjectile != null && proj.owner == Projectile.owner && proj.ModProjectile is WulfrumScrew screw && screw.BazingaTime == 0)
                {
                    float collisionPoint = 0f;
                    float bladeLength = 86f * Projectile.scale;
                    if (Collision.CheckAABBvLineCollision(proj.Hitbox.TopLeft(), proj.Hitbox.Size(), Owner.Center + OffsetFromPlayer, Owner.Center + OffsetFromPlayer + (Projectile.velocity * bladeLength), 34, ref collisionPoint))
                    {
                        Vector2 thudVelocity = TrueDirection.ToRotationVector2() * 6f;
                        NPC potentialAimAssist = FindTarget();
                        if (potentialAimAssist != null)
                            thudVelocity = (potentialAimAssist.Center - proj.Center).SafeNormalize(Vector2.Zero) * 6f;

                        screw.BazingaTime = WulfrumScrew.BazingaTimeMax;
                        proj.velocity = thudVelocity;
                        if (screw.AlreadyBazinged == 0)
                            proj.damage = (int)(proj.damage * WulfrumScrewdriver.ScrewBazingaModeDamageMult);
                        proj.timeLeft = WulfrumScrew.Lifetime;
                        proj.knockBack *= 2.5f;
                        screw.AlreadyBazinged++;

                        SoundEngine.PlaySound(WulfrumScrewdriver.ScrewHitSound, Projectile.Center);

                        if (screw.AlreadyBazinged > 2)
                            SoundEngine.PlaySound(WulfrumScrewdriver.FunnyUltrablingSound, Projectile.Center);
                        

                        if (Main.myPlayer == proj.owner)
                        {
                            Owner.Calamity().GeneralScreenShakePower = 6f;
                        }

                        return;
                    }
                }
            }
        }

        public NPC FindTarget()
        {
            float bestScore = 0;
            NPC bestTarget = null;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC potentialTarget = Main.npc[i];

                if (!potentialTarget.CanBeChasedBy(null, false))
                    continue;

                float distance = potentialTarget.Distance(Projectile.Center);
                float angle = TrueDirection.ToRotationVector2().AngleBetween((potentialTarget.Center - Projectile.Center));

                float extraDistance = potentialTarget.width / 2 + potentialTarget.height / 2;

                if (distance - extraDistance < WulfrumScrewdriver.ScrewBazingaAimAssistReach && angle < WulfrumScrewdriver.ScrewBazingaAimAssistAngle / 2f)
                {
                    if (!Collision.CanHit(Projectile.Center, 1, 1, potentialTarget.Center, 1, 1) && extraDistance < distance)
                        continue;

                    float attemptedScore = EvaluatePotentialTarget(distance - extraDistance, angle / 2f);
                    if (attemptedScore > bestScore)
                    {
                        bestTarget = potentialTarget;
                        bestScore = attemptedScore;
                    }
                }
            }
            return bestTarget;
        }

        public float EvaluatePotentialTarget(float distance, float angle)
        {
            float score = 1 - distance / WulfrumScrewdriver.ScrewBazingaAimAssistReach * 0.2f;
            //Prioritize angle over distance
            score += (1 - Math.Abs(angle) / (WulfrumScrewdriver.ScrewBazingaAimAssistAngle / 2f)) * 0.8f;

            return score;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(WulfrumScrewdriver.ThudSound, target.Center);
            Projectile.timeLeft = 0;

            //Chance to gain a screw
            if (Main.rand.NextBool(5) && Main.myPlayer == Owner.whoAmI)
            {
                if (Owner.HeldItem.ModItem is WulfrumScrewdriver screwdriver && !screwdriver.ScrewStored)
                {
                    WulfrumScrewdriver.ScrewStart = new Vector3(target.Center + Projectile.velocity * 14f * Main.rand.NextFloat() - Main.screenPosition, Main.rand.NextFloat(MathHelper.PiOver2 - MathHelper.PiOver4));
                    WulfrumScrewdriver.ScrewTimer = WulfrumScrewdriver.ScrewTime;
                    WulfrumScrewdriver.ScrewQeuedForStorage = true;

                    SoundEngine.PlaySound(SoundID.Item156);
                }
            }

            //Dust
            for (int k = 0; k < 4; k++)
            {
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 70f, 16, Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(6), 0, default, Main.rand.NextFloat(0.7f, 1f));
            }


            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.numHits == 0)
            {
                Owner.itemTime = (int)EndLag;
                Owner.itemAnimation = (int)EndLag;
            }

            //Go into jojo spam mode if you hit an enemy
            else
            {
                Owner.itemTime = 0;
                Owner.itemAnimation = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            if (SmearTex == null)
                SmearTex = ModContent.Request<Texture2D>("CalamityMod/Particles/MediumLongThrust");
            Texture2D smearTex = SmearTex.Value;

            Vector2 drawOrigin = new Vector2(tex.Width / 2f, tex.Height);
            Vector2 scale = new Vector2(Math.Abs((float)Math.Sin(LifetimeCompletion * MathHelper.TwoPi * 0.5f)), 1f);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, scale * Projectile.scale, 0f, 0);

            if (LifetimeCompletion < 0.6f)
            {
                int frameCount = (int)Math.Floor((LifetimeCompletion / 0.6f) * 3f);
                Rectangle frame = new Rectangle(0, (smearTex.Height / 3) * frameCount, smearTex.Width, smearTex.Height / 3);
                float opacity = 1 - (float)Math.Pow(LifetimeCompletion / 0.6f, 0.5f);

                Main.spriteBatch.Draw(smearTex, Projectile.Center + Projectile.velocity * 67f - Main.screenPosition, frame, Color.White * opacity, Projectile.rotation, frame.Size() / 2f, 0.9f, 0, 0);
            }
            return false;
        }
    }
}
