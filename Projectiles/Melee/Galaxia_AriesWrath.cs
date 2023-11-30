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
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class AriesWrath : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private NPC[] excludedTargets = new NPC[4];
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra2";
        public Player Owner => Main.player[Projectile.owner];
        public ref float ChainSwapTimer => ref Projectile.ai[0];
        public ref float BlastCooldown => ref Projectile.ai[1];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed && Owner.HeldItem.type == ItemType<FourSeasonsGalaxia>();

        const float MaxProjReach = 500f; //How far away do you check for enemies for the extra projs from crits be

        public Particle smear;
        public Projectile lastConstellation;

        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 80;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = FourSeasonsGalaxia.AriesAttunement_LocalIFrames;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.One * 50 * Projectile.scale, Vector2.One * 100 * Projectile.scale);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
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
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, target.SafeDirectionTo(potentialTarget.Center, Vector2.Zero) * 25f, ProjectileType<GalaxiaBolt>(), (int)(hit.Damage * FourSeasonsGalaxia.AriesAttunement_OnHitBoltDamageReduction), 0, Owner.whoAmI, 0.9f, MathHelper.PiOver4 * 0.4f);
                proj.scale = 2f;
            }
            Array.Clear(excludedTargets, 0, 3);
            BlastCooldown = 30f;

        }

        public NPC TargetNext(Vector2 hitFrom, int index)
        {
            float longestReach = MaxProjReach;
            NPC target = null;
            for (int i = 0; i < Main.maxNPCs; ++i)
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
                if ((Owner.Center - Projectile.Center).Length() < 30f || (Owner.Center - Projectile.Center).Length() > 2000f || Projectile.velocity.Length() > 100f)
                    Projectile.Kill();

                else
                {
                    if (Projectile.timeLeft <= 2)
                    {
                        Projectile.velocity *= 10f;
                    }
                    if (Projectile.velocity.AngleBetween(Owner.Center - Projectile.Center) > MathHelper.PiOver4)
                        Projectile.velocity = (Projectile.velocity.ToRotation().AngleTowards(Projectile.SafeDirectionTo(Owner.Center, Vector2.Zero).ToRotation(), MathHelper.Pi / 20f)).ToRotationVector2() * Projectile.velocity.Length() * 0.98f;
                    else
                        Projectile.velocity = (Projectile.velocity.ToRotation().AngleTowards(Projectile.SafeDirectionTo(Owner.Center, Vector2.Zero).ToRotation(), MathHelper.Pi)).ToRotationVector2() * Projectile.velocity.Length() * 1.05f;
                    Projectile.rotation = Main.GlobalTimeWrappedHourly * 25f;
                    Projectile.scale = MathHelper.Clamp((Owner.Center - Projectile.Center).Length() / (FourSeasonsGalaxia.AriesAttunement_Reach * 0.5f), 0.3f, 2f);
                    Projectile.timeLeft = 4;
                }
                return;
            }

            //On initialization basically
            if (ChainSwapTimer == 0f)
            {
                Projectile.Center = Owner.Center;
                SoundEngine.PlaySound(SoundID.Item120 with { Volume = SoundID.Item120.Volume * 0.5f }, Projectile.Center);

                if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;
            }

            Projectile.scale = 1f + ScaleEquation();
            Projectile.timeLeft = 2;

            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Calamity().mouseWorld, 0.05f * ThrowDisplace());
            Projectile.Center = Projectile.Center.MoveTowards(Owner.Calamity().mouseWorld, 40f * ThrowDisplace());

            if ((Projectile.Center - Owner.Center).Length() > FourSeasonsGalaxia.AriesAttunement_Reach)
                Projectile.Center = Owner.Center + Owner.SafeDirectionTo(Projectile.Center, Vector2.Zero) * FourSeasonsGalaxia.AriesAttunement_Reach;

            Projectile.rotation = Main.GlobalTimeWrappedHourly * 25f;
            //Make the owner look like theyre "holding" the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Owner.SafeDirectionTo(Projectile.Center, Vector2.Zero);
            Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
            Owner.itemRotation = Projectile.velocity.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            if (smear == null)
            {
                smear = new CircularSmearSmokeyVFX(Projectile.Center, Color.MediumOrchid, Projectile.rotation, Projectile.scale);
                GeneralParticleHandler.SpawnParticle(smear);
            }
            if (smear != null)
            {
                smear.Position = Projectile.Center;
                smear.Rotation = Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4;
                smear.Time = 0;
                smear.Scale = Projectile.scale;
                smear.Color.A = (byte)(255 * MathHelper.Clamp(ChainSwapTimer / 50f, 0, 1));
            }

            if (Main.rand.NextBool())
            {
                float maxDistance = Projectile.scale * 82f;
                Vector2 distance = Main.rand.NextVector2Circular(maxDistance, maxDistance);
                Vector2 angularVelocity = Utils.SafeNormalize(distance.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * 2f * (1f + distance.Length() / 15f);
                Particle glitter = new CritSpark(Projectile.Center + distance, angularVelocity, Main.rand.Next(3) == 0 ? Color.HotPink : Color.Plum, Color.DarkOrchid, 1f + 1 * (distance.Length() / maxDistance), 10, 0.05f, 3f);
                GeneralParticleHandler.SpawnParticle(glitter);
            }

            float smokeDistance = Projectile.scale * 62f;
            Vector2 smokePos = Main.rand.NextVector2Circular(smokeDistance, smokeDistance);
            Vector2 smokeSpeed = Utils.SafeNormalize(smokePos.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * 0.1f * (1f + smokePos.Length() / 15f);
            Particle smoke = new HeavySmokeParticle(Projectile.Center + smokePos, smokeSpeed, Color.Lerp(Color.Navy, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.4f, 1f) * Projectile.scale, 0.8f, 0, false, 0, true);
            GeneralParticleHandler.SpawnParticle(smoke);

            if (Main.rand.Next(3) == 0)
            {
                Particle smokeGlow = new HeavySmokeParticle(Projectile.Center + smokePos, smokeSpeed, Main.hslToRgb(0.85f, 1, 0.5f), 20, Main.rand.NextFloat(0.4f, 1f) * Projectile.scale, 0.8f, 0, true, 0.01f, true);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            if ((lastConstellation == null || !lastConstellation.active) && Owner.whoAmI == Main.myPlayer && ChainSwapTimer > 20)
            {
                lastConstellation = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ProjectileType<AriesWrathConstellation>(), (int)(Projectile.damage * FourSeasonsGalaxia.AriesAttunement_ChainDamageReduction), 0, Owner.whoAmI);
            }

            ChainSwapTimer++;
            BlastCooldown--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sword = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/GalaxiaExtra2").Value;

            Vector2 drawPos = Projectile.Center;
            Vector2 drawOrigin = sword.Size() / 2f;
            float drawRotation = Projectile.rotation + MathHelper.PiOver4;

            Main.EntitySpriteDraw(sword, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            return false;
        }
    }
}
