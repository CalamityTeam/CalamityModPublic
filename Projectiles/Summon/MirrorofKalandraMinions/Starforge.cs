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
    public class Starforge : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Projectile.Center.MinionHoming(MirrorofKalandra.TargetDistanceDetection, Owner);
        public ref float TimerToBoom => ref Projectile.ai[0];
        public ref float DrawSpin => ref Projectile.ai[1];
        public ref float Oscillation => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 12000;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = MirrorofKalandra.Purple_IFrames;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 92;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            CheckMinionExistence();

            if (Main.rand.NextBool(3))
            {
                MediumMistParticle mist = new MediumMistParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2),
                    Main.rand.NextVector2Circular(4f, 4f),
                    Color.DarkMagenta,
                    Color.Magenta,
                    Main.rand.NextFloat(.9f, 1.1f),
                    Main.rand.NextFloat(140f, 150f));
                GeneralParticleHandler.SpawnParticle(mist);

                int flavorDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 272, 0, 0, 0, default, .6f);
                Main.dust[flavorDust].noGravity = true;
            }

            if (Target is not null)
            {
                // The distance to the target plus a small number so it's not 0, it'd break calculations.
                float distanceToTarget = Projectile.Distance(Target.Center) + .01f;

                // The minion will head towards it's rotation.
                // If the target's close, the minion'll speed up, and viceversa, so it doesn't circle around the target doing nothing.
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * (MirrorofKalandra.Purple_MinRamSpeed + (12f / (distanceToTarget * .01f)));
                Projectile.velocity = Vector2.Clamp(Projectile.velocity, Vector2.One * -MirrorofKalandra.Purple_MaxRamSpeed, Vector2.One * MirrorofKalandra.Purple_MaxRamSpeed);
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(Target.Center), .001f * distanceToTarget);

                TimerToBoom++;
            }
            else if (Target is null)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center + Projectile.rotation.ToRotationVector2() * (MirrorofKalandra.IdleDistanceFromPlayer + MirrorofKalandra.IdleDistanceFromPlayer * (MathF.Sin(Oscillation) / MirrorofKalandra.OscillationRange)), .4f);
                Projectile.velocity = Vector2.Zero;
                Projectile.rotation = Projectile.rotation.AngleLerp(-MathHelper.PiOver2 - MathHelper.PiOver4 / 1.5f, .15f);
                Oscillation += MirrorofKalandra.OscillationSpeed;
            }
        }

        public void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<KalandraMirrorBuff>(), 3600);
            if (Projectile.type != ModContent.ProjectileType<Starforge>())
                return;

            if (Owner.dead)
                ModdedOwner.KalandraMirror = false;
            if (ModdedOwner.KalandraMirror)
                Projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // If enough time has passed and an enemy is hit, boom.
            if (TimerToBoom >= MirrorofKalandra.Purple_BlastFireRate && Main.myPlayer == Projectile.owner)
            {
                int blast = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<StarforgeBlast>(),
                    (int)(Projectile.damage * MirrorofKalandra.Purple_BlastDMGModifier),
                    Projectile.knockBack,
                    Owner.whoAmI);

                if (Main.projectile.IndexInRange(blast))
                    Main.projectile[blast].originalDamage = (int)(Projectile.originalDamage / MirrorofKalandra.Purple_BlastDMGModifier);

                TimerToBoom = 0f;
            }

            SparkParticle sparkOnHit = new SparkParticle(Vector2.Lerp(Projectile.Center, target.Center, .8f),
                -Projectile.velocity * .01f,
                false,
                20,
                Main.rand.NextFloat(1.2f, 1.8f),
                Color.Purple);
            GeneralParticleHandler.SpawnParticle(sparkOnHit);

            SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound with { PitchVariance = .5f } with { Volume = .2f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            DrawSpin -= MathHelper.ToRadians(MirrorofKalandra.Purple_SpinSpeed);
            float rotation = (Target is not null) ? DrawSpin : Projectile.rotation + MathHelper.Pi - MathHelper.PiOver4;

            if (CalamityConfig.Instance.Afterimages && Target is not null)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Color afterimageDrawColor = Color.Purple with { A = 125 } * Projectile.Opacity * (1f - i / (float)Projectile.oldPos.Length);
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, rotation, origin, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
                }
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

            return false;
        }
    }
}
