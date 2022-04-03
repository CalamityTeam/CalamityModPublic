using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class MagnusBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
            float num29 = 5f;
            float num30 = 150f;
            float scaleFactor = 6f;
            Vector2 value7 = new Vector2(10f, 20f);
            float num31 = 1f;
            int num32 = 3 * Projectile.MaxUpdates;
            int num33 = Utils.SelectRandom(Main.rand, new int[]
            {
                56,
                92,
                229,
                206,
                181
            });
            int num34 = 261;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (Projectile.ai[1] == 1f && Projectile.owner == Main.myPlayer)
            {
                int num35 = -1;
                float num36 = num30;
                for (int num37 = 0; num37 < 200; num37++)
                {
                    if (Main.npc[num37].active && Main.npc[num37].CanBeChasedBy(Projectile, false))
                    {
                        Vector2 center3 = Main.npc[num37].Center;
                        float num38 = Vector2.Distance(center3, Projectile.Center);
                        if (num38 < num36 && num35 == -1 && Collision.CanHitLine(Projectile.Center, 1, 1, center3, 1, 1))
                        {
                            num36 = num38;
                            num35 = num37;
                        }
                    }
                }
                if (num36 < 8f)
                {
                    Projectile.Kill();
                    return;
                }
                if (num35 != -1)
                {
                    Projectile.ai[1] = num29 + 1f;
                    Projectile.ai[0] = (float)num35;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.ai[1] > num29)
            {
                Projectile.ai[1] += 1f;
                int num39 = (int)Projectile.ai[0];
                if (!Main.npc[num39].active || !Main.npc[num39].CanBeChasedBy(Projectile, false))
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.velocity.ToRotation();
                    Vector2 vector6 = Main.npc[num39].Center - Projectile.Center;
                    if (vector6.Length() < 20f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (vector6 != Vector2.Zero)
                    {
                        vector6.Normalize();
                        vector6 *= scaleFactor;
                    }
                    float num40 = 30f;
                    Projectile.velocity = (Projectile.velocity * (num40 - 1f) + vector6) / num40;
                }
            }
            if (Projectile.ai[1] >= 1f && Projectile.ai[1] < num29)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] == num29)
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
                for (int num41 = 0; num41 < 2; num41++)
                {
                    Vector2 value8 = Vector2.UnitX * -30f;
                    value8 = -Vector2.UnitY.RotatedBy((double)(Projectile.localAI[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7 - Projectile.rotation.ToRotationVector2() * 10f;
                    int num42 = Dust.NewDust(Projectile.Center, 0, 0, num34, 0f, 0f, 160, default, 1f);
                    Main.dust[num42].scale = num31;
                    Main.dust[num42].noGravity = true;
                    Main.dust[num42].position = Projectile.Center + value8 + Projectile.velocity * 2f;
                    Main.dust[num42].velocity = Vector2.Normalize(Projectile.Center + Projectile.velocity * 2f * 8f - Main.dust[num42].position) * 2f + Projectile.velocity * 2f;
                }
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 value9 = -Vector2.UnitX.RotatedByRandom(0.2).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int num44 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 234, 0f, 0f, 100, default, 1f);
                Main.dust[num44].velocity *= 0.1f;
                Main.dust[num44].position = Projectile.Center + value9 * (float)Projectile.width / 2f + Projectile.velocity * 2f;
                Main.dust[num44].fadeIn = 0.9f;
            }
            if (Main.rand.NextBool(64))
            {
                Vector2 value10 = -Vector2.UnitX.RotatedByRandom(0.4).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int num46 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 234, 0f, 0f, 155, default, 0.8f);
                Main.dust[num46].velocity *= 0.3f;
                Main.dust[num46].position = Projectile.Center + value10 * (float)Projectile.width / 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num46].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool(4))
            {
                for (int num47 = 0; num47 < 2; num47++)
                {
                    Vector2 value11 = -Vector2.UnitX.RotatedByRandom(0.8).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int num48 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num33, 0f, 0f, 0, default, 1.2f);
                    Main.dust[num48].velocity *= 0.3f;
                    Main.dust[num48].noGravity = true;
                    Main.dust[num48].position = Projectile.Center + value11 * (float)Projectile.width / 2f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num48].fadeIn = 1.4f;
                    }
                }
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 value13 = -Vector2.UnitX.RotatedByRandom(0.2).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int num50 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num34, 0f, 0f, 100, default, 1f);
                Main.dust[num50].velocity *= 0.3f;
                Main.dust[num50].position = Projectile.Center + value13 * (float)Projectile.width / 2f;
                Main.dust[num50].fadeIn = 1.2f;
                Main.dust[num50].scale = 1.5f;
                Main.dust[num50].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.25f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.25f / 255f);
            for (int num151 = 0; num151 < 2; num151++)
            {
                int num154 = 14;
                int num155 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, 263, 0f, 0f, 100, default, 1.35f);
                Main.dust[num155].noGravity = true;
                Main.dust[num155].velocity *= 0.1f;
                Main.dust[num155].velocity += Projectile.velocity * 0.5f;
            }
            if (Main.rand.NextBool(8))
            {
                int num156 = 16;
                int num157 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num156 * 2, Projectile.height - num156 * 2, 263, 0f, 0f, 100, default, 1f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].noGravity = true;
                Main.dust[num157].velocity += Projectile.velocity * 0.5f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[Projectile.type];
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 480);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 480);

            if (!target.canGhostHeal || Main.player[Projectile.owner].moonLeech)
                return;

            Player player = Main.player[Projectile.owner];
            player.statLife += 1;
            player.statMana += 25;
            player.HealEffect(1);
            player.ManaEffect(25);
        }

        public override void Kill(int timeLeft)
        {
            int num47 = Utils.SelectRandom(Main.rand, new int[]
            {
                56,
                92,
                229,
                206,
                181
            });
            int num48 = 263;
            int num49 = 263;
            int height = 50;
            float num50 = 1.7f;
            float num51 = 0.8f;
            float num52 = 2f;
            Vector2 value3 = (Projectile.rotation - 1.57079637f).ToRotationVector2();
            Vector2 value4 = value3 * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            for (int num53 = 0; num53 < 40; num53++)
            {
                num47 = Utils.SelectRandom(Main.rand, new int[]
                {
                    56,
                    92,
                    229,
                    206,
                    181
                });
                int num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num47, 0f, 0f, 200, default, num50);
                Dust dust = Main.dust[num54];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
                dust.velocity += value4 * Main.rand.NextFloat();
                num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num48, 0f, 0f, 100, default, num51);
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += value4 * Main.rand.NextFloat();
            }
            for (int num55 = 0; num55 < 20; num55++)
            {
                int num56 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num49, 0f, 0f, 0, default, num52);
                Dust dust = Main.dust[num56];
                dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
            }
        }
    }
}
