using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class MagnusBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            float limit = 5f;
            float scaleFactor = 6f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (Projectile.ai[1] == 1f && Projectile.owner == Main.myPlayer)
            {
                int targetIdx = -1;
                float npcRange = 150f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        Vector2 npcPos = Main.npc[i].Center;
                        float npcDist = Vector2.Distance(npcPos, Projectile.Center);
                        if (npcDist < npcRange && targetIdx == -1 && Collision.CanHitLine(Projectile.Center, 1, 1, npcPos, 1, 1))
                        {
                            npcRange = npcDist;
                            targetIdx = i;
                        }
                    }
                }
                if (npcRange < 8f)
                {
                    Projectile.Kill();
                    return;
                }
                if (targetIdx != -1)
                {
                    Projectile.ai[1] = limit + 1f;
                    Projectile.ai[0] = (float)targetIdx;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.ai[1] > limit)
            {
                Projectile.ai[1] += 1f;
                int idx = (int)Projectile.ai[0];
                if (!Main.npc[idx].active || !Main.npc[idx].CanBeChasedBy(Projectile, false))
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.velocity.ToRotation();
                    Vector2 toNPC = Main.npc[idx].Center - Projectile.Center;
                    if (toNPC.Length() < 20f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (toNPC != Vector2.Zero)
                    {
                        toNPC.Normalize();
                        toNPC *= scaleFactor;
                    }
                    float homingSpeed = 30f;
                    Projectile.velocity = (Projectile.velocity * (homingSpeed - 1f) + toNPC) / homingSpeed;
                }
            }
            if (Projectile.ai[1] >= 1f && Projectile.ai[1] < limit)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] == limit)
                {
                    Projectile.ai[1] = 1f;
                }
            }

            int dustTypeRand = Utils.SelectRandom(Main.rand, new int[]
            {
                56,
                92,
                229,
                206,
                181
            });
            int dustType = 261;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 48f)
            {
                Projectile.localAI[0] = 0f;
            }
            else if (Projectile.alpha == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = Vector2.UnitX * -30f;
                    offset = -Vector2.UnitY.RotatedBy((double)(Projectile.localAI[0] * 0.1308997f + i * MathHelper.Pi), default) * new Vector2(10f, 20f) - Projectile.rotation.ToRotationVector2() * 10f;
                    int idx = Dust.NewDust(Projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                    Main.dust[idx].scale = 1f;
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = Projectile.Center + offset + Projectile.velocity * 2f;
                    Main.dust[idx].velocity = Vector2.Normalize(Projectile.Center + Projectile.velocity * 2f * 8f - Main.dust[idx].position) * 2f + Projectile.velocity * 2f;
                }
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 offset = -Vector2.UnitX.RotatedByRandom(0.2).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 234, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 0.1f;
                Main.dust[idx].position = Projectile.Center + offset * (float)Projectile.width / 2f + Projectile.velocity * 2f;
                Main.dust[idx].fadeIn = 0.9f;
            }
            if (Main.rand.NextBool(64))
            {
                Vector2 offset = -Vector2.UnitX.RotatedByRandom(0.4).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 234, 0f, 0f, 155, default, 0.8f);
                Main.dust[idx].velocity *= 0.3f;
                Main.dust[idx].position = Projectile.Center + offset * (float)Projectile.width / 2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool(4))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = -Vector2.UnitX.RotatedByRandom(0.8).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustTypeRand, 0f, 0f, 0, default, 1.2f);
                    Main.dust[idx].velocity *= 0.3f;
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = Projectile.Center + offset * (float)Projectile.width / 2f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[idx].fadeIn = 1.4f;
                    }
                }
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 offset = -Vector2.UnitX.RotatedByRandom(0.2).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 0.3f;
                Main.dust[idx].position = Projectile.Center + offset * (float)Projectile.width / 2f;
                Main.dust[idx].fadeIn = 1.2f;
                Main.dust[idx].scale = 1.5f;
                Main.dust[idx].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.25f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.25f / 255f);
            for (int i = 0; i < 2; i++)
            {
                int sizeFactor = 14;
                int idx = Dust.NewDust(Projectile.position, Projectile.width - sizeFactor * 2, Projectile.height - sizeFactor * 2, 263, 0f, 0f, 100, default, 1.35f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.1f;
                Main.dust[idx].velocity += Projectile.velocity * 0.5f;
            }
            if (Main.rand.NextBool(8))
            {
                int sizeFactor = 16;
                int idx = Dust.NewDust(Projectile.position, Projectile.width - sizeFactor * 2, Projectile.height - sizeFactor * 2, 263, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 0.25f;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity += Projectile.velocity * 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
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

        public override void OnKill(int timeLeft)
        {
            int dustType1 = 263;
            int dustType2 = 263;
            int height = 50;
            float scale1 = 1.7f;
            float scale2 = 0.8f;
            float scale3 = 2f;
            Vector2 value3 = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            Vector2 velocity = value3 * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			Projectile.ExpandHitboxBy(height);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            for (int i = 0; i < 40; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    56,
                    92,
                    229,
                    206,
                    181
                });
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 200, default, scale1);
                Dust dust = Main.dust[idx];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
                dust.velocity += velocity * Main.rand.NextFloat();
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType1, 0f, 0f, 100, default, scale2);
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += velocity * Main.rand.NextFloat();
            }
            for (int i = 0; i < 20; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType2, 0f, 0f, 0, default, scale3);
                Dust dust = Main.dust[idx];
                dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.velocity += velocity * (0.6f + 0.6f * Main.rand.NextFloat());
            }
        }
    }
}
