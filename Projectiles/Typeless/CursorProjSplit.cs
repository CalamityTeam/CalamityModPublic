using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Typeless
{
    public class CursorProjSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Typeless/CursorProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 3;

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;

            float aiTrack = 5f;
            float scaleFactor = 6f;
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                242,
                229,
                226,
                247
            });
            int crystalDustType = 255;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (Projectile.ai[1] == 1f && Projectile.owner == Main.myPlayer)
            {
                if (Projectile.alpha < 128)
                {
                    int targetID = -1;
                    float hitDistance = 300f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].CanBeChasedBy(Projectile, false))
                        {
                            Vector2 targetCenter = Main.npc[i].Center;
                            float targetDist = Vector2.Distance(targetCenter, Projectile.Center);
                            if (targetDist < hitDistance && targetID == -1 && Collision.CanHitLine(Projectile.Center, 1, 1, targetCenter, 1, 1))
                            {
                                hitDistance = targetDist;
                                targetID = i;
                            }
                        }
                    }
                    if (hitDistance < 4f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (targetID != -1)
                    {
                        Projectile.ai[1] = aiTrack + 1f;
                        Projectile.ai[0] = (float)targetID;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (Projectile.ai[1] > aiTrack)
            {
                Projectile.ai[1] += 1f;
                int npcTrack = (int)Projectile.ai[0];
                if (!Main.npc[npcTrack].active || !Main.npc[npcTrack].CanBeChasedBy(Projectile, false))
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.velocity.ToRotation();
                    Vector2 npcDirection = Main.npc[npcTrack].Center - Projectile.Center;
                    if (npcDirection.Length() < 10f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (npcDirection != Vector2.Zero)
                    {
                        npcDirection.Normalize();
                        npcDirection *= scaleFactor;
                    }
                    Projectile.velocity = (Projectile.velocity * 29f + npcDirection) / 30f;
                }
            }
            if (Projectile.ai[1] >= 1f && Projectile.ai[1] < aiTrack)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] == aiTrack)
                {
                    Projectile.ai[1] = 1f;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 48f)
            {
                Projectile.localAI[0] = 0f;
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 rotateFirstDust = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int crystalDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, crystalDustType, 0f, 0f, 100, default, 1f);
                Main.dust[crystalDust].velocity *= 0.1f;
                Main.dust[crystalDust].position = Projectile.Center + rotateFirstDust * (float)Projectile.width / 2f + Projectile.velocity * 2f;
                Main.dust[crystalDust].fadeIn = 0.9f;
            }
            if (Main.rand.NextBool(18))
            {
                Vector2 rotateSecondDust = -Vector2.UnitX.RotatedByRandom(0.39269909262657166).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int greenDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 155, default, 0.8f);
                Main.dust[greenDust].velocity *= 0.3f;
                Main.dust[greenDust].position = Projectile.Center + rotateSecondDust * (float)Projectile.width / 2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[greenDust].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 rotateThirdDust = -Vector2.UnitX.RotatedByRandom(0.78539818525314331).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int randomDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Main.dust[randomDust].velocity *= 0.3f;
                Main.dust[randomDust].noGravity = true;
                Main.dust[randomDust].position = Projectile.Center + rotateThirdDust * (float)Projectile.width / 2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[randomDust].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 value13 = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int crystalDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, crystalDustType, 0f, 0f, 100, default, 1f);
                Main.dust[crystalDust2].velocity *= 0.3f;
                Main.dust[crystalDust2].position = Projectile.Center + value13 * (float)Projectile.width / 2f;
                Main.dust[crystalDust2].fadeIn = 1.2f;
                Main.dust[crystalDust2].scale = 1.5f;
                Main.dust[crystalDust2].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.2f / 255f);
            int paleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - 28, Projectile.height - 28, 234, 0f, 0f, 100, default, 0.8f);
            Main.dust[paleDust].velocity *= 0.1f;
            Main.dust[paleDust].velocity += Projectile.velocity * 0.5f;
            Main.dust[paleDust].noGravity = true;
            if (Main.rand.NextBool(12))
            {
                int shinyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - 32, Projectile.height - 32, 159, 0f, 0f, 100, default, 1f);
                Main.dust[shinyDust].velocity *= 0.25f;
                Main.dust[shinyDust].velocity += Projectile.velocity * 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Vaporfied>(), 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Vaporfied>(), 60);

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.alpha >= 128)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.alpha < 128;

        public override void OnKill(int timeLeft)
        {
            int otherDustType = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                242,
                229,
                226,
                247
            });

            int randomDust = 187;
            float crystalDust2 = 1.2f;

            Vector2 dustRotate = (Projectile.rotation - 1.57079637f).ToRotationVector2();
            Vector2 dustVel = dustRotate * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            int dustCount;
            for (int j = 0; j < 20; j = dustCount + 1)
            {
                int superRandomDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, otherDustType, 0f, 0f, 200, default, crystalDust2);
                Dust dust = Main.dust[superRandomDust];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 3f;
                dust.velocity += dustVel * Main.rand.NextFloat();
                superRandomDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, default, 0.6f);
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.velocity.Y -= 6f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Cyan * 0.5f;
                dust.velocity += dustVel * Main.rand.NextFloat();
                dustCount = j;
            }

            for (int k = 0; k < 10; k = dustCount + 1)
            {
                int palestDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 234, 0f, 0f, 0, default, 1.5f);
                Main.dust[palestDust].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                Dust dust = Main.dust[palestDust];
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 0.5f;
                dust.velocity += dustVel * (0.6f + 0.6f * Main.rand.NextFloat());
                dustCount = k;
            }
        }
    }
}
