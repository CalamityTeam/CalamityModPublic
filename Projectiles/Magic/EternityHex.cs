using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityHex : ModProjectile
    {
        public const float bossLifeMaxDamageMult = 1f / 350f;
        public const float normalEnemyLifeMaxDamageMult = 1f / 100f;
        public const int trueTimeLeft = 310;
        public const int extraUpdateCount = 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.tileCollide = false;
            projectile.timeLeft = int.MaxValue; //Killed manually
            projectile.extraUpdates = extraUpdateCount;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            if (projectile.ai[0] >= Main.npc.Length || projectile.ai[0] < 0)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            NPC target = Main.npc[(int)projectile.ai[0]];

            if (!target.active)
            {
                NPC potentialNPC = projectile.Center.ClosestNPCAt(4400f, true);
                if (potentialNPC != null)
                {
                    projectile.ai[0] = potentialNPC.whoAmI;
                    target = Main.npc[(int)projectile.ai[0]];
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if ((proj.whoAmI == projectile.whoAmI ||
                            proj.type == ModContent.ProjectileType<EternityCrystal>()) &&
                            proj.active && proj.owner == projectile.owner)
                        {
                            if (proj.type == ModContent.ProjectileType<EternityCrystal>())
                                proj.ai[0] = projectile.ai[0];
                            for (int j = 0; j < 44; j++)
                            {
                                Dust dust = Dust.NewDustPerfect(proj.Center, Eternity.dustID, newColor: new Color(245, 112, 218));
                                dust.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(2f, 6f);
                                dust.noGravity = true;
                            }
                        }
                    }
                }
                else
                {
                    DeathDust();
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.localAI[1] >= Main.projectile.Length || projectile.localAI[0] < 0)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            Projectile book = Main.projectile[(int)projectile.localAI[1]];

            if (!book.active)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            projectile.localAI[0] += 1f;
            //Infinity loop in dust
            for (int i = 0; i < 3; i++)
            {
                projectile.ai[1] += MathHelper.TwoPi / 175f;

                //Causes the lemniscate to smoothen out and look better
                float scale = 2f / (3f - (float)Math.Cos(2 * projectile.ai[1]));
                float outwardMultiplier = MathHelper.Clamp(projectile.localAI[0] / 2f + 20f, 20f, 230f);
                Vector2 additiveVector = new Vector2(scale * (float)Math.Cos(projectile.ai[1]), scale * (float)Math.Sin(2f * projectile.ai[1]) / 2f);

                projectile.Center = target.Center + additiveVector * outwardMultiplier;

                float timeRatio = MathHelper.Clamp(projectile.localAI[0] / trueTimeLeft, 0f, 1f);
                Color dustColor = Color.Lerp(new Color(245, 112, 218), new Color(28, 13, 118), timeRatio);
                int dustIndex = Dust.NewDust(projectile.Center, 0, 0, Eternity.dustID, newColor: dustColor);
                Main.dust[dustIndex].velocity = Vector2.Zero;
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = MathHelper.Clamp(timeRatio * 1.5f, 0.6f, 1.5f);
            }
            if (projectile.localAI[0] < trueTimeLeft * 2 - 40)
            {
                float range = projectile.localAI[0] / (trueTimeLeft * 2f - 40);
                float random = Main.rand.NextFloat();
                if (random <= (range * 0.6f + 0.4f))
                {
                    for (int iterative = 0; iterative < 12; iterative++)
                    {
                        //For where on the imaginary circle the dust appears
                        float randTwoPi = Main.rand.NextFloat() * MathHelper.TwoPi;
                        //For determining the circle of the radius and the velocity
                        float rand01 = Main.rand.NextFloat();
                        Vector2 spawnPosition = target.Center + randTwoPi.ToRotationVector2() * (70f + 530f * rand01);
                        Vector2 velocity = (randTwoPi - 3f * MathHelper.Pi / 8f).ToRotationVector2() * (10f + 9f * Main.rand.NextFloat() + 4f * rand01);
                        Dust swirlingDust = Dust.NewDustPerfect(spawnPosition, Eternity.dustID, new Vector2?(velocity), 0, Main.rand.NextBool(3) ? Eternity.blueColor : Eternity.pinkColor, 1.4f);
                        swirlingDust.scale = 0.8f;
                        swirlingDust.fadeIn = 0.95f + rand01 * 0.3f;
                        swirlingDust.noGravity = true;
                    }
                }
                if (random <= (range * 0.6f + 0.4f) / 30f)
                {
                    if (!target.immortal &&
                        !target.dontTakeDamage)
                    {
                        int damage = 2;
                        //To give a bit of variety in how damage is calculated, I decided to incorporate width and height
                        damage += (int)Math.Sqrt(target.width * target.height) * 10; //Leviathan damage from this calculation = floor(sqrt(850, 450)) * 10 = 6184 damage
                        damage += (int)(target.boss ? target.lifeMax * bossLifeMaxDamageMult : target.lifeMax * normalEnemyLifeMaxDamageMult);
                        damage += target.damage * 5;
                        //The same damage value on each hit could cause the bugs channel to go apeshit lol
                        damage = (int)(damage * Main.rand.NextFloat(0.9f, 1.1f));
                        damage = (int)MathHelper.Clamp(damage, 1f, Eternity.BaseDamage * 3);
                        RegisterDPS(damage);

                        target.StrikeNPC(damage, 0f, 0, false);
                    }
                }
                //This is where most of the damage comes from. Be careful when messing with this
                if ((int)projectile.localAI[0] % 30 == 0 &&
                    CalamityUtils.CountProjectiles(ModContent.ProjectileType<EternityHoming>()) < Eternity.MaxHomers)
                {
                    int homerCount = 6;
                    int trueMeleeID = ModContent.ProjectileType<EternityHoming>();
                    int trueMeleeDamage = Eternity.BaseDamage;
                    float angleVariance = MathHelper.TwoPi / homerCount;
                    float spinOffsetAngle = MathHelper.Pi / (2f * homerCount);
                    Vector2 posVec = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 10f;

                    for (int i = 0; i < homerCount; ++i)
                    {
                        posVec = posVec.RotatedBy(angleVariance);
                        Vector2 velocity = new Vector2(posVec.X, posVec.Y).RotatedBy(spinOffsetAngle);
                        velocity = Vector2.Normalize(velocity) * 10f;
                        Projectile.NewProjectile(target.Center + posVec * 4f, velocity, trueMeleeID, trueMeleeDamage, 0f, projectile.owner, projectile.ai[0]);
                    }
                }
            }
            else if (projectile.localAI[0] < trueTimeLeft * 2)
            {
                if (Main.rand.NextBool(3))
                {
                    int damage = 4000;
                    damage = (int)(damage * Main.rand.NextFloat(0.9f, 1.1f));
                    RegisterDPS(damage);
                    target.StrikeNPC(damage, 0f, 0, false);
                    int totalProjectiles = Main.rand.Next(8, 14) * 2;
                    float spread = MathHelper.ToRadians(Main.rand.Next(24, 36));
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
                    double deltaAngle = spread / totalProjectiles; // Angle between each projectile
                    double offsetAngle;
                    float velocity = Main.rand.NextFloat(6f, 26f);

                    for (int i = 0; i < 6; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Dust.NewDustPerfect(target.Center, Eternity.dustID, -velocity * ((float)offsetAngle).ToRotationVector2(), 0, Eternity.pinkColor);
                        Dust.NewDustPerfect(target.Center, Eternity.dustID, velocity * ((float)offsetAngle).ToRotationVector2(), 0, Eternity.blueColor);
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
                Dust dust = Dust.NewDustPerfect(projectile.Center, Eternity.dustID, newColor: new Color(245, 112, 218));
                dust.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(2f, 6f);
                dust.noGravity = true;
            }
        }
        //So that the player can gauge the DPS of this weapon effectively (StrikeNPC alone will not register the DPS to the player. I have to do this myself)
        private void RegisterDPS(int damage)
        {
            Main.player[projectile.owner].addDPS(damage);
        }
    }
}