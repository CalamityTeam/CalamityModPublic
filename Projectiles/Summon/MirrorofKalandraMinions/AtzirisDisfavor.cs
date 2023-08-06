using System;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.MirrorofKalandraMinions
{
    public class AtzirisDisfavor : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Projectile.Center.MinionHoming(MirrorofKalandra.TargetDistanceDetection, Owner);
        public ref float DrawSpin => ref Projectile.ai[0];
        public ref float Oscillation => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 12000;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = MirrorofKalandra.Axe_IFrames;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 112;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            CheckMinionExistence();

            // Flavor visual dust effect.
            if (Main.rand.NextBool(3))
            {
                int flavorDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 228, 0, 0, 0, default, 1.2f);
                Main.dust[flavorDust].noGravity = true;
            }
            
            if (Target is not null)
            {
                // The distance to the target plus a small number so it's not 0, it'd break calculations.
                float distanceToTarget = Projectile.Distance(Target.Center) + .01f;

                // The minion will head towards it's rotation.
                // If the target's close, the minion'll speed up, and viceversa, so it doesn't circle around the target doing nothing.
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * (MirrorofKalandra.Axe_MinRamSpeed + (12f / (distanceToTarget * .01f)));
                Projectile.velocity = Vector2.Clamp(Projectile.velocity, Vector2.One * -MirrorofKalandra.Axe_MaxRamSpeed, Vector2.One * MirrorofKalandra.Axe_MaxRamSpeed);
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(Target.Center), .001f * distanceToTarget);

                // Swing effect.
                SemiCircularSmearVFX swingTrail = new SemiCircularSmearVFX(Projectile.Center + (DrawSpin + MathHelper.Pi).ToRotationVector2() * 10f, // Weird vector calculation for better alignment.
                        Color.OrangeRed,
                        DrawSpin + MathHelper.PiOver2,
                        Projectile.scale * .8f,
                        Vector2.One);
                GeneralParticleHandler.SpawnParticle(swingTrail);

                // Repeating swing sound.
                if (Projectile.soundDelay <= 0)
                {
                    SoundEngine.PlaySound(CommonCalamitySounds.LouderSwingWoosh with { Pitch = -.8f, PitchVariance = 1f, Volume = .6f }, Projectile.Center);
                    Projectile.soundDelay = 15;
                    Projectile.netUpdate = true;
                }
            }
            else if (Target is null)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center,
                    Owner.Center + Projectile.rotation.ToRotationVector2() * (MirrorofKalandra.IdleDistanceFromPlayer + MirrorofKalandra.IdleDistanceFromPlayer * (MathF.Sin(Oscillation) / MirrorofKalandra.OscillationRange)),
                    .3f);
                Projectile.velocity = Vector2.Zero;
                Projectile.rotation = Projectile.rotation.AngleLerp(-MathHelper.PiOver2, .1f);
                Oscillation += MirrorofKalandra.OscillationSpeed;
            }
        }

        public void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<KalandraMirrorBuff>(), 3600);
            if (Projectile.type != ModContent.ProjectileType<AtzirisDisfavor>())
                return;

            if (Owner.dead)
                ModdedOwner.KalandraMirror = false;
            if (ModdedOwner.KalandraMirror)
                Projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ImpactParticle sparkHit = new ImpactParticle(Vector2.Lerp(Projectile.Center, target.Center, .8f),
                Main.rand.NextBool().ToDirectionInt() * Main.rand.NextFloat(.08f, .12f),
                20,
                1f,
                Color.Gold);
            GeneralParticleHandler.SpawnParticle(sparkHit);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            DrawSpin += MathHelper.ToRadians(MirrorofKalandra.Axe_SpinSpeed);
            float rotation = (Target is not null) ? DrawSpin : Projectile.rotation + MathHelper.PiOver4;

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
