using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityHex : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int TargetNPCIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float LemniscateAngle
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public float Time
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public int BookProjectileIndex
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        public const int Lifetime = 310;
        public const float BossLifeMaxDamageMult = 1f / 350f;
        public const float NormalEnemyLifeMaxDamageMult = 1f / 100f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 63;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (TargetNPCIndex >= Main.npc.Length || TargetNPCIndex < 0)
            {
                DeathDust();
                Projectile.Kill();
                return;
            }

            NPC target = Main.npc[TargetNPCIndex];

            // Delete the hex (and everything else by extension) if any necessary components are incorrect/would cause errors.
            if (BookProjectileIndex >= Main.projectile.Length || BookProjectileIndex < 0 || Time < 0)
            {
                DeathDust();
                Projectile.Kill();
                return;
            }

            if (!target.active)
            {
                NPC potentialTarget = Main.MouseWorld.ClosestNPCAt(4400f, true, true);
                if (potentialTarget != null)
                {
                    // If something happens to the original NPC, such as death, attempt to locate a new target and attack to them.
                    ChooseNewTarget(potentialTarget);
                    target = potentialTarget;
                }
                // If there is no NPC to attack to, die.
                else
                {
                    DeathDust();
                    Projectile.Kill();
                    return;
                }
            }

            Projectile book = Main.projectile[BookProjectileIndex];

            if (!book.active)
            {
                DeathDust();
                Projectile.Kill();
                return;
            }

            Time++;

            // Generate a field of dust with a color that fades to black with time in the shape of a Lemniscate of Bernoulli.
            for (int i = 0; i < 3; i++)
            {
                LemniscateAngle += MathHelper.TwoPi / 200f;
                DetermineLemniscatePosition(target);
            }

            if (Time >= Lifetime * Projectile.MaxUpdates)
            {
                Projectile.Kill();
                return;
            }


            float effectRate = MathHelper.Lerp(0.4f, 1f, Time / (Lifetime * Projectile.MaxUpdates - 40));
            float random = Main.rand.NextFloat();

            Projectile.Opacity = Utils.GetLerpValue(Lifetime * Projectile.MaxUpdates, (Lifetime - 60f) * Projectile.MaxUpdates, Time, true);

            // Spawn a bunch of swirling dust around the target's position, hexing them.
            if (random <= effectRate)
                SpawnSwirlingDust(target);

            // Randomly strike the target directly.
            if (Main.myPlayer == Projectile.owner && random <= effectRate / 20f && !target.immortal && !target.dontTakeDamage && !target.townNPC)
            {
                int damage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(Eternity.BaseDamage * 3f);
                int strike = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, Projectile.owner, target.whoAmI);
                if (Main.projectile.IndexInRange(strike))
                    Main.projectile[strike].DamageType = DamageClass.Magic;
            }

            // Release bursts of homing dark magic bolts periodically. The amount of bolts that can be summoned has a hard limit.
            // This is where most of the damage comes from. Be careful when messing with this.
            if ((int)Time % 30 == 0 && CalamityUtils.CountProjectiles(ModContent.ProjectileType<EternityHoming>()) < Eternity.MaxHomers)
            {
                int homerCount = 6;
                int damage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(0.8f * Eternity.BaseDamage);
                for (int i = 0; i < homerCount; i++)
                {
                    Vector2 velocity = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / homerCount * i).RotatedByRandom(0.3f) * 10f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + velocity * 4f, velocity, ModContent.ProjectileType<EternityHoming>(), damage, 0f, Projectile.owner, TargetNPCIndex);
                }
            }
        }

        public void DeathDust()
        {
            for (int i = 0; i < 44; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Eternity.DustID, newColor: new Color(245, 112, 218));
                dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 6f);
                dust.noGravity = true;
            }
        }

        public void DetermineLemniscatePosition(NPC target)
        {
            // This value causes the lemniscate to smoothen out and look better.
            // This added factor results in Lemniscate of Bernoulli instead of a Lemniscate of Gerono. The latter is considerably more squashed and
            // looks less cool.
            float scale = 2f / (3f - (float)Math.Cos(2 * LemniscateAngle));

            float outwardMultiplier = MathHelper.Lerp(4f, 220f, Utils.GetLerpValue(0f, 120f, Time, true));
            Vector2 lemniscateOffset = scale * new Vector2((float)Math.Cos(LemniscateAngle), (float)Math.Sin(2f * LemniscateAngle) / 2f);
            Projectile.Center = target.Center + lemniscateOffset * outwardMultiplier;
        }

        public void ChooseNewTarget(NPC newTarget)
        {
            TargetNPCIndex = newTarget.whoAmI;

            // Adjust the target index for the other components of the projectile.
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active)
                    continue;
                if (proj.owner != Projectile.owner)
                    continue;
                if (proj.type != ModContent.ProjectileType<EternityCrystal>() && proj.type != ModContent.ProjectileType<EternityCircle>())
                    continue;

                proj.ai[0] = TargetNPCIndex;
                DeathDust();
            }
        }

        public static void SpawnSwirlingDust(NPC target)
        {
            for (int i = 0; i < 3; i++)
            {
                float randomAngle = Main.rand.NextFloat() * MathHelper.TwoPi;
                float outwardnessFactor = Main.rand.NextFloat();
                Vector2 spawnPosition = target.Center + randomAngle.ToRotationVector2() * MathHelper.Lerp(70f, EternityCircle.TargetOffsetRadius - 60f, outwardnessFactor);
                Vector2 velocity = (randomAngle - 3f * MathHelper.Pi / 8f).ToRotationVector2() * (10f + 9f * Main.rand.NextFloat() + 4f * outwardnessFactor);
                Dust swirlingDust = Dust.NewDustPerfect(spawnPosition, 267, new Vector2?(velocity), 0, Main.rand.NextBool(3) ? Eternity.BlueColor : Eternity.PinkColor, 1.4f);
                swirlingDust.scale = 1.2f;
                swirlingDust.fadeIn = 0.25f + outwardnessFactor * 0.1f;
                swirlingDust.noGravity = true;
            }
        }

        public Color PrimitiveColorFunction(float completionRatio)
        {
            float leftoverTimeScale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f;
            leftoverTimeScale *= 0.5f;

            Color headColor = Color.Lerp(Color.Black, Color.Magenta, 0.1f);
            Color tailColor = Color.Lerp(Color.Magenta, Color.Cyan, completionRatio * 0.5f + leftoverTimeScale);
            float opacity = (float)Math.Pow(Utils.GetLerpValue(1f, 0.61f, completionRatio, true), 0.4) * Projectile.Opacity;
            float fadeToMagenta = MathHelper.SmoothStep(0f, 1f, (float)Math.Pow(completionRatio, 0.6D));

            return Color.Lerp(headColor, tailColor, fadeToMagenta) * opacity;
        }

        public static float PrimitiveWidthFunction(float completionRatio)
        {
            float widthInterpolant = Utils.GetLerpValue(0f, 0.12f, completionRatio, true);
            return MathHelper.SmoothStep(1f, 10f, widthInterpolant);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak"));
            PrimitiveSet.Prepare(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 84);
            return false;
        }
    }
}
