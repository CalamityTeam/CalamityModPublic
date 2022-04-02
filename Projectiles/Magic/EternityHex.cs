using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityHex : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        internal PrimitiveTrail LemniscateDrawer = null;
        public int TargetNPCIndex
        {
            get => (int)projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float LemniscateAngle
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public float Time
        {
            get => projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public int BookProjectileIndex
        {
            get => (int)projectile.localAI[1];
            set => projectile.localAI[1] = value;
        }
        public const int Lifetime = 310;
        public const float BossLifeMaxDamageMult = 1f / 350f;
        public const float NormalEnemyLifeMaxDamageMult = 1f / 100f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 63;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (TargetNPCIndex >= Main.npc.Length || TargetNPCIndex < 0)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            NPC target = Main.npc[TargetNPCIndex];

            // Delete the hex (and everything else by extension) if any necessary components are incorrect/would cause errors.
            if (BookProjectileIndex >= Main.projectile.Length || BookProjectileIndex < 0 || Time < 0)
            {
                DeathDust();
                projectile.Kill();
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
                    projectile.Kill();
                    return;
                }
            }

            Projectile book = Main.projectile[BookProjectileIndex];

            if (!book.active)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            Time++;

            // Generate a field of dust with a color that fades to black with time in the shape of a Lemniscate of Bernoulli.
            for (int i = 0; i < 3; i++)
            {
                LemniscateAngle += MathHelper.TwoPi / 200f;
                DetermineLemniscatePosition(target);
            }

            if (Time < Lifetime * projectile.MaxUpdates)
            {
                float effectRate = MathHelper.Lerp(0.4f, 1f, Time / (Lifetime * projectile.MaxUpdates - 40));
                float random = Main.rand.NextFloat();

                projectile.Opacity = Utils.InverseLerp(Lifetime * projectile.MaxUpdates, (Lifetime - 60f) * projectile.MaxUpdates, Time, true);

                // Spawn a bunch of swirling dust and do damage.
                if (random <= effectRate)
                    SpawnSwirlingDust(target);

                if (random <= effectRate / 30f)
                {
                    if (!target.immortal && !target.dontTakeDamage && !target.townNPC)
                    {
                        int damage = 2;
                        damage += (int)Math.Sqrt(target.width * target.height) * 10; // Damage done to Leviathan based on this formula = floor(sqrt(850 * 450) * 10) = 6184 damage.
                        damage += (int)(target.lifeMax * (target.boss ? BossLifeMaxDamageMult : NormalEnemyLifeMaxDamageMult));
                        damage += target.damage * 5;
                        damage = (int)(damage * Main.rand.NextFloat(0.9f, 1.1f));
                        damage = (int)MathHelper.Clamp(damage, 1f, Eternity.BaseDamage * player.MagicDamage() * 3);
                        target.StrikeNPC(damage, 0f, 0, false);
                        RegisterDPS(damage);
                    }
                }
                // This is where most of the damage comes from. Be careful when messing with this.
                if ((int)Time % 30 == 0 && CalamityUtils.CountProjectiles(ModContent.ProjectileType<EternityHoming>()) < Eternity.MaxHomers)
                {
                    int homerCount = 6;
                    int damage = (int)(Eternity.BaseDamage * player.MagicDamage() * 0.8f);

                    for (int i = 0; i < homerCount; i++)
                    {
                        Vector2 velocity = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / homerCount * i).RotatedByRandom(0.3f) * 10f;
                        Projectile.NewProjectile(target.Center + velocity * 4f, velocity, ModContent.ProjectileType<EternityHoming>(), damage, 0f, projectile.owner, TargetNPCIndex);
                    }
                }
            }
            else
            {
                projectile.Kill();
            }
        }

        public void DeathDust()
        {
            for (int i = 0; i < 44; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, Eternity.DustID, newColor: new Color(245, 112, 218));
                dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 6f);
                dust.noGravity = true;
            }
        }

        public void DetermineLemniscatePosition(NPC target)
        {
            // This value causes the lemniscate to smoothen out and look better.
            float scale = 2f / (3f - (float)Math.Cos(2 * LemniscateAngle));

            float outwardMultiplier = MathHelper.Lerp(4f, 220f, Utils.InverseLerp(0f, 120f, Time, true));
            Vector2 lemniscateOffset = scale * new Vector2((float)Math.Cos(LemniscateAngle), (float)Math.Sin(2f * LemniscateAngle) / 2f);

            projectile.Center = target.Center + lemniscateOffset * outwardMultiplier;
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
                if (proj.owner != projectile.owner)
                    continue;
                if (proj.type != ModContent.ProjectileType<EternityCrystal>() && proj.type != ModContent.ProjectileType<EternityCircle>())
                    continue;

                proj.ai[0] = TargetNPCIndex;
                DeathDust();
            }
        }

        public void SpawnSwirlingDust(NPC target)
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

        // So that the player can gauge the DPS of this weapon effectively (StrikeNPC alone will not register the DPS to the player. I have to do this myself).
        public void RegisterDPS(int damage)
        {
            Main.player[projectile.owner].addDPS(damage);
        }

        public Color PrimitiveColorFunction(float completionRatio)
        {
            float leftoverTimeScale = (float)Math.Sin(Main.GlobalTime * 4f) * 0.5f + 0.5f;
            leftoverTimeScale *= 0.5f;

            Color headColor = Color.Lerp(Color.Black, Color.Magenta, 0.1f);
            Color tailColor = Color.Lerp(Color.Magenta, Color.Cyan, completionRatio * 0.5f + leftoverTimeScale);
            float opacity = (float)Math.Pow(Utils.InverseLerp(1f, 0.61f, completionRatio, true), 0.4) * projectile.Opacity;
            float fadeToMagenta = MathHelper.SmoothStep(0f, 1f, (float)Math.Pow(completionRatio, 0.6D));

            return Color.Lerp(headColor, tailColor, fadeToMagenta) * opacity;
        }

        public float PrimitiveWidthFunction(float completionRatio)
        {
            float widthInterpolant = Utils.InverseLerp(0f, 0.12f, completionRatio, true);
            return MathHelper.SmoothStep(1f, 10f, widthInterpolant);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (LemniscateDrawer is null)
                LemniscateDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction, null, GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/EternityStreak"));
            LemniscateDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 84);
            return false;
        }
    }
}
