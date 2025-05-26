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
    public class ScorpioRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public ref float RocketID => ref Projectile.ai[0];
        public ref float ProjectileSpeed => ref Projectile.ai[1];
        public ref float Time => ref Projectile.ai[2];

        public static float TimeToLaunch = 15f;
        public static float TimeForFullPropulsion = 10f;

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
            Projectile.width = Projectile.height = 34;
            Projectile.timeLeft = 600;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            // Don't target the enemy immediately, the rocket will slow down as it's being shot, to give it more flavor.
            if (Time >= TimeToLaunch)
            {
                NPC target = Projectile.Center.ClosestNPCAt(EnemyDetectionDistance);
                float projectileSpeed = Utils.Remap(Time, TimeToLaunch, TimeToLaunch + TimeForFullPropulsion, 1f, ProjectileSpeed);
                if (target is not null)
                {
                    float targetDirectionRotation = Projectile.SafeDirectionTo(target.Center).ToRotation();
                    float turningRate = Utils.Remap(Time, TimeToLaunch, TimeToLaunch + TimeForFullPropulsion, 0.01f, TrackingSpeed);
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetDirectionRotation, turningRate).ToRotationVector2() * projectileSpeed;
                }
                else
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * projectileSpeed;

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
            }
            else
                Projectile.velocity *= 0.9f;

            if (Projectile.wet && RocketID == ItemID.DryRocket && RocketID == ItemID.WetRocket && RocketID == ItemID.LavaRocket && RocketID == ItemID.HoneyRocket)
                Projectile.Kill();

            // Rotates towards its velocity.
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }

            Time++;
        }

        public override void OnKill(int timeLeft)
        {
            var info = new CalamityUtils.RocketBehaviorInfo((int)RocketID);
            int blastRadius = Projectile.RocketBehavior(info);
            Projectile.ExpandHitboxBy((float)blastRadius);
            Projectile.Damage();

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            int dustAmount = Main.rand.Next(25, 30 + 1);
            for (int i = 0; i < dustAmount; i++)
            {
                Dust boomDust = Dust.NewDustPerfect(Projectile.Center, DustEffectsID, (MathHelper.TwoPi / dustAmount * i).ToRotationVector2() * Main.rand.NextFloat(4f, 10f), Scale: Main.rand.NextFloat(1f, 1.45f));
                boomDust.noGravity = true;
                boomDust.noLight = true;
                boomDust.noLightEmittence = true;
            }

            for (int i = 0; i < 7; i++)
            {
                float blastVel = Projectile.width / 35f;
                Vector2 velocity = new Vector2(blastVel, blastVel).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.5f);
                Particle nanoDust = new NanoParticle(Projectile.Center, velocity, Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor, Main.rand.NextFloat(1f, 1.5f), 40, Main.rand.NextBool(), true);
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

            SoundEngine.PlaySound(RocketHit, Projectile.Center);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 1f;
            if (Projectile.numHits > 1)
                Projectile.damage = (int)(Projectile.damage * 0.2f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        public override bool? CanDamage() => Time >= TimeToLaunch ? null : false;

        public float TrailWidthFunction(float completionRatio) => Utils.Remap(completionRatio, 0f, 0.8f, 6f, 0f);
        public Color TrailColorFunction(float completionRatio) => Color.Lerp(EffectsColor, StaticEffectsColor * 0.75f, Utils.GetLerpValue(0f, 0.5f, completionRatio)) * Utils.GetLerpValue(255f, 0f, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTexture = Request<Texture2D>("CalamityMod/Projectiles/Ranged/ScorpioRocket_Glow").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + MathHelper.PiOver2;
            Vector2 rotationPoint = frame.Size() * 0.5f;

            if (Time >= TimeToLaunch)
            {
                // 29FEB2024: Ozzatron: hopefully ported this correctly to the new prim system by Toasty
                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak"));
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(TrailWidthFunction, TrailColorFunction, (_) => Projectile.Size * 0.5f, smoothen: false, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 25);
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, drawColor, drawRotation, rotationPoint, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glowTexture, drawPosition, frame, Color.White, drawRotation, rotationPoint, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}
