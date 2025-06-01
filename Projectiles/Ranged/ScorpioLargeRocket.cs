using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Items.Weapons.Ranged.Scorpio;
using static CalamityMod.Projectiles.Ranged.ScorpioHoldout;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class ScorpioLargeRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public ref float RocketID => ref Projectile.ai[0];
        public ref float ProjectileSpeed => ref Projectile.ai[1];

        public static int Lifetime = 600;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.MaxUpdates = 3;
            Projectile.width = Projectile.height = 15;
            Projectile.timeLeft = Lifetime;
            Projectile.localNPCHitCooldown = -1;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.ClosestNPCAt(NukeEnemyDistanceDetection);
            if (target is not null)
            {
                Vector2 targetDirection = Projectile.SafeDirectionTo(target.Center);

                // If the projectile is aligned a certain amount to the direction to the target, it gets small homing.
                float trackingSpeed = Vector2.Dot(targetDirection, Projectile.rotation.ToRotationVector2()) > NukeRequiredRotationProximity ? NukeTrackingSpeed : 0f;

                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetDirection.ToRotation(), trackingSpeed).ToRotationVector2() * ProjectileSpeed;
            }

            Projectile.velocity *= 1.032f;

            if (Projectile.wet && RocketID == ItemID.DryRocket && RocketID == ItemID.WetRocket && RocketID == ItemID.LavaRocket && RocketID == ItemID.HoneyRocket)
                Projectile.Kill();

            // Every X frames it changes the projectile to its next animation frame.
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                // Cycles through the 4 frames of the animation.
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }

            // Rotates towards its velocity.
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            // The projectile will fade away as its time alive is ending.
            Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, 30f, 0f, 0f, 255f);

            Dust trailDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustEffectsID, Scale: Main.rand.NextFloat(0.8f, 1f));
            trailDust.noGravity = true;
            trailDust.noLight = true;
            trailDust.noLightEmittence = true;

            if (Projectile.timeLeft < Lifetime - 5)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center - Projectile.velocity * 2f, Projectile.velocity * 0.01f, false, 8, 1.3f, StaticEffectsColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Projectile.timeLeft % 3 == 0)
            {
                Particle nanoDust = new NanoParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)), Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor, Main.rand.NextFloat(0.65f, 0.9f), Main.rand.Next(15, 20 + 1), Main.rand.NextBool(), true);
                GeneralParticleHandler.SpawnParticle(nanoDust);
            }

            Particle blastRing = new DirectionalPulseRing(
                Projectile.Center + Projectile.velocity * 1.5f,
                Vector2.Zero,
                StaticEffectsColor * 2,
                Vector2.One,
                0f,
                0.25f,
                0.25f,
                2);
            GeneralParticleHandler.SpawnParticle(blastRing);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 15f;
            if (Projectile.numHits == 1)
                Projectile.damage = (int)(Projectile.damage * .89f); // Reduction in damage if the hits are more than one, mostly for Deus
            if (Projectile.numHits > 1)
                Projectile.damage = (int)(Projectile.damage * 0.92f); // 8% penalty on explosion hits
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.SourceDamage *= 1.8f;

        public override void OnKill(int timeLeft)
        {
            var info = new CalamityUtils.RocketBehaviorInfo((int)RocketID)
            {
                smallRadius = 13,
                mediumRadius = 26,
                largeRadius = 40
            };
            int blastRadius = Projectile.RocketBehavior(info);
            Projectile.ExpandHitboxBy(blastRadius);
            Projectile.Damage();

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            int dustAmount = Main.rand.Next(30, 35 + 1);
            for (int i = 0; i < dustAmount; i++)
            {
                Dust boomDust = Dust.NewDustPerfect(Projectile.Center, DustEffectsID, (MathHelper.TwoPi / dustAmount * i).ToRotationVector2() * Main.rand.NextFloat(4f, 10f), Scale: Main.rand.NextFloat(1.2f, 1.75f));
                boomDust.noGravity = true;
                boomDust.noLight = true;
                boomDust.noLightEmittence = true;
            }

            for (int i = 0; i < 40; i++)
            {
                float blastVel = Projectile.width / 33f;
                Vector2 velocity = new Vector2(blastVel, blastVel).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.7f);
                Particle nanoDust = new NanoParticle(Projectile.Center, velocity, Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor, Main.rand.NextFloat(2f, 3f), 45, Main.rand.NextBool(), true);
                GeneralParticleHandler.SpawnParticle(nanoDust);
            }

            Particle smallBlastRing = new DirectionalPulseRing(
                Projectile.Center,
                Vector2.Zero,
                StaticEffectsColor,
                Vector2.One,
                0f,
                Projectile.width / 2180f,
                Projectile.width / 312f,
                40);
            GeneralParticleHandler.SpawnParticle(smallBlastRing);

            Particle blastRing = new DirectionalPulseRing(
                Projectile.Center,
                Vector2.Zero,
                EffectsColor * 0.6f,
                Vector2.One,
                0f,
                Projectile.width / 1755f,
                Projectile.width / 175f,
                20);
            GeneralParticleHandler.SpawnParticle(blastRing);

            Vector2 BurstFXDirection = new Vector2(15, 0);
            for (int i = 0; i < 4; i++)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center, (BurstFXDirection) * (i + 1), false, 11, 5f - i * 0.6f, StaticEffectsColor * 0.8f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int i = 0; i < 4; i++)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center, (-BurstFXDirection) * (i + 1), false, 11, 5f - i * 0.6f, StaticEffectsColor * 0.8f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            Particle orb = new GenericBloom(Projectile.Center, Vector2.Zero, StaticEffectsColor, 2, 13, false);
            GeneralParticleHandler.SpawnParticle(orb);
            Particle orb2 = new GenericBloom(Projectile.Center, Vector2.Zero, Color.White, 1.5f, 12, false);
            GeneralParticleHandler.SpawnParticle(orb2);

            SoundEngine.PlaySound(RocketHit, Projectile.Center);
            SoundEngine.PlaySound(NukeHit, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 1)
                Main.player[Projectile.owner].Calamity().GeneralScreenShakePower = 6f;
        }

        public float TrailWidthFunction(float completionRatio) => Utils.Remap(completionRatio, 0f, 0.8f, 15f, 0f);
        public Color TrailColorFunction(float completionRatio) => Color.Lerp(EffectsColor, EffectsColor * 0.3f, Utils.GetLerpValue(0f, 0.8f, completionRatio)) * Utils.GetLerpValue(255f, 0f, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTexture = Request<Texture2D>("CalamityMod/Projectiles/Ranged/ScorpioLargeRocket_Glow").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + MathHelper.PiOver2;
            Vector2 rotationPoint = frame.Size() * 0.5f;

            // 29FEB2024: Ozzatron: hopefully ported this correctly to the new prim system by Toasty
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(TrailWidthFunction, TrailColorFunction, (_) => Projectile.Size * 0.5f, smoothen: false, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 25);

            Main.EntitySpriteDraw(texture, drawPosition, frame, drawColor, drawRotation, rotationPoint, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glowTexture, drawPosition, frame, Color.White, drawRotation, rotationPoint, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}
