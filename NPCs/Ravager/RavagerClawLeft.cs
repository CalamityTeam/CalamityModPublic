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
            this.HideFromBestiary();
            DisplayName.SetDefault("Ravager");
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.GetNPCDamage();
            NPC.width = 80;
            NPC.height = 40;
            NPC.defense = 40;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 12788;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.alpha = 255;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath14;
            if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
            {
                NPC.damage = (int)(NPC.damage * 1.5);
                NPC.defense *= 2;
                NPC.lifeMax *= 4;
            }
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 26000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            if (NPC.timeLeft < 1800)
            {
                NPC.timeLeft = 1800;
            }
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                {
                    NPC.alpha = 0;
                }
                NPC.ai[1] = -90f;
            }
            if (NPC.ai[0] == 0f)
            {
                NPC.noTileCollide = true;
                float num659 = 36f;
                Vector2 vector79 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num660 = Main.npc[CalamityGlobalNPC.scavenger].Center.X - vector79.X;
                float num661 = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - vector79.Y;
                num660 -= 120f;
                num661 += 50f;
                float num662 = (float)Math.Sqrt(num660 * num660 + num661 * num661);
                if (num662 < 12f + num659)
                {
                    NPC.rotation = 0f;
                    NPC.Center = Main.npc[CalamityGlobalNPC.scavenger].Center + new Vector2(-120f, 50f);
                    NPC.ai[1] += 1f;
                    if (NPC.life < NPC.lifeMax / 2 || death)
                    {
                        NPC.ai[1] += 1f;
                    }
                    if (NPC.life < NPC.lifeMax / 3 || death)
                    {
                        NPC.ai[1] += 1f;
                    }
                    if (NPC.life < NPC.lifeMax / 5 || death)
                    {
                        NPC.ai[1] += 5f;
                    }
                    if (NPC.ai[1] >= 60f)
                    {
                        // Get a target
                        if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                            NPC.TargetClosest();

                        // Despawn safety, make sure to target another player if the current player target is too far away
                        if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                            NPC.TargetClosest();

                        if (NPC.Center.X + 100f > Main.player[NPC.target].Center.X)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[0] = 1f;
                            return;
                        }
                        NPC.ai[1] = 0f;
                        return;
                    }
                }
                else
                {
                    num662 = num659 / num662;
                    NPC.velocity.X = num660 * num662;
                    NPC.velocity.Y = num661 * num662;
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.noTileCollide = true;
                NPC.collideX = false;
                NPC.collideY = false;
                float num663 = 12f;
                if (NPC.life < NPC.lifeMax / 2 || death)
                {
                    num663 += 2f;
                }
                if (NPC.life < NPC.lifeMax / 3 || death)
                {
                    num663 += 2f;
                }
                if (NPC.life < NPC.lifeMax / 5 || death)
                {
                    num663 += 5f;
                }
                Vector2 vector80 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num664 = Main.player[NPC.target].Center.X - vector80.X;
                float num665 = Main.player[NPC.target].Center.Y - vector80.Y;
                float num666 = (float)Math.Sqrt(num664 * num664 + num665 * num665);
                num666 = num663 / num666;
                NPC.velocity.X = num664 * num666;
                NPC.velocity.Y = num665 * num666;
                NPC.ai[0] = 2f;
                NPC.rotation = (float)Math.Atan2(-NPC.velocity.Y, -NPC.velocity.X);
            }
            else if (NPC.ai[0] == 2f)
            {
                if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                {
                    if (NPC.velocity.X > 0f && NPC.Center.X > Main.player[NPC.target].Center.X)
                    {
                        NPC.noTileCollide = false;
                    }
                    if (NPC.velocity.X < 0f && NPC.Center.X < Main.player[NPC.target].Center.X)
                    {
                        NPC.noTileCollide = false;
                    }
                }
                else
                {
                    if (NPC.velocity.Y > 0f && NPC.Center.Y > Main.player[NPC.target].Center.Y)
                    {
                        NPC.noTileCollide = false;
                    }
                    if (NPC.velocity.Y < 0f && NPC.Center.Y < Main.player[NPC.target].Center.Y)
                    {
                        NPC.noTileCollide = false;
                    }
                }
                Vector2 vector81 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num667 = Main.npc[CalamityGlobalNPC.scavenger].Center.X - vector81.X;
                float num668 = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - vector81.Y;
                num667 += Main.npc[CalamityGlobalNPC.scavenger].velocity.X;
                num668 += Main.npc[CalamityGlobalNPC.scavenger].velocity.Y;
                num668 += 40f;
                num667 -= 110f;
                float num669 = (float)Math.Sqrt(num667 * num667 + num668 * num668);
                if ((num669 > (death ? 900f : 700f) || NPC.collideX || NPC.collideY) | NPC.justHit)
                {
                    NPC.noTileCollide = true;
                    NPC.ai[0] = 0f;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.noTileCollide = true;
                float num671 = 12f;
                float num672 = 0.4f;
                Vector2 vector82 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num673 = Main.player[NPC.target].Center.X - vector82.X;
                float num674 = Main.player[NPC.target].Center.Y - vector82.Y;
                float num675 = (float)Math.Sqrt(num673 * num673 + num674 * num674);
                num675 = num671 / num675;
                num673 *= num675;
                num674 *= num675;
                if (NPC.velocity.X < num673)
                {
                    NPC.velocity.X += num672;
                    if (NPC.velocity.X < 0f && num673 > 0f)
                    {
                        NPC.velocity.X += num672 * 2f;
                    }
                }
                else if (NPC.velocity.X > num673)
                {
                    NPC.velocity.X -= num672;
                    if (NPC.velocity.X > 0f && num673 < 0f)
                    {
                        NPC.velocity.X -= num672 * 2f;
                    }
                }
                if (NPC.velocity.Y < num674)
                {
                    NPC.velocity.Y += num672;
                    if (NPC.velocity.Y < 0f && num674 > 0f)
                    {
                        NPC.velocity.Y += num672 * 2f;
                    }
                }
                else if (NPC.velocity.Y > num674)
                {
                    NPC.velocity.Y -= num672;
                    if (NPC.velocity.Y > 0f && num674 < 0f)
                    {
                        NPC.velocity.Y -= num672 * 2f;
                    }
                }
                NPC.rotation = (float)Math.Atan2(-NPC.velocity.Y, -NPC.velocity.X);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
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
                    spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerChain").Value, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y),
                        new Rectangle?(new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerChain").Value.Width, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerChain").Value.Height)), color, rotation,
                        new Vector2(ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerChain").Value.Width * 0.5f, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerChain").Value.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (damage > 0)
                player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life > 0)
            {
                int num285 = 0;
                while (num285 < damage / NPC.lifeMax * 100.0)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                    num285++;
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScavengerClawLeft").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScavengerClawLeft2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScavengerClawLeft3").Type, 1f);
                }
            }
        }
    }
}
