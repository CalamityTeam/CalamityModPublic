using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Typeless
{
    public class LunicBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            float aiTrack = 5f;
            float scaleFactor = 6f;
            Vector2 dustRotation = new Vector2(10f, 20f);
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                244,
                229,
                262,
                247
            });
            int solarDustType = 259;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (Projectile.ai[1] == 1f && Projectile.owner == Main.myPlayer)
            {
                int targetID = -1;
                float hitDistance = 150f;
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
                if (hitDistance < 8f)
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
                    if (npcDirection.Length() < 20f)
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
            else if (Projectile.alpha == 0)
            {
                for (int j = 0; j < 2; j++)
                {
                    Vector2 rotateFirstDust = Vector2.UnitX * -30f;
                    rotateFirstDust = -Vector2.UnitY.RotatedBy((double)(Projectile.localAI[0] * 0.1308997f + (float)j * 3.14159274f), default) * dustRotation - Projectile.rotation.ToRotationVector2() * 10f;
                    int solarDust = Dust.NewDust(Projectile.Center, 0, 0, solarDustType, 0f, 0f, 160, default, 1f);
                    Main.dust[solarDust].scale = 1f;
                    Main.dust[solarDust].noGravity = true;
                    Main.dust[solarDust].position = Projectile.Center + rotateFirstDust + Projectile.velocity * 2f;
                    Main.dust[solarDust].velocity = Vector2.Normalize(Projectile.Center + Projectile.velocity * 2f * 8f - Main.dust[solarDust].position) * 2f + Projectile.velocity * 2f;
                }
            }
            if (Main.rand.NextBool(12))
            {
                for (int k = 0; k < 1; k++)
                {
                    Vector2 rotateSecondDust = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int smokyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1f);
                    Main.dust[smokyDust].velocity *= 0.1f;
                    Main.dust[smokyDust].position = Projectile.Center + rotateSecondDust * (float)Projectile.width / 2f + Projectile.velocity * 2f;
                    Main.dust[smokyDust].fadeIn = 0.9f;
                }
            }
            if (Main.rand.NextBool(64))
            {
                for (int l = 0; l < 1; l++)
                {
                    Vector2 rotateThirdDust = -Vector2.UnitX.RotatedByRandom(0.39269909262657166).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int smokyDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 155, default, 0.8f);
                    Main.dust[smokyDust2].velocity *= 0.3f;
                    Main.dust[smokyDust2].position = Projectile.Center + rotateThirdDust * (float)Projectile.width / 2f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[smokyDust2].fadeIn = 1.4f;
                    }
                }
            }
            if (Main.rand.NextBool(4))
            {
                for (int m = 0; m < 2; m++)
                {
                    Vector2 rotateFourthDust = -Vector2.UnitX.RotatedByRandom(0.78539818525314331).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int randomDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1.2f);
                    Main.dust[randomDust].velocity *= 0.3f;
                    Main.dust[randomDust].noGravity = true;
                    Main.dust[randomDust].position = Projectile.Center + rotateFourthDust * (float)Projectile.width / 2f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[randomDust].fadeIn = 1.4f;
                    }
                }
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 rotateFifthDust = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int solarDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, solarDustType, 0f, 0f, 100, default, 1f);
                Main.dust[solarDust2].velocity *= 0.3f;
                Main.dust[solarDust2].position = Projectile.Center + rotateFifthDust * (float)Projectile.width / 2f;
                Main.dust[solarDust2].fadeIn = 1.2f;
                Main.dust[solarDust2].scale = 1.5f;
                Main.dust[solarDust2].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.25f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.25f / 255f);
            for (int r = 0; r < 3; r++)
            {
                int moreSolarDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - 28, Projectile.height - 28, 259, 0f, 0f, 100, default, 1.35f);
                Main.dust[moreSolarDust].noGravity = true;
                Main.dust[moreSolarDust].velocity *= 0.1f;
                Main.dust[moreSolarDust].velocity += Projectile.velocity * 0.5f;
            }
            if (Main.rand.NextBool(8))
            {
                int mostSolarDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - 32, Projectile.height - 32, 259, 0f, 0f, 100, default, 1f);
                Main.dust[mostSolarDust].velocity *= 0.25f;
                Main.dust[mostSolarDust].noGravity = true;
                Main.dust[mostSolarDust].velocity += Projectile.velocity * 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 480);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 480);

        public override void OnKill(int timeLeft)
        {
            int m = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                244,
                229,
                262,
                247
            });
            int randomDust = 259;
            int height = 50;
            float solarDust2 = 1.7f;
            Vector2 dustRotate = (Projectile.rotation - 1.57079637f).ToRotationVector2();
            Vector2 dustVel = dustRotate * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            int inc;
            for (int j = 0; j < 40; j = inc + 1)
            {
                m = Utils.SelectRandom(Main.rand, new int[]
                {
                    246,
                    244,
                    229,
                    262,
                    247
                });
                int orangeDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, m, 0f, 0f, 200, default, solarDust2);
                Dust dust = Main.dust[orangeDust];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
                dust.velocity += dustVel * Main.rand.NextFloat();
                orangeDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, default, 0.8f);
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += dustVel * Main.rand.NextFloat();
                inc = j;
            }
            for (int k = 0; k < 20; k = inc + 1)
            {
                int deathSolar = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 259, 0f, 0f, 0, default, 2f);
                Dust dust = Main.dust[deathSolar];
                dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.velocity += dustVel * (0.6f + 0.6f * Main.rand.NextFloat());
                inc = k;
            }
        }
    }
}
