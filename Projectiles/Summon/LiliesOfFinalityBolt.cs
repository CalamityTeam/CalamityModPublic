using System;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Items.Weapons.Summon.LiliesOfFinality;

namespace CalamityMod.Projectiles.Summon
{
    public class LiliesOfFinalityBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private ref float RandomColorOffset => ref Projectile.localAI[0];

        private const int TimeBeforeHoming = 10;
        private const int TimeDying = 15;

        private SlotId LoopingSound;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = TimeBeforeHoming + Ariane_BoltTimeHoming + TimeDying;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.MinionHoming(MaxEnemyDistanceDetection, Main.player[Projectile.owner]);
            if (target is not null && Projectile.timeLeft > TimeDying && Projectile.timeLeft < Ariane_BoltTimeHoming)
            {
                float turnRate = Utils.Remap(Projectile.timeLeft, Ariane_BoltTimeHoming, TimeDying, Ariane_MinTurnRate, Ariane_MaxTurnRate);
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(Projectile.SafeDirectionTo(target.Center).ToRotation(), turnRate).ToRotationVector2() * Ariane_BoltProjectileSpeed;
            }

            // When the projectile's abut to die, it'll stop in place and spawn some dust to cover the shader suddenly cutting off.
            if (Projectile.timeLeft < TimeDying - 1)
            {
                Projectile.velocity = Vector2.Zero;

                // If on a dedicated server, don't bother running the visuals and sounds to save resources.
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust fadingDust = Dust.NewDustDirect(
                            Projectile.position,
                            Projectile.width,
                            Projectile.height,
                            CommonDustID,
                            Scale: Main.rand.NextFloat(0.8f, 1.2f));
                        fadingDust.noGravity = true;
                    }
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            // If on a dedicated server, don't bother running the visuals and sounds to save resources.
            if (Main.dedServ)
                return;

            if (Main.rand.NextBool())
            {
                Dust trailDust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(10f, 10f),
                    CommonDustID,
                    Projectile.velocity * Main.rand.NextFloat(0.01f, 0.05f),
                    Scale: Main.rand.NextFloat(0.4f, 0.6f));
                trailDust.noGravity = true;
            }

            if (SoundEngine.TryGetActiveSound(LoopingSound, out var sound) && sound.IsPlaying)
                sound.Position = Projectile.Center;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Since we don't want all the projectiles on screen to cycle through the same colors at the same time, we set an initial offset to the color.
            RandomColorOffset = Main.rand.NextFloat(100f);

            LoopingSound = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ArianeShot") with { Volume = 0.2f }, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Since we don't want the trail to instantly disappear on hit, we'll make reduce its time alive to near death.
            Projectile.timeLeft = TimeDying;

            SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.5f, Pitch = 0.4f }, Projectile.Center);
            if (SoundEngine.TryGetActiveSound(LoopingSound, out var sound))
                sound.Stop();
        }

        private float WidthFunction(float completionRatio) => MathHelper.Lerp(0f, 32f, MathF.Pow(completionRatio, 1f / 2.5f));

        private Color ColorFunction(float completionRatio)
        {
            float offsetTime = Main.GlobalTimeWrappedHourly + RandomColorOffset;
            float fadeToEnd = MathHelper.Lerp(0.65f, 1f, (float)Math.Cos(-offsetTime * 3f) * 0.5f + 0.5f);
            float fadeOpacity = Utils.GetLerpValue(1f, 0.64f, completionRatio, true) * Projectile.Opacity;
            Color endColor = Color.Lerp(Color.Fuchsia, Color.Red, (float)Math.Sin(completionRatio * MathHelper.Pi * 1.6f - offsetTime * 4f) * 0.5f + 0.5f);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * fadeOpacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new PrimitiveSettings(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f, pixelate: false, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 32);
            return false;
        }
    }
}
