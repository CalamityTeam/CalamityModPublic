using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crags
{
    public class SoulSlurper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Slurper");
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.npcSlots = 1f;
            npc.damage = 30;
            npc.width = 60;
            npc.height = 40;
            npc.defense = 30;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 60;
            npc.knockBackResist = 0.65f;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.noGravity = true;
            npc.lavaImmune = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            if (CalamityWorld.downedProvidence)
            {
                npc.damage = 60;
                npc.defense = 45;
                npc.lifeMax = 2000;
            }
            banner = npc.type;
            bannerItem = ModContent.ItemType<SoulSlurperBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public override void AI()
        {
            bool provy = CalamityWorld.downedProvidence;
            Player target = Main.player[npc.target];
            if (npc.target < 0 || npc.target == 255 || target.dead)
            {
                npc.TargetClosest(true);
            }
            float num = 5f;
            float num2 = 0.07f;
            Vector2 source = npc.Center;
            float num4 = target.Center.X;
            float num5 = target.Center.Y;
            num4 = (float)((int)(num4 / 8f) * 8);
            num5 = (float)((int)(num5 / 8f) * 8);
            source.X = (float)((int)(source.X / 8f) * 8);
            source.Y = (float)((int)(source.Y / 8f) * 8);
            num4 -= source.X;
            num5 -= source.Y;
            float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
            float num7 = num6;
            bool flag = false;
            if (num6 > 600f)
            {
                flag = true;
            }
            if (num6 == 0f)
            {
                num4 = npc.velocity.X;
                num5 = npc.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            if (num7 > 100f)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] > 0f)
                {
                    npc.velocity.Y += 0.023f;
                }
                else
                {
                    npc.velocity.Y -= 0.023f;
                }
                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                {
                    npc.velocity.X += 0.023f;
                }
                else
                {
                    npc.velocity.X -= 0.023f;
                }
                if (npc.ai[0] > 200f)
                {
                    npc.ai[0] = -200f;
                }
            }
            if (target.dead)
            {
                num4 = (float)npc.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (npc.velocity.X < num4)
            {
                npc.velocity.X += num2;
            }
            else if (npc.velocity.X > num4)
            {
                npc.velocity.X -= num2;
            }
            if (npc.velocity.Y < num5)
            {
                npc.velocity.Y += num2;
            }
            else if (npc.velocity.Y > num5)
            {
                npc.velocity.Y -= num2;
            }
            npc.localAI[0] += 1f;
            if (npc.justHit)
            {
                npc.localAI[0] = 0f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 120f)
            {
                npc.localAI[0] = 0f;
                if (Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                {
                    int dmg = 30;
                    if (Main.expertMode)
                    {
                        dmg = 22;
                    }
                    int projType = ModContent.ProjectileType<BrimstoneBarrage>();
                    Projectile.NewProjectile(source.X, source.Y, num4, num5, projType, dmg + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
                }
            }
            int num10 = (int)npc.Center.X;
            int num11 = (int)npc.Center.Y;
            num10 /= 16;
            num11 /= 16;
            if (!WorldGen.SolidTile(num10, num11))
            {
                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.75f, 0f, 0f);
            }
            if (num4 > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2((double)num5, (double)num4);
            }
            if (num4 < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2((double)num5, (double)num4) + MathHelper.Pi;
            }
            float num12 = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -num12;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -num12;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
                {
                    npc.velocity.Y = 2f;
                }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
                {
                    npc.velocity.Y = -2f;
                }
            }
            if (flag)
            {
                if ((npc.velocity.X > 0f && num4 > 0f) || (npc.velocity.X < 0f && num4 < 0f))
                {
                    if (Math.Abs(npc.velocity.X) < 12f)
                    {
                        npc.velocity.X *= 1.05f;
                    }
                }
                else
                {
                    npc.velocity.X *= 0.9f;
                }
            }
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2));
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = npc.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2((float)texture.Width, (float)(texture.Height)) * npc.scale / 2f;
                    vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture.Width, (float)(texture.Height)) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            texture = ModContent.GetTexture("CalamityMod/NPCs/Crags/SoulSlurperGlow");
            Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (float)(num153 - num163) / 15f;
                    Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2((float)texture.Width, (float)(texture.Height)) * npc.scale / 2f;
                    vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                }
            }

            spriteBatch.Draw(texture, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemChance(npc, ModContent.ItemType<SlurperPole>(), (Main.hardMode ? 30 : 10));
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SoulSlurperGores/SoulSlurper"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SoulSlurperGores/SoulSlurper2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SoulSlurperGores/SoulSlurper3"), npc.scale);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 20; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
