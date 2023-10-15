using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TenebreusTidesWaterSword : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private int penetrationAmt = 2;
        private bool dontDraw = false;
        private int drawInt = 0;

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = penetrationAmt;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5 * Projectile.MaxUpdates;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
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
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.ToRadians(45f);

            // If projectile hasn't hit anything yet
            if (Projectile.ai[0] == 0f)
            {
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] > 7f)
                {
                    int water = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 0.4f);
                    Main.dust[water].noGravity = true;
                    Main.dust[water].velocity *= 0.5f;
                    Main.dust[water].velocity += Projectile.velocity * 0.1f;
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
                    float detectRange = 700f;
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
                        Projectile.position = targetSpot + ((float)Main.rand.NextDouble() * 6.28318548f).ToRotationVector2() * 100f - new Vector2((float)Projectile.width, (float)Projectile.height) / 2f;
                        dontDraw = true;
                        Projectile.velocity = Vector2.Normalize(targetSpot - Projectile.Center) * 18f;
                    }
                    else
                        Projectile.Kill();
                }

                if (Main.rand.NextBool(3))
                {
                    int water = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 0.4f);
                    Main.dust[water].noGravity = true;
                    Main.dust[water].velocity *= 0.5f;
                    Main.dust[water].velocity += Projectile.velocity * 0.1f;
                }
            }

            // If 'split' projectile has a target
            else if (Projectile.ai[0] >= (float)(1 + penetrationAmt) && Projectile.ai[0] < (float)(1 + penetrationAmt * 2))
            {
                Projectile.scale = 0.9f;

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

                int water = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 0.4f);
                Main.dust[water].noGravity = true;
                Main.dust[water].velocity *= 0.5f;
                Main.dust[water].velocity += Projectile.velocity * 0.1f;
            }

            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0f, 0f, (255 - Projectile.alpha) * 1f / 255f);
            if (dontDraw)
                drawInt++;
            if (drawInt > 1)
            {
                drawInt = 0;
                dontDraw = false;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(50, 50, 255, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            if (dontDraw)
                return false;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage()
        {
            // Do not do damage if a tile is hit OR if projectile has 'split' and hasn't been live for more than 5 frames
            if ((((int)(Projectile.ai[0] - 1f) / penetrationAmt == 0 && penetrationAmt < 3) || Projectile.ai[1] < 5f) && Projectile.ai[0] != 0f)
                return false;
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }
    }
}
