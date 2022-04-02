using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class RavagerClawLeft : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.GetNPCDamage();
            npc.width = 80;
            npc.height = 40;
            npc.defense = 40;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 12788;
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.alpha = 255;
            npc.netAlways = true;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
            if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
            {
                npc.damage = (int)(npc.damage * 1.5);
                npc.defense *= 2;
                npc.lifeMax *= 4;
            }
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 26000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            if (npc.timeLeft < 1800)
            {
                npc.timeLeft = 1800;
            }
            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
                npc.ai[1] = -90f;
            }
            if (npc.ai[0] == 0f)
            {
                npc.noTileCollide = true;
                float num659 = 36f;
                Vector2 vector79 = new Vector2(npc.Center.X, npc.Center.Y);
                float num660 = Main.npc[CalamityGlobalNPC.scavenger].Center.X - vector79.X;
                float num661 = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - vector79.Y;
                num660 -= 120f;
                num661 += 50f;
                float num662 = (float)Math.Sqrt(num660 * num660 + num661 * num661);
                if (num662 < 12f + num659)
                {
                    npc.rotation = 0f;
                    npc.Center = Main.npc[CalamityGlobalNPC.scavenger].Center + new Vector2(-120f, 50f);
                    npc.ai[1] += 1f;
                    if (npc.life < npc.lifeMax / 2 || death)
                    {
                        npc.ai[1] += 1f;
                    }
                    if (npc.life < npc.lifeMax / 3 || death)
                    {
                        npc.ai[1] += 1f;
                    }
                    if (npc.life < npc.lifeMax / 5 || death)
                    {
                        npc.ai[1] += 5f;
                    }
                    if (npc.ai[1] >= 60f)
                    {
                        // Get a target
                        if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                            npc.TargetClosest();

                        // Despawn safety, make sure to target another player if the current player target is too far away
                        if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                            npc.TargetClosest();

                        if (npc.Center.X + 100f > Main.player[npc.target].Center.X)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[0] = 1f;
                            return;
                        }
                        npc.ai[1] = 0f;
                        return;
                    }
                }
                else
                {
                    num662 = num659 / num662;
                    npc.velocity.X = num660 * num662;
                    npc.velocity.Y = num661 * num662;
                    npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.noTileCollide = true;
                npc.collideX = false;
                npc.collideY = false;
                float num663 = 12f;
                if (npc.life < npc.lifeMax / 2 || death)
                {
                    num663 += 2f;
                }
                if (npc.life < npc.lifeMax / 3 || death)
                {
                    num663 += 2f;
                }
                if (npc.life < npc.lifeMax / 5 || death)
                {
                    num663 += 5f;
                }
                Vector2 vector80 = new Vector2(npc.Center.X, npc.Center.Y);
                float num664 = Main.player[npc.target].Center.X - vector80.X;
                float num665 = Main.player[npc.target].Center.Y - vector80.Y;
                float num666 = (float)Math.Sqrt(num664 * num664 + num665 * num665);
                num666 = num663 / num666;
                npc.velocity.X = num664 * num666;
                npc.velocity.Y = num665 * num666;
                npc.ai[0] = 2f;
                npc.rotation = (float)Math.Atan2(-npc.velocity.Y, -npc.velocity.X);
            }
            else if (npc.ai[0] == 2f)
            {
                if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
                {
                    if (npc.velocity.X > 0f && npc.Center.X > Main.player[npc.target].Center.X)
                    {
                        npc.noTileCollide = false;
                    }
                    if (npc.velocity.X < 0f && npc.Center.X < Main.player[npc.target].Center.X)
                    {
                        npc.noTileCollide = false;
                    }
                }
                else
                {
                    if (npc.velocity.Y > 0f && npc.Center.Y > Main.player[npc.target].Center.Y)
                    {
                        npc.noTileCollide = false;
                    }
                    if (npc.velocity.Y < 0f && npc.Center.Y < Main.player[npc.target].Center.Y)
                    {
                        npc.noTileCollide = false;
                    }
                }
                Vector2 vector81 = new Vector2(npc.Center.X, npc.Center.Y);
                float num667 = Main.npc[CalamityGlobalNPC.scavenger].Center.X - vector81.X;
                float num668 = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - vector81.Y;
                num667 += Main.npc[CalamityGlobalNPC.scavenger].velocity.X;
                num668 += Main.npc[CalamityGlobalNPC.scavenger].velocity.Y;
                num668 += 40f;
                num667 -= 110f;
                float num669 = (float)Math.Sqrt(num667 * num667 + num668 * num668);
                if ((num669 > (death ? 900f : 700f) || npc.collideX || npc.collideY) | npc.justHit)
                {
                    npc.noTileCollide = true;
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.noTileCollide = true;
                float num671 = 12f;
                float num672 = 0.4f;
                Vector2 vector82 = new Vector2(npc.Center.X, npc.Center.Y);
                float num673 = Main.player[npc.target].Center.X - vector82.X;
                float num674 = Main.player[npc.target].Center.Y - vector82.Y;
                float num675 = (float)Math.Sqrt(num673 * num673 + num674 * num674);
                num675 = num671 / num675;
                num673 *= num675;
                num674 *= num675;
                if (npc.velocity.X < num673)
                {
                    npc.velocity.X += num672;
                    if (npc.velocity.X < 0f && num673 > 0f)
                    {
                        npc.velocity.X += num672 * 2f;
                    }
                }
                else if (npc.velocity.X > num673)
                {
                    npc.velocity.X -= num672;
                    if (npc.velocity.X > 0f && num673 < 0f)
                    {
                        npc.velocity.X -= num672 * 2f;
                    }
                }
                if (npc.velocity.Y < num674)
                {
                    npc.velocity.Y += num672;
                    if (npc.velocity.Y < 0f && num674 > 0f)
                    {
                        npc.velocity.Y += num672 * 2f;
                    }
                }
                else if (npc.velocity.Y > num674)
                {
                    npc.velocity.Y -= num672;
                    if (npc.velocity.Y > 0f && num674 < 0f)
                    {
                        npc.velocity.Y -= num672 * 2f;
                    }
                }
                npc.rotation = (float)Math.Atan2(-npc.velocity.Y, -npc.velocity.X);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            float drawPositionX = Main.npc[CalamityGlobalNPC.scavenger].Center.X - center.X;
            float drawPositionY = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - center.Y;
            drawPositionY += 12f;
            drawPositionX -= 28f;
            float rotation = (float)Math.Atan2(drawPositionY, drawPositionX) - MathHelper.PiOver2;
            bool draw = true;
            while (draw)
            {
                float totalDrawDistance = (float)Math.Sqrt(drawPositionX * drawPositionX + drawPositionY * drawPositionY);
                if (totalDrawDistance < 16f)
                {
                    draw = false;
                }
                else
                {
                    totalDrawDistance = 16f / totalDrawDistance;
                    drawPositionX *= totalDrawDistance;
                    drawPositionY *= totalDrawDistance;
                    center.X += drawPositionX;
                    center.Y += drawPositionY;
                    drawPositionX = Main.npc[CalamityGlobalNPC.scavenger].Center.X - center.X;
                    drawPositionY = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - center.Y;
                    drawPositionY += 12f;
                    drawPositionX -= 28f;
                    Color color = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
                    spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerChain"), new Vector2(center.X - Main.screenPosition.X, center.Y - Main.screenPosition.Y),
                        new Rectangle?(new Rectangle(0, 0, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerChain").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerChain").Height)), color, rotation,
                        new Vector2(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerChain").Width * 0.5f, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerChain").Height * 0.5f), 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override bool PreNPCLoot() => false;

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                int num285 = 0;
                while (num285 < damage / npc.lifeMax * 100.0)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                    num285++;
                }
            }
            else
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerClawLeft"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerClawLeft2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerClawLeft3"), 1f);
            }
        }
    }
}
