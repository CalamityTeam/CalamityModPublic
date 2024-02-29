using System;
using System.IO;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class SnakeEyesProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Player Owner => Main.player[Projectile.owner];
        public NPC TargetShot => Main.npc[(int)TargetID];
        public NPC Target => Projectile.Center.MinionHoming(SnakeEyes.EnemyDistanceDetection, Owner);

        public ref float MinionID => ref Projectile.ai[0];
        public ref float TargetID => ref Projectile.ai[1];
        public ref float TimerToRedirect => ref Projectile.ai[2];

        public bool HasHitEnemy = false;
        public bool HasRedirected = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;

            // While this projectile doesn't have afterimages, it keeps track of old positions for its primitive drawcode.
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 10;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        #region Variable Syncing

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasHitEnemy);
            writer.Write(HasRedirected);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasHitEnemy = reader.ReadBoolean();
            HasRedirected = reader.ReadBoolean();
        }

        #endregion

        public override void AI()
        {
            if (HasHitEnemy && TimerToRedirect < SnakeEyes.TimeToRedirect)
                TimerToRedirect++;

            if (TimerToRedirect >= SnakeEyes.TimeToRedirect && !HasRedirected)
            {
                if (TargetShot is not null && TargetShot.active)
                    TargetEnemy(TargetShot);
                else if (Target is not null)
                    TargetEnemy(Target);
            }

            if (HasRedirected)
            {
                if (TargetShot is not null && TargetShot.active)
                    FollowEnemy(TargetShot);
                else if (Target is not null)
                    FollowEnemy(Target);
            }

            for (int i = 0; i < 2; i++)
            {
                Dust trailDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, HasRedirected ? 261 : 226, Scale: HasRedirected ? 2f : .5f);
                trailDust.velocity = Vector2.Zero;
                trailDust.noGravity = true;
            }
        }

        #region AI Methods

        private void TargetEnemy(NPC target)
        {
            Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * SnakeEyes.ProjectileSpeed;
            HasRedirected = true;

            SoundEngine.PlaySound(SoundID.Item92 with { Volume = .8f, Pitch = .5f, PitchVariance = .1f }, Projectile.Center);

            Projectile.netUpdate = true;
        }

        private void FollowEnemy(NPC target)
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * SnakeEyes.ProjectileSpeed, .2f);
        }

        #endregion

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HasHitEnemy = true;

            if (HasRedirected && TargetShot == target)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            if (!HasRedirected)
                return;

            Projectile.ExpandHitboxBy(300);
            Projectile.Damage();

            for (int i = 0; i < 20; i++)
            {
                Dust boomDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 261, Scale: 2f);
                boomDust.noGravity = true;
            }

            Particle boom = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.White, Vector2.One, 0f, 0.05f, 1f + Main.rand.NextFloat(0.2f), 20);
            GeneralParticleHandler.SpawnParticle(boom);

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }

        #region Shader Trail Methods

        private float PrimitiveWidthFunction(float completionRatio)
        {
            float arrowheadCutoff = 0.36f;
            float width = 39f;
            float minHeadWidth = 0.02f;
            float maxHeadWidth = width;
            if (completionRatio <= arrowheadCutoff)
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(0f, arrowheadCutoff, completionRatio, true));
            return width;
        }

        private Color PrimitiveColorFunction(float completionRatio)
        {
            float endFadeRatio = 0.41f;

            float completionRatioFactor = 2.7f;
            float globalTimeFactor = 5.3f;
            float endFadeFactor = 3.2f;
            float endFadeTerm = Utils.GetLerpValue(0f, endFadeRatio * 0.5f, completionRatio, true) * endFadeFactor;
            float cosArgument = completionRatio * completionRatioFactor - Main.GlobalTimeWrappedHourly * globalTimeFactor + endFadeTerm;
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f;

            float colorLerpFactor = 0.6f;
            Color startingColor = Color.Lerp(HasRedirected ? Color.White with { A = 50 } : new Color(0, 0, 0, 0), HasRedirected ? Color.White with { A = 100 } : Color.Cyan with { A = 25 }, startingInterpolant * colorLerpFactor);

            return Color.Lerp(startingColor, HasRedirected ? Color.White : Color.DarkCyan with { A = 50 }, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, endFadeRatio, completionRatio, true)));
        }

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak"));
            PrimitiveSet.Prepare(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 92);
            return false;
        }
    }
}
