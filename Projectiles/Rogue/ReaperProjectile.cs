using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Dusts;
using Microsoft.CodeAnalysis;

namespace CalamityMod.Projectiles.Rogue
{
    public class ReaperProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TheOldReaper";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = Projectile.MaxUpdates * 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 9; // can't hit too fast, but can hit many many times
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override void AI()
        {
            Projectile.rotation += 0.4f;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[1] += 1f;

                // If the Reaper lands a hit, switch to second behavior mode immediately.
                if (Projectile.ai[1] >= 60f || Projectile.numHits > 0)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }

                // Initial homing before landing a hit.
                else
                    CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 14f, 14f);
            }

            // Homing after landing a hit. This homing repeatedly turns on and off.
            else
            {
                float homingRange = 1100f; //tbh 700 works for fat targets but then we'll get so many bug reports cuz it doesnt work on dummies
                bool noHomingThisFrame = false;
                if (Projectile.ai[0] == 1f)
                {
                    Projectile.ai[1] += 1f;
                    if (Projectile.ai[1] > 30f)
                    {
                        Projectile.ai[1] = 0f;
                        Projectile.ai[0] = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                        noHomingThisFrame = true;
                }

                if (noHomingThisFrame)
                    return;

                Vector2 homingTarget = Projectile.Center;
                bool foundTarget = false;
                Vector2 targetVelocity = new Vector2(0f);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC potentialTarget = Main.npc[i];
                    if (potentialTarget.CanBeChasedBy(Projectile, false) && Projectile.WithinRange(Main.npc[i].Center, homingRange))
                    {
                        if (!foundTarget)
                        {
                            homingRange = Vector2.Distance(Projectile.Center, potentialTarget.Center);
                            homingTarget = potentialTarget.Center;
                            targetVelocity = potentialTarget.velocity;
                            foundTarget = true;
                            break;
                        }
                    }
                }


                if (foundTarget && Projectile.ai[0] == 0f)
                {
                    bool perfectAim = Main.rand.NextBool(3);
                    if (perfectAim)
                        Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, homingTarget, targetVelocity, Main.rand.NextFloat(18f, 20f));
                    else
                    {
                        float angularTurnSpeed = 0.35f;
                        float angleToTargetCoords = Projectile.AngleTo(homingTarget);
                        Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(angleToTargetCoords, angularTurnSpeed).ToRotationVector2() * Main.rand.NextFloat(18f, 20f);
                    }
                }

                if (Projectile.ai[1] > 0f)
                    Projectile.ai[1] += (float)Main.rand.Next(1, 5);
                if (Projectile.ai[1] > 30f)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }

                if (Projectile.ai[0] == 0f)
                {
                    if (Projectile.ai[1] == 0f && foundTarget && homingRange < 500f)
                    {
                        Projectile.ai[1] += 1f;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.ai[0] = 1f;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1)
            {
                //Glowing
                Main.spriteBatch.EnterShaderRegion();
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1.64f, 0.8f, 0.5f)); //Using RGB directly fails, gotta use HSL
                float opacityFactor = Projectile.ai[1] - 5;
                if (opacityFactor < 0f)
                    opacityFactor = 0f;
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(opacityFactor / 40f);
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                Main.spriteBatch.ExitShaderRegion();
            }
            else
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 8; j++)
            {
                int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 3f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2f);
                Main.dust[dust2].velocity *= 2f;
            }
        }
    }
}
