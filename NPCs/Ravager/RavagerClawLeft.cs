using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class RavagerClawLeft : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.RavagerBody.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
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
            NPC.lifeMax = 12500;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.alpha = 255;
            NPC.netAlways = true;
            NPC.HitSound = RavagerBody.HitSound;
            NPC.DeathSound = RavagerBody.LimbLossSound;
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
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

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
                Vector2 npcCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                float ravBodyXDist = Main.npc[CalamityGlobalNPC.scavenger].Center.X - npcCenter.X;
                float ravBodyYDist = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - npcCenter.Y;
                ravBodyXDist -= 120f;
                ravBodyYDist += 50f;
                float ravBodyDistance = (float)Math.Sqrt(ravBodyXDist * ravBodyXDist + ravBodyYDist * ravBodyYDist);
                if (ravBodyDistance < 48f)
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
                    ravBodyDistance = 36f / ravBodyDistance;
                    NPC.velocity.X = ravBodyXDist * ravBodyDistance;
                    NPC.velocity.Y = ravBodyYDist * ravBodyDistance;
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                SoundEngine.PlaySound(RavagerBody.FistSound, NPC.Center);
                NPC.noTileCollide = true;
                NPC.collideX = false;
                NPC.collideY = false;
                float clawSpeed = 12f;
                if (NPC.life < NPC.lifeMax / 2 || death)
                {
                    clawSpeed += 2f;
                }
                if (NPC.life < NPC.lifeMax / 3 || death)
                {
                    clawSpeed += 2f;
                }
                if (NPC.life < NPC.lifeMax / 5 || death)
                {
                    clawSpeed += 5f;
                }
                Vector2 npcCenterAttack = new Vector2(NPC.Center.X, NPC.Center.Y);
                float targetX = Main.player[NPC.target].Center.X - npcCenterAttack.X;
                float targetY = Main.player[NPC.target].Center.Y - npcCenterAttack.Y;
                float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                targetDistance = clawSpeed / targetDistance;
                NPC.velocity.X = targetX * targetDistance;
                NPC.velocity.Y = targetY * targetDistance;
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
                Vector2 npcCenterRetract = new Vector2(NPC.Center.X, NPC.Center.Y);
                float bodyReturnXDist = Main.npc[CalamityGlobalNPC.scavenger].Center.X - npcCenterRetract.X;
                float bodyReturnYDist = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - npcCenterRetract.Y;
                bodyReturnXDist += Main.npc[CalamityGlobalNPC.scavenger].velocity.X;
                bodyReturnYDist += Main.npc[CalamityGlobalNPC.scavenger].velocity.Y;
                bodyReturnYDist += 40f;
                bodyReturnXDist -= 110f;
                float bodyReturnDistance = (float)Math.Sqrt(bodyReturnXDist * bodyReturnXDist + bodyReturnYDist * bodyReturnYDist);
                if ((bodyReturnDistance > (death ? 900f : 700f) || NPC.collideX || NPC.collideY) | NPC.justHit)
                {
                    NPC.noTileCollide = true;
                    NPC.ai[0] = 0f;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.noTileCollide = true;
                float velocityMult = 0.4f;
                Vector2 clawCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                float playerX = Main.player[NPC.target].Center.X - clawCenter.X;
                float playerY = Main.player[NPC.target].Center.Y - clawCenter.Y;
                float playerDist = (float)Math.Sqrt(playerX * playerX + playerY * playerY);
                playerDist = 12f / playerDist;
                playerX *= playerDist;
                playerY *= playerDist;
                if (NPC.velocity.X < playerX)
                {
                    NPC.velocity.X += velocityMult;
                    if (NPC.velocity.X < 0f && playerX > 0f)
                    {
                        NPC.velocity.X += velocityMult * 2f;
                    }
                }
                else if (NPC.velocity.X > playerX)
                {
                    NPC.velocity.X -= velocityMult;
                    if (NPC.velocity.X > 0f && playerX < 0f)
                    {
                        NPC.velocity.X -= velocityMult * 2f;
                    }
                }
                if (NPC.velocity.Y < playerY)
                {
                    NPC.velocity.Y += velocityMult;
                    if (NPC.velocity.Y < 0f && playerY > 0f)
                    {
                        NPC.velocity.Y += velocityMult * 2f;
                    }
                }
                else if (NPC.velocity.Y > playerY)
                {
                    NPC.velocity.Y -= velocityMult;
                    if (NPC.velocity.Y > 0f && playerY < 0f)
                    {
                        NPC.velocity.Y -= velocityMult * 2f;
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

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                int dustCounter = 0;
                while (dustCounter < hit.Damage / NPC.lifeMax * 100.0)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                    dustCounter++;
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
