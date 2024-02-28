using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MineralMortarProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public Color FrontTrailColor;
        public Color BackTrailColor;
        public ref float RocketType => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 25f)
                Projectile.velocity.Y += 0.3f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            // Effects; no server syncing.
            if (!Main.dedServ)
            {
                bool randomDust = Main.rand.NextBool();
                Dust trailDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, randomDust ? 109 : DustID.Torch, Main.rand.NextFloat(3f), Main.rand.NextFloat(3f), Scale: randomDust ? Main.rand.NextFloat(1f, 1.5f) : Main.rand.NextFloat(1.5f, 2.5f));
                trailDust.noGravity = true;
                trailDust.noLight = true;
                trailDust.noLightEmittence = true;

                Particle smoke = new MediumMistParticle(Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), new Color(255, 100, 0, 100), Color.Transparent, Main.rand.NextFloat(.3f, 1f), Main.rand.NextFloat(300f, 500f));
                GeneralParticleHandler.SpawnParticle(smoke);
            }

            // Multiplayer syncing every second.
            if (Projectile.timeLeft % 60 == 0)
            {
                Projectile.netUpdate = true;
                if (Projectile.netSpam >= 10)
                    Projectile.netSpam = 9;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            switch (RocketType)
            {
                case ItemID.DryRocket:
                    FrontTrailColor = Color.Transparent;
                    BackTrailColor = Color.Transparent;
                    break;

                case ItemID.WetRocket:
                    FrontTrailColor = Color.DarkCyan;
                    BackTrailColor = Color.Blue;
                    break;

                case ItemID.LavaRocket:
                    FrontTrailColor = Color.Red;
                    BackTrailColor = Color.DarkRed;
                    break;

                case ItemID.HoneyRocket:
                    FrontTrailColor = Color.Yellow;
                    BackTrailColor = Color.Orange;
                    break;

                default:
                    FrontTrailColor = Color.Orange;
                    BackTrailColor = Color.DarkOrange;
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            const int SmallRadius = 4;
            const int MediumRadius = 7;
            const int BigRadius = 10;

            var info = new CalamityUtils.RocketBehaviorInfo((int)RocketType)
            {
                smallRadius = SmallRadius,
                mediumRadius = MediumRadius,
                largeRadius = BigRadius
            };
            int blastRadius = Projectile.RocketBehavior(info);
            Projectile.ExpandHitboxBy((float)blastRadius);
            Projectile.Damage();

            // Effects; no server syncing.
            if (!Main.dedServ)
            {
                float radius(float smallBoomRadius, float mediumBoomRadius, float bigBoomRadius)
                {
                    if (blastRadius == SmallRadius)
                        return smallBoomRadius;

                    if (blastRadius == MediumRadius)
                        return mediumBoomRadius;

                    if (blastRadius == BigRadius)
                        return bigBoomRadius;

                    return 0f;
                }

                Particle boom = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.Orange, Vector2.One, Main.rand.NextFloat(MathHelper.Pi), 0.1f, radius(0.37f, 0.65f, 0.93f), 15);
                GeneralParticleHandler.SpawnParticle(boom);

                for (int i = 1; i < 3; i++)
                {
                    Particle boomRing = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.DarkOrange, Vector2.One, 0f, radius(0.06f, 0.1f, 0.14f) * i, radius(0.57f, 1f, 1.43f) * i, 20);
                    GeneralParticleHandler.SpawnParticle(boomRing);
                }

                for (int i = 0; i < 30; i++)
                {
                    bool randomDust = Main.rand.NextBool();
                    Dust boomDust = Dust.NewDustPerfect(Projectile.Center, randomDust ? 109 : DustID.Torch, Main.rand.NextVector2Circular(10f, 10f), Scale: randomDust ? Main.rand.NextFloat(1f, 2f) : Main.rand.NextFloat(2.5f, 3f));
                    boomDust.noGravity = true;
                    boomDust.noLight = true;
                    boomDust.noLightEmittence = true;
                }

                int debriAmount = Main.rand.Next(10, 15 + 1);
                for (int debriIndex = 0; debriIndex < debriAmount; debriIndex++)
                {
                    float angle = MathHelper.TwoPi / debriAmount * debriIndex;
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(2f, 8f);
                    Particle debri = new StoneDebrisParticle(Projectile.Center, velocity, Color.Lerp(Color.White, Color.LightGray, Main.rand.NextFloat()), Main.rand.NextFloat(0.4f, 0.6f), Main.rand.Next(30, 45 + 1), Main.rand.NextFloat(MathHelper.Pi));
                    GeneralParticleHandler.SpawnParticle(debri);
                }

                int mistAmount = Main.rand.Next(5, 8 + 1);
                for (int mistIndex = 0; mistIndex < mistAmount; mistIndex++)
                {
                    float angle = MathHelper.TwoPi / mistAmount * mistIndex;
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(5f, 15f);
                    Particle boomMist = new MediumMistParticle(Projectile.Center, velocity, new Color(255, 100, 0), Color.Transparent, Main.rand.NextFloat(.6f, 1.4f), Main.rand.NextFloat(200f, 400f));
                    GeneralParticleHandler.SpawnParticle(boomMist);
                }

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/MineralMortarExplode") { PitchVariance = 0.2f }, Projectile.Center);
            }
        }

        public static float TrailWidthFunction(float completionRatio) => MathHelper.Lerp(20f, 0f, completionRatio);

        public Color ColorTrailFunction(float completionRatio) => Color.Lerp(BackTrailColor, FrontTrailColor, completionRatio);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + MathHelper.PiOver2;
            Vector2 origin = texture.Size() * 0.5f;

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SwordSlashTexture"));
            PrimitiveSet.Prepare(Projectile.oldPos, new(TrailWidthFunction, ColorTrailFunction, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 50);
            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < 3; i++)
                {
                    Color afterimageDrawColor = Color.DarkOrange with { A = 75 } * (1f - i / (float)Projectile.oldPos.Length);
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, afterimageDrawPosition, null, afterimageDrawColor, Projectile.oldRot[i] - MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture, position, null, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}
