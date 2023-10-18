using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class TimeBoltKnife : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TimeBolt";

        private int maxPenetrate = 6;
        private int penetrationAmt = 6;
        private bool initialized = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = penetrationAmt;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(penetrationAmt);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            penetrationAmt = reader.ReadInt32();
        }

        public override void AI()
        {
            if (!initialized)
            {
                if (Projectile.Calamity().stealthStrike)
                {
                    maxPenetrate = 11;
                    penetrationAmt = maxPenetrate;
                }
                initialized = true;
            }

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.03f;

            // If projectile hasn't hit anything yet
            if (Projectile.ai[0] == 0f)
            {
                Projectile.tileCollide = true;
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] > 7f)
                {
                    int dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        226,
                        229
                    });
                    Vector2 center = Projectile.Center;
                    Vector2 fourVector = new Vector2(-4f, 4f);
                    fourVector += new Vector2(-4f, 4f);
                    fourVector = fourVector.RotatedBy((double)Projectile.rotation, default);
                    int dust = Dust.NewDust(center + fourVector + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 1f);
                    Dust dust2 = Main.dust[dust];
                    dust2.velocity *= 0.1f;
                    if (Main.rand.Next(6) != 0)
                        dust2.noGravity = true;
                }
                float scalar = 0.01f;
                int alphaAmt = 5;
                int alphaCeiling = alphaAmt * 15;
                int alphaFloor = 0;
                if (Projectile.localAI[0] > 7f)
                {
                    if (Projectile.localAI[1] == 0f)
                    {
                        Projectile.scale -= scalar;

                        Projectile.alpha += alphaAmt;
                        if (Projectile.alpha > alphaCeiling)
                        {
                            Projectile.alpha = alphaCeiling;
                            Projectile.localAI[1] = 1f;
                        }
                    }
                    else if (Projectile.localAI[1] == 1f)
                    {
                        Projectile.scale += scalar;

                        Projectile.alpha -= alphaAmt;
                        if (Projectile.alpha <= alphaFloor)
                        {
                            Projectile.alpha = alphaFloor;
                            Projectile.localAI[1] = 0f;
                        }
                    }
                }
            }

            // If projectile has hit an enemy and has 'split'
            else if (Projectile.ai[0] >= 1f && Projectile.ai[0] < (float)(1 + penetrationAmt))
            {
                Projectile.tileCollide = false;
                Projectile.alpha += 15;
                Projectile.velocity *= 0.98f;
                Projectile.localAI[0] = 0f;

                if (Projectile.alpha >= 255)
                {
                    if (Projectile.ai[0] == 1f)
                    {
                        Projectile.Kill();
                        return;
                    }

                    int whoAmI = -1;
                    Vector2 targetSpot = Projectile.Center;
                    float detectRange = 1000f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                            if (targetDist < detectRange)
                            {
                                detectRange = targetDist;
                                targetSpot = npc.Center;
                                whoAmI = i;
                            }
                        }
                    }

                    if (whoAmI >= 0)
                    {
                        Projectile.netUpdate = true;
                        Projectile.ai[0] += (float)penetrationAmt;
                        Projectile.position = targetSpot + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * 100f - new Vector2((float)Projectile.width, (float)Projectile.height) / 2f;
                        Projectile.velocity = Vector2.Normalize(targetSpot - Projectile.Center) * 18f;
                    }
                    else
                        Projectile.Kill();
                }

                if (Main.rand.NextBool(3))
                {
                    int dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        226,
                        229
                    });
                    Vector2 center = Projectile.Center;
                    Vector2 otherFourVector = new Vector2(-4f, 4f);
                    otherFourVector += new Vector2(-4f, 4f);
                    otherFourVector = otherFourVector.RotatedBy((double)Projectile.rotation, default);
                    int dust = Dust.NewDust(center + otherFourVector + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 0.6f);
                    Dust dust2 = Main.dust[dust];
                    dust2.velocity *= 0.1f;
                    dust2.noGravity = true;
                }
            }

            // If 'split' projectile has a target
            else if (Projectile.ai[0] >= (float)(1 + penetrationAmt) && Projectile.ai[0] < (float)(1 + penetrationAmt * 2))
            {
                Projectile.scale = 0.9f;
                Projectile.tileCollide = false;

                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 15f)
                {
                    Projectile.alpha += 51;
                    Projectile.velocity *= 0.8f;

                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                }
                else
                {
                    Projectile.alpha -= 125;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;

                    Projectile.velocity *= 0.98f;
                }

                Projectile.localAI[0] += 1f;

                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    226,
                    229
                });
                Vector2 center = Projectile.Center;
                Vector2 thirdFourVector = new Vector2(-4f, 4f);
                thirdFourVector += new Vector2(-4f, 4f);
                thirdFourVector = thirdFourVector.RotatedBy((double)Projectile.rotation, default);
                int dust = Dust.NewDust(center + thirdFourVector + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 0.6f);
                Dust dust2 = Main.dust[dust];
                dust2.velocity *= 0.1f;
                dust2.noGravity = true;
            }

            float colorScale = (float)Projectile.alpha / 255f;
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0.3f * colorScale, 0.4f * colorScale, 1f * colorScale);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200) * ((255f - (float)Projectile.alpha) / 255f);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = 0f;
            Projectile.netUpdate = true;
            Projectile.velocity = oldVelocity / 2f;

            if (penetrationAmt == maxPenetrate)
                SlowTime();

            penetrationAmt = 2;

            return false;
        }

        public override bool? CanDamage()
        {
            // Do not do damage if a tile is hit OR if projectile has 'split' and hasn't been live for more than 5 frames
            if ((((int)(Projectile.ai[0] - 1f) / penetrationAmt == 0 && penetrationAmt < 3) || Projectile.ai[1] < 5f) && Projectile.ai[0] != 0f)
                return false;
            return null;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (penetrationAmt == maxPenetrate)
                SlowTime();

            // If 'split' projectile hits an enemy
            if (Projectile.ai[0] >= (float)(1 + penetrationAmt) && Projectile.ai[0] < (float)(1 + penetrationAmt * 2))
                Projectile.ai[0] = 0f;

            // Becomes 5 on first hit, then 4, and so on
            penetrationAmt--;

            // Hits enemy, ai[0] = 0f + 4f = 4f on first hit
            // ai[0] = 4f - 1f = 3f on second hit
            // ai[0] = 3f - 1f = 2f on third hit
            if (Projectile.ai[0] == 0f)
                Projectile.ai[0] += (float)penetrationAmt;
            else
                Projectile.ai[0] -= (float)(penetrationAmt + 1);

            Projectile.ai[1] = 0f;
            Projectile.netUpdate = true;
        }

        private void SlowTime()
        {
            SoundEngine.PlaySound(SoundID.Item114, Projectile.Center);

            float radius = Projectile.Calamity().stealthStrike ? 500f : 300f;
            int numDust = (int)(0.2f * MathHelper.TwoPi * radius);
            float angleIncrement = MathHelper.TwoPi / (float)numDust;
            Vector2 dustOffset = new Vector2(radius, 0f);
            dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
            for (int i = 0; i < numDust; i++)
            {
                dustOffset = dustOffset.RotatedBy(angleIncrement);
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    226,
                    229
                });
                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType);
                Main.dust[dust].position = Projectile.Center + dustOffset;
                if (Main.rand.Next(6) != 0)
                    Main.dust[dust].noGravity = true;
                Main.dust[dust].fadeIn = 1f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].scale = 0.3f;
            }

            int buffType = ModContent.BuffType<TimeDistortion>();

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.dontTakeDamage && !npc.buffImmune[buffType] && Vector2.Distance(Projectile.Center, npc.Center) <= radius)
                {
                    if (npc.FindBuffIndex(buffType) == -1)
                        npc.AddBuff(buffType, 60, false);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.Calamity().stealthStrike)
                target.AddBuff(ModContent.BuffType<TimeDistortion>(), 120);
        }
    }
}
