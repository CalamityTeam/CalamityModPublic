using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class AriesWrath : ModProjectile
    {
        private NPC[] excludedTargets = new NPC[4];
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra2";
        public Player Owner => Main.player[projectile.owner];
        public ref float ChainSwapTimer => ref projectile.ai[0];
        public ref float BlastCooldown => ref projectile.ai[1];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed && Owner.HeldItem.type == ItemType<FourSeasonsGalaxia>();

        const float MaxProjReach = 500f; //How far away do you check for enemies for the extra projs from crits be

        public Particle smear;
        public Projectile lastConstellation;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aries's Wrath");

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;

        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 80;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = FourSeasonsGalaxia.AriesAttunement_LocalIFrames;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - Vector2.One * 50 * projectile.scale, Vector2.One * 100 * projectile.scale);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 sparkSpeed = Owner.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 9f;
                Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.HotPink, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 1f);
                GeneralParticleHandler.SpawnParticle(Spark);
            }

            Vector2 sliceDirection = Main.rand.NextVector2CircularEdge(50f, 100f);
            Particle SliceLine = new LineVFX(target.Center - sliceDirection, sliceDirection * 2f, 0.2f, Color.HotPink * 0.6f)
            {
                Lifetime = 6
            };
            GeneralParticleHandler.SpawnParticle(SliceLine);

            if (BlastCooldown > 0)
                return;

            excludedTargets[0] = target;
            for (int i = 0; i < 3; i++)
            {
                NPC potentialTarget = TargetNext(target.Center, i);
                if (potentialTarget == null)
                    break;
                Projectile proj = Projectile.NewProjectileDirect(target.Center, target.SafeDirectionTo(potentialTarget.Center, Vector2.Zero) * 25f, ProjectileType<GalaxiaBolt>(), (int)(damage * FourSeasonsGalaxia.AriesAttunement_OnHitBoltDamageReduction), 0, Owner.whoAmI, 0.9f, MathHelper.PiOver4 * 0.4f);
                proj.scale = 2f;
            }
            Array.Clear(excludedTargets, 0, 3);
            BlastCooldown = 30f;

        }

        public NPC TargetNext(Vector2 hitFrom, int index)
        {
            float longestReach = MaxProjReach;
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

        //Animation keys
        public CurveSegment slowIn = new CurveSegment(EasingType.PolyIn, 0f, 0.2f, 1f, 3);
        public CurveSegment bounce = new CurveSegment(EasingType.SineBump, 0.3f, 1f, 0.2f);
        public CurveSegment remain = new CurveSegment(EasingType.SineBump, 0.6f, 1f, -0.1f);
        internal float ThrowDisplace() => PiecewiseAnimation(MathHelper.Clamp(ChainSwapTimer / 40f, 0, 1), new CurveSegment[] { slowIn, bounce, remain });

        //Animation keys
        public CurveSegment scaleUp = new CurveSegment(EasingType.PolyIn, 0f, 0.2f, 1f, 3);
        public CurveSegment scaleDown = new CurveSegment(EasingType.SineBump, 0.3f, 1f, 0.2f);
        internal float ScaleEquation() => PiecewiseAnimation(MathHelper.Clamp(ChainSwapTimer / 30f, 0, 1), new CurveSegment[] { scaleUp, scaleDown });


        public override void AI()
        {
            if (!OwnerCanShoot)
            {
                //Kill the projectile if too far away from the player or close enough to get "re-absorbed)
                if ((Owner.Center - projectile.Center).Length() < 30f || (Owner.Center - projectile.Center).Length() > 2000f || projectile.velocity.Length() > 100f)
                    projectile.Kill();

                else
                {
                    if (projectile.timeLeft <= 2)
                    {
                        projectile.velocity *= 10f;
                    }
                    if (projectile.velocity.AngleBetween(Owner.Center - projectile.Center) > MathHelper.PiOver4)
                        projectile.velocity = (projectile.velocity.ToRotation().AngleTowards(projectile.SafeDirectionTo(Owner.Center, Vector2.Zero).ToRotation(), MathHelper.Pi / 20f)).ToRotationVector2() * projectile.velocity.Length() * 0.98f;
                    else
                        projectile.velocity = (projectile.velocity.ToRotation().AngleTowards(projectile.SafeDirectionTo(Owner.Center, Vector2.Zero).ToRotation(), MathHelper.Pi)).ToRotationVector2() * projectile.velocity.Length() * 1.05f;
                    projectile.rotation = Main.GlobalTime * 25f;
                    projectile.scale = MathHelper.Clamp((Owner.Center - projectile.Center).Length() / (FourSeasonsGalaxia.AriesAttunement_Reach * 0.5f), 0.3f, 2f);
                    projectile.timeLeft = 4;
                }
                return;
            }

            //On initialization basically
            if (ChainSwapTimer == 0f)
            {
                projectile.Center = Owner.Center;
                var scream = Main.PlaySound(SoundID.Item120, projectile.Center);
                SafeVolumeChange(ref scream, 0.5f);

                if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;
            }

            projectile.scale = 1f + ScaleEquation();
            projectile.timeLeft = 2;

            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Calamity().mouseWorld, 0.05f * ThrowDisplace());
            projectile.Center = projectile.Center.MoveTowards(Owner.Calamity().mouseWorld, 40f * ThrowDisplace());

            if ((projectile.Center - Owner.Center).Length() > FourSeasonsGalaxia.AriesAttunement_Reach)
                projectile.Center = Owner.Center + Owner.SafeDirectionTo(projectile.Center, Vector2.Zero) * FourSeasonsGalaxia.AriesAttunement_Reach;

            projectile.rotation = Main.GlobalTime * 25f;
            //Make the owner look like theyre "holding" the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            projectile.velocity = Owner.SafeDirectionTo(projectile.Center, Vector2.Zero);
            Owner.direction = Math.Sign(projectile.velocity.X);
            Owner.itemRotation = projectile.velocity.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            if (smear == null)
            {
                smear = new CircularSmearSmokeyVFX(projectile.Center, Color.MediumOrchid, projectile.rotation, projectile.scale);
                GeneralParticleHandler.SpawnParticle(smear);
            }
            if (smear != null)
            {
                smear.Position = projectile.Center;
                smear.Rotation = projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4;
                smear.Time = 0;
                smear.Scale = projectile.scale;
                smear.Color.A = (byte)(255 * MathHelper.Clamp(ChainSwapTimer / 50f, 0, 1));
            }

            if (Main.rand.NextBool())
            {
                float maxDistance = projectile.scale * 82f;
                Vector2 distance = Main.rand.NextVector2Circular(maxDistance, maxDistance);
                Vector2 angularVelocity = Utils.SafeNormalize(distance.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * 2f * (1f + distance.Length() / 15f);
                Particle glitter = new CritSpark(projectile.Center + distance, angularVelocity, Main.rand.Next(3) == 0 ? Color.HotPink : Color.Plum, Color.DarkOrchid, 1f + 1 * (distance.Length() / maxDistance), 10, 0.05f, 3f);
                GeneralParticleHandler.SpawnParticle(glitter);
            }

            float smokeDistance = projectile.scale * 62f;
            Vector2 smokePos = Main.rand.NextVector2Circular(smokeDistance, smokeDistance);
            Vector2 smokeSpeed = Utils.SafeNormalize(smokePos.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * 0.1f * (1f + smokePos.Length() / 15f);
            Particle smoke = new HeavySmokeParticle(projectile.Center + smokePos, smokeSpeed, Color.Lerp(Color.Navy, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.4f, 1f) * projectile.scale, 0.8f, 0, false, 0, true);
            GeneralParticleHandler.SpawnParticle(smoke);

            if (Main.rand.Next(3) == 0)
            {
                Particle smokeGlow = new HeavySmokeParticle(projectile.Center + smokePos, smokeSpeed, Main.hslToRgb(0.85f, 1, 0.5f), 20, Main.rand.NextFloat(0.4f, 1f) * projectile.scale, 0.8f, 0, true, 0.01f, true);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            if ((lastConstellation == null || !lastConstellation.active) && Owner.whoAmI == Main.myPlayer && ChainSwapTimer > 20)
            {
                lastConstellation = Projectile.NewProjectileDirect(Owner.Center, Vector2.Zero, ProjectileType<AriesWrathConstellation>(), (int)(projectile.damage * FourSeasonsGalaxia.AriesAttunement_ChainDamageReduction), 0, Owner.whoAmI);
            }

            ChainSwapTimer++;
            BlastCooldown--;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/GalaxiaExtra2");

            Vector2 drawPos = projectile.Center;
            Vector2 drawOrigin = sword.Size() / 2f;
            float drawRotation = projectile.rotation + MathHelper.PiOver4;

            spriteBatch.Draw(sword, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            return false;
        }
    }
}