using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.SupremeCalamitas
{
	[AutoloadBossHead]
	public class SupremeCalamitas : ModNPC
	{
        private float bossLife;
        private float uDieLul = 1f;
        private float passedVar = 0f;

        private bool protectionBoost = false;
        private bool canDespawn = false;
        private bool despawnProj = false;
        private bool startText = false;
        private bool wormAlive = false;
        private bool startBattle = false; //100%
        private bool startSecondAttack = false; //80%
        private bool startThirdAttack = false; //60%
        private bool halfLife = false; //40%
        private bool startFourthAttack = false; //30%
        private bool secondStage = false; //20%
        private bool startFifthAttack = false; //10%
        private bool gettingTired = false; //8%
        private bool gettingTired2 = false; //6%
        private bool gettingTired3 = false; //4%
        private bool gettingTired4 = false; //2%
        private bool gettingTired5 = false; //1%
        private bool willCharge = false;

        private int giveUpCounter = 1200;
        private int lootTimer = 0; //900 * 5 = 4500
        private int phaseChange = 0;
        private int spawnX = 0;
        private int spawnX2 = 0;
        private int spawnXReset = 0;
        private int spawnXReset2 = 0;
        private int spawnXAdd = 200;
        private int spawnY = 0;
        private int spawnYReset = 0;
        private int spawnYAdd = 0;

        private Rectangle safeBox = default(Rectangle);

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Supreme Calamitas");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 350;
			npc.npcSlots = 50f;
			npc.width = 120; //324
			npc.height = 120; //216
			npc.defense = 0;
			npc.lifeMax = CalamityWorld.revenge ? 5500000 : 5000000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 6250000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 4600000 : 4100000;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
            }
			npc.dontTakeDamage = false;
			npc.chaseable = true;
			npc.boss = true;
            npc.canGhostHeal = false;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SC1");
		}
		
		public override void AI()
		{
            #region StartUp
            CalamityGlobalNPC.SCal = npc.whoAmI;
            lootTimer++;
            if (Main.slimeRain)
            {
                Main.StopSlimeRain(true);
                if (Main.netMode == 2)
                {
                    NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                }
            }
            if (Main.raining)
            {
                Main.raining = false;
                if (Main.netMode == 2)
                {
                    NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                }
            }
            bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			Player player = Main.player[npc.target];
            if (!startText)
            {
                if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalKillCount == 4)
                {
                    string key = "Mods.CalamityMod.SupremeBossText12"; //kill SCal 4 times
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                else if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalKillCount == 1)
                {
                    string key = "Mods.CalamityMod.SupremeBossText11"; //kill SCal once
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalDeathCount < 51)
                {
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalDeathCount == 50)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText15"; //die 50 or more times
                        Color messageColor = Color.Orange;
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    else if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalDeathCount > 19)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText14"; //die 20 or more times
                        Color messageColor = Color.Orange;
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    else if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalDeathCount > 4)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText13"; //die 5 or more times
                        Color messageColor = Color.Orange;
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                }
                startText = true;
            }
            #endregion
            #region ArenaCreation
            if (npc.localAI[3] == 0f)
            {
                npc.localAI[3] = 1f;
                Vector2 vectorPlayer = new Vector2(player.position.X, player.position.Y);
                if (CalamityWorld.bossRushActive)
                {
                    safeBox.X = spawnX = spawnXReset = (int)(vectorPlayer.X - 1250f);
                    spawnX2 = spawnXReset2 = (int)(vectorPlayer.X + 1250f);
                    safeBox.Y = spawnY = spawnYReset = (int)(vectorPlayer.Y - 1250f);
                    safeBox.Width = 2500;
                    safeBox.Height = 2500;
                    spawnYAdd = 125;
                }
                else if (CalamityWorld.death)
                {
                    safeBox.X = spawnX = spawnXReset = (int)(vectorPlayer.X - 1000f);
                    spawnX2 = spawnXReset2 = (int)(vectorPlayer.X + 1000f);
                    safeBox.Y = spawnY = spawnYReset = (int)(vectorPlayer.Y - 1000f);
                    safeBox.Width = 2000;
                    safeBox.Height = 2000;
                    spawnYAdd = 100;
                }
                else
                {
                    safeBox.X = spawnX = spawnXReset = (int)(vectorPlayer.X - (revenge ? 1500f : 2000f));
                    spawnX2 = spawnXReset2 = (int)(vectorPlayer.X + (revenge ? 1500f : 2000f));
                    safeBox.Y = spawnY = spawnYReset = (int)(vectorPlayer.Y - (revenge ? 1500f : 2000f));
                    safeBox.Width = revenge ? 3000 : 4000;
                    safeBox.Height = revenge ? 3000 : 4000;
                    spawnYAdd = revenge ? 150 : 200;
                }
                if (Main.netMode != 1)
                {
                    int num52 = (int)(safeBox.X + (float)(safeBox.Width / 2)) / 16;
                    int num53 = (int)(safeBox.Y + (float)(safeBox.Height / 2)) / 16;
                    int num54 = safeBox.Width / 2 / 16 + 1;
                    for (int num55 = num52 - num54; num55 <= num52 + num54; num55++)
                    {
                        for (int num56 = num53 - num54; num56 <= num53 + num54; num56++)
                        {
                            if ((num55 == num52 - num54 || num55 == num52 + num54 || num56 == num53 - num54 || num56 == num53 + num54) && !Main.tile[num55, num56].active())
                            {
                                Main.tile[num55, num56].type = (ushort)mod.TileType("ArenaTile");
                                Main.tile[num55, num56].active(true);
                            }
                            Main.tile[num55, num56].lava(false);
                            Main.tile[num55, num56].liquid = 0;
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num55, num56, 1, TileChangeType.None);
                            }
                            else
                            {
                                WorldGen.SquareTileFrame(num55, num56, true);
                            }
                        }
                    }
                }
            }
            if (!player.Hitbox.Intersects(safeBox))
            {
                if (uDieLul < 3f)
                {
                    uDieLul *= 1.01f;
                }
                else if (uDieLul > 3f)
                {
                    uDieLul = 3f;
                }
                protectionBoost = true;
            }
            else
            {
                if (uDieLul > 1f)
                {
                    uDieLul *= 0.99f;
                }
                else if (uDieLul < 1f)
                {
                    uDieLul = 1f;
                }
                protectionBoost = false;
            }
            #endregion
            #region Despawn
            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -50f);
                    canDespawn = true;
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else
            {
                canDespawn = false;
            }
            #endregion
            #region FirstAttack
            if (npc.localAI[2] < 900f)
            {
                despawnProj = true;
                npc.localAI[2] += 1f;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != 1)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 4f : 6f))
                    {
                        npc.localAI[0] = 0f;
                        int damage = expertMode ? 200 : 250; //800 500
                        if (npc.localAI[2] < 300f) //blasts from above
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 4f * uDieLul, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (npc.localAI[2] < 600f) //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from above, left, and right
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 3f * uDieLul, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            else if (!startBattle)
            {
                string key = "Mods.CalamityMod.SupremeBossText3";
                Color messageColor = Color.Orange;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                if (Main.netMode != 1)
                {
                    spawnY += 400;
                    if (revenge)
                    {
                        spawnY -= 100;
                    }
                    if (CalamityWorld.bossRushActive)
                    {
                        spawnY -= 50;
                    }
                    else if (CalamityWorld.death)
                    {
                        spawnY -= 100;
                    }
                    for (int x = 0; x < 5; x++)
                    {
                        int heart = NPC.NewNPC(spawnX + 50, spawnY, mod.NPCType("SCalWormHeart"), 0, 0f, 0f, 0f, 0f, 255);
                        spawnX += spawnXAdd;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(23, -1, -1, null, heart, 0f, 0f, 0f, 0, 0, 0);
                        }
                        int heart2 = NPC.NewNPC(spawnX2 - 50, spawnY, mod.NPCType("SCalWormHeart"), 0, 0f, 0f, 0f, 0f, 255);
                        spawnX2 -= spawnXAdd;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(23, -1, -1, null, heart2, 0f, 0f, 0f, 0, 0, 0);
                        }
                        spawnY += spawnYAdd;
                    }
                    spawnX = spawnXReset;
                    spawnX2 = spawnXReset2;
                    spawnY = spawnYReset;
                    NPC.SpawnOnPlayer(npc.FindClosestPlayer(), mod.NPCType("SCalWormHead"));
                }
                startBattle = true;
            }
            #endregion
            #region SecondAttack
            if (npc.localAI[2] < 1800f && startSecondAttack)
            {
                despawnProj = true;
                npc.localAI[2] += 1f;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != 1)
                {
                    int damage = expertMode ? 150 : 200; //600 400
                    if (npc.localAI[2] < 1200f)
                    {
                        if (npc.localAI[2] % 180 == 0) //blasts from top
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, mod.ProjectileType("BrimstoneGigaBlast"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    else if (npc.localAI[2] < 1500f && npc.localAI[2] > 1200f)
                    {
                        if (npc.localAI[2] % 180 == 0) //blasts from right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -5f * uDieLul, 0f, mod.ProjectileType("BrimstoneGigaBlast"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    else if (npc.localAI[2] > 1500f)
                    {
                        if (npc.localAI[2] % 180 == 0) //blasts from top
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, mod.ProjectileType("BrimstoneGigaBlast"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 6f : 9f))
                    {
                        npc.localAI[0] = 0f;
                        if (npc.localAI[2] < 1200f) //blasts from below
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y + 1000f, 0f, -4f * uDieLul, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (npc.localAI[2] < 1500f) //blasts from left
                        {
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            if (!startSecondAttack && ((double)npc.life <= (double)npc.lifeMax * 0.75))
            {
                string key = "Mods.CalamityMod.SupremeBossText4";
                Color messageColor = Color.Orange;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                startSecondAttack = true;
                return;
            }
            #endregion
            #region ThirdAttack
            if (npc.localAI[2] < 2700f && startThirdAttack)
            {
                despawnProj = true;
                npc.localAI[2] += 1f;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != 1)
                {
                    int damage = expertMode ? 150 : 200;
                    if (npc.localAI[2] % 180 == 0) //blasts from top
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, mod.ProjectileType("BrimstoneGigaBlast"), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (npc.localAI[2] % 240 == 0) //fireblasts from above
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 10f * uDieLul, mod.ProjectileType("BrimstoneFireblast"), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 9f : 11f))
                    {
                        npc.localAI[0] = 0f;
                        if (npc.localAI[2] < 2100f) //blasts from above
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 4f * uDieLul, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (npc.localAI[2] < 2400f) //blasts from right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            if (!startThirdAttack && ((double)npc.life <= (double)npc.lifeMax * 0.5))
            {
                music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SC2");
                string key = "Mods.CalamityMod.SupremeBossText5";
                Color messageColor = Color.Orange;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                startThirdAttack = true;
                return;
            }
            #endregion
            #region FourthAttack
            if (npc.localAI[2] < 3600f && startFourthAttack)
            {
                despawnProj = true;
                npc.localAI[2] += 1f;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != 1) //more clustered attack
                {
                    int damage = expertMode ? 150 : 200;
                    if (npc.localAI[2] % 180 == 0) //blasts from top
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, mod.ProjectileType("BrimstoneGigaBlast"), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (npc.localAI[2] % 240 == 0) //fireblasts from above
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 10f * uDieLul, mod.ProjectileType("BrimstoneFireblast"), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (npc.localAI[2] % 450 == 0) //giant homing fireballs
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 1f * uDieLul, mod.ProjectileType("BrimstoneMonster"), damage, 0f, Main.myPlayer, 0f, passedVar);
                        passedVar += 1f;
                    }
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 12f : 15f))
                    {
                        npc.localAI[0] = 0f;
                        if (npc.localAI[2] < 3000f) //blasts from below
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y + 1000f, 0f, -4f * uDieLul, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (npc.localAI[2] < 3300f) //blasts from left
                        {
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            if (!startFourthAttack && ((double)npc.life <= (double)npc.lifeMax * 0.3))
            {
                music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SC3");
                string key = "Mods.CalamityMod.SupremeBossText7";
                Color messageColor = Color.Orange;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                startFourthAttack = true;
                return;
            }
            #endregion
            #region FifthAttack
            if (npc.localAI[2] < 4500f && startFifthAttack)
            {
                despawnProj = true;
                npc.localAI[2] += 1f;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != 1)
                {
                    int damage = expertMode ? 150 : 200;
                    if (npc.localAI[2] % 240 == 0) //blasts from top
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, mod.ProjectileType("BrimstoneGigaBlast"), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (npc.localAI[2] % 360 == 0) //fireblasts from above
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 10f * uDieLul, mod.ProjectileType("BrimstoneFireblast"), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (npc.localAI[2] % 450 == 0) //giant homing fireballs
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 1f * uDieLul, mod.ProjectileType("BrimstoneMonster"), damage, 0f, Main.myPlayer, 0f, passedVar);
                        passedVar += 1f;
                    }
                    if (npc.localAI[2] % 30 == 0) //projectiles that move in wave pattern
                    {
                        Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-500, 500), -10f * uDieLul, 0f, mod.ProjectileType("BrimstoneWave"), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 15f : 18f))
                    {
                        npc.localAI[0] = 0f;
                        if (npc.localAI[2] < 3900f) //blasts from above
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 4f * uDieLul, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (npc.localAI[2] < 4200f) //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from above, left, and right
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 3f * uDieLul, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3f * uDieLul, 0f, mod.ProjectileType("BrimstoneHellblast2"), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            if (!startFifthAttack && ((double)npc.life <= (double)npc.lifeMax * 0.1))
            {
                string key = "Mods.CalamityMod.SupremeBossText9";
                Color messageColor = Color.Orange;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                startFifthAttack = true;
                return;
            }
            #endregion
            #region EndSections
            if (startFifthAttack)
            {
                if (gettingTired5)
                {
                    music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SC4");
                    npc.noGravity = false;
                    npc.noTileCollide = false;
                    npc.damage = 0;
                    npc.velocity.X *= 0.98f;
                    npc.velocity.Y = 5f;
                    Vector2 vector2 = new Vector2(npc.Center.X, npc.Center.Y);
                    float num = player.Center.X - vector2.X;
                    float num1 = player.Center.Y - vector2.Y;
                    npc.rotation = (float)Math.Atan2((double)num1, (double)num) - 1.57f;
                    if (player.GetModPlayer<CalamityPlayer>(mod).sCalKillCount > 0) //after first time you kill her
                    {
                        if (giveUpCounter == 900)
                        {
                            string key = "Mods.CalamityMod.SupremeBossText27";
                            Color messageColor = Color.Orange;
                            if (Main.netMode == 0)
                            {
                                Main.NewText(Language.GetTextValue(key), messageColor);
                            }
                            else if (Main.netMode == 2)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                            }
                        }
                        giveUpCounter--;
                        npc.chaseable = (giveUpCounter < 900) ? true : false;
                        npc.dontTakeDamage = (giveUpCounter < 900) ? false : true;
                        return;
                    }
                    if (giveUpCounter == 600)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText25";
                        Color messageColor = Color.Orange;
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    if (giveUpCounter == 300)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText26";
                        Color messageColor = Color.Orange;
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    if (giveUpCounter <= 0)
                    {
                        npc.chaseable = true;
                        npc.dontTakeDamage = false;
                        return;
                    }
                    giveUpCounter--;
                    npc.chaseable = false;
                    npc.dontTakeDamage = true;
                    return;
                }
                if (!gettingTired5 && ((double)npc.life <= (double)npc.lifeMax * 0.01))
                {
                    string key = "Mods.CalamityMod.SupremeBossText24";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    gettingTired5 = true;
                    return;
                }
                else if (!gettingTired4 && ((double)npc.life <= (double)npc.lifeMax * 0.02))
                {
                    string key = "Mods.CalamityMod.SupremeBossText23";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    gettingTired4 = true;
                    return;
                }
                else if (!gettingTired3 && ((double)npc.life <= (double)npc.lifeMax * 0.04))
                {
                    string key = "Mods.CalamityMod.SupremeBossText22";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    gettingTired3 = true;
                    return;
                }
                else if (!gettingTired2 && ((double)npc.life <= (double)npc.lifeMax * 0.06))
                {
                    string key = "Mods.CalamityMod.SupremeBossText21";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    gettingTired2 = true;
                    return;
                }
                else if (!gettingTired && ((double)npc.life <= (double)npc.lifeMax * 0.08))
                {
                    string key = "Mods.CalamityMod.SupremeBossText20";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    if (Main.netMode != 1)
                    {
                        spawnY += 400;
                        if (revenge)
                        {
                            spawnY -= 100;
                        }
                        if (CalamityWorld.bossRushActive)
                        {
                            spawnY -= 50;
                        }
                        else if (CalamityWorld.death)
                        {
                            spawnY -= 100;
                        }
                        for (int x = 0; x < 5; x++)
                        {
                            int heart = NPC.NewNPC(spawnX + 50, spawnY, mod.NPCType("SCalWormHeart"), 0, 0f, 0f, 0f, 0f, 255);
                            spawnX += spawnXAdd;
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(23, -1, -1, null, heart, 0f, 0f, 0f, 0, 0, 0);
                            }
                            int heart2 = NPC.NewNPC(spawnX2 - 50, spawnY, mod.NPCType("SCalWormHeart"), 0, 0f, 0f, 0f, 0f, 255);
                            spawnX2 -= spawnXAdd;
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(23, -1, -1, null, heart2, 0f, 0f, 0f, 0, 0, 0);
                            }
                            spawnY += spawnYAdd;
                        }
                        spawnX = spawnXReset;
                        spawnX2 = spawnXReset2;
                        spawnY = spawnYReset;
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), mod.NPCType("SCalWormHead"));
                    }
                    gettingTired = true;
                    return;
                }
            }
            #endregion
            #region DespawnProjectiles
            if (npc.localAI[2] % 900 == 0 && despawnProj)
            {
                int proj;
                for (int x = 0; x < 1000; x = proj + 1)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && (projectile.type == mod.ProjectileType("BrimstoneHellblast2") || 
                        projectile.type == mod.ProjectileType("BrimstoneGigaBlast") || 
                        projectile.type == mod.ProjectileType("BrimstoneBarrage") || 
                        projectile.type == mod.ProjectileType("BrimstoneFireblast") ||
                        projectile.type == mod.ProjectileType("BrimstoneWave")))
                    {
                        projectile.Kill();
                    }
                    proj = x;
                }
                despawnProj = false;
            }
            #endregion
            #region TransformSeekerandBrotherTriggers
            if (!halfLife && ((double)npc.life <= (double)npc.lifeMax * 0.4))
			{
				string key = "Mods.CalamityMod.SupremeBossText";
				Color messageColor = Color.Orange;
				if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				halfLife = true;
			}
            if ((double)npc.life <= (double)npc.lifeMax * 0.2)
			{
				if (secondStage == false)
				{
                    string key = "Mods.CalamityMod.SupremeBossText8";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    if (Main.netMode != 1)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 20; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 18) * 300)), (int)(npc.Center.Y + (Math.Cos(I * 18) * 300)), mod.NPCType("SoulSeekerSupreme"), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 18;
                            Eye.ai[3] = I * 18;
                        }
                    }
					secondStage = true;
				}
			}
            if (bossLife == 0f && npc.life > 0)
			{
				bossLife = (float)npc.lifeMax;
			}
	       	if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.55);
					if ((float)(npc.life + num660) < bossLife)
					{
						bossLife = (float)npc.life;
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), mod.NPCType("SupremeCataclysm"));
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), mod.NPCType("SupremeCatastrophe"));
						string key = "Mods.CalamityMod.SupremeBossText6";
						Color messageColor = Color.Orange;
						if (Main.netMode == 0)
						{
							Main.NewText(Language.GetTextValue(key), messageColor);
						}
						else if (Main.netMode == 2)
						{
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
						}
						return;
					}
				}
	       	}
            #endregion
            #region TargetandRotation
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(true);
			}
			float num801 = npc.position.X + (float)(npc.width / 2) - player.position.X - (float)(player.width / 2);
			float num802 = npc.position.Y + (float)npc.height - 59f - player.position.Y - (float)(player.height / 2);
			float num803 = (float)Math.Atan2((double)num802, (double)num801) + 1.57f;
			if (num803 < 0f)
			{
				num803 += 6.283f;
			}
			else if ((double)num803 > 6.283)
			{
				num803 -= 6.283f;
			}
			float num804 = 0.1f;
			if (npc.rotation < num803)
			{
				if ((double)(num803 - npc.rotation) > 3.1415)
				{
					npc.rotation -= num804;
				}
				else
				{
					npc.rotation += num804;
				}
			}
			else if (npc.rotation > num803)
			{
				if ((double)(npc.rotation - num803) > 3.1415)
				{
					npc.rotation += num804;
				}
				else
				{
					npc.rotation -= num804;
				}
			}
			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
			{
				npc.rotation = num803;
			}
			if (npc.rotation < 0f)
			{
				npc.rotation += 6.283f;
			}
			else if ((double)npc.rotation > 6.283)
			{
				npc.rotation -= 6.283f;
			}
			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
			{
				npc.rotation = num803;
			}
            #endregion
            #region FirstStage
            if (npc.ai[0] == 0f)
			{
                npc.damage = expertMode ? 720 : 450;
                if (NPC.AnyNPCs(mod.NPCType("SCalWormHead")))
                {
                    wormAlive = true;
                    npc.dontTakeDamage = true;
                    npc.chaseable = false;
                }
                else
                {
                    if (NPC.AnyNPCs(mod.NPCType("SupremeCataclysm")) || NPC.AnyNPCs(mod.NPCType("SupremeCatastrophe")))
                    {
                        npc.dontTakeDamage = true;
                        npc.chaseable = false;
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                        npc.chaseable = true;
                    }
                    wormAlive = false;
                }
                if (npc.ai[1] == 0f)
				{
					float num823 = 12f;
					float num824 = 0.12f;
					Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
					float num826 = player.position.Y + (float)(player.height / 2) - 550f - vector82.Y;
					float num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
					num827 = num823 / num827;
					num825 *= num827;
					num826 *= num827;
					if (npc.velocity.X < num825)
					{
						npc.velocity.X = npc.velocity.X + num824;
						if (npc.velocity.X < 0f && num825 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num824;
						}
					}
					else if (npc.velocity.X > num825)
					{
						npc.velocity.X = npc.velocity.X - num824;
						if (npc.velocity.X > 0f && num825 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num824;
						}
					}
					if (npc.velocity.Y < num826)
					{
						npc.velocity.Y = npc.velocity.Y + num824;
						if (npc.velocity.Y < 0f && num826 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num824;
						}
					}
					else if (npc.velocity.Y > num826)
					{
						npc.velocity.Y = npc.velocity.Y - num824;
						if (npc.velocity.Y > 0f && num826 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num824;
						}
					}
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 300f)
					{
						npc.ai[1] = -1f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
					}
					vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num825 = player.position.X + (float)(player.width / 2) - vector82.X;
					num826 = player.position.Y + (float)(player.height / 2) - vector82.Y;
					npc.rotation = (float)Math.Atan2((double)num826, (double)num825) - 1.57f;
					if (Main.netMode != 1)
					{
						npc.localAI[1] += wormAlive ? 0.5f : 1f;
						if (npc.localAI[1] > 90f)
						{
							npc.localAI[1] = 0f;
							float num828 = 10f * uDieLul;
							int num829 = expertMode ? 150 : 200; //600 400
                            Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
							float num181 = Math.Abs(num180) * 0.1f;
							float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
							float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
							num183 = num828 / num183;
							num180 *= num183;
							num182 *= num183;
							value9.X += num180;
							value9.Y += num182;
							int randomShot = Main.rand.Next(6); //0 to 5
							if (randomShot == 0)
							{
								randomShot = mod.ProjectileType("BrimstoneFireblast");
								num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
								num827 = num828 / num827;
								num825 *= num827;
								num826 *= num827;
								vector82.X += num825 * 15f;
								vector82.Y += num826 * 15f;
								Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
							}
							else if (randomShot <= 4)
							{
								randomShot = mod.ProjectileType("BrimstoneBarrage");
								for (int num186 = 1; num186 <= 8; num186++)
								{
									num180 = player.position.X + (float)player.width * 0.5f - value9.X;
									num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
									num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                    float speedBoost = (float)(num186 > 4 ? -(num186 - 4) : num186);
                                    num183 = (8f + speedBoost) / num183;
									num180 *= num183;
									num182 *= num183;
									Projectile.NewProjectile(value9.X, value9.Y, num180 + speedBoost, num182 + speedBoost, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
								}
							}
							else
							{
								randomShot = mod.ProjectileType("BrimstoneGigaBlast");
								num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
								num827 = num828 / num827;
								num825 *= num827;
								num826 *= num827;
								vector82.X += num825 * 15f;
								vector82.Y += num826 * 15f;
								Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
							}
							return;
						}
					}
				}
				else if (npc.ai[1] == 1f)
				{
					npc.rotation = num803; //change
					float num383 = wormAlive ? 26f : 30f;
					if ((double)npc.life < (double)npc.lifeMax * 0.95) 
					{
						num383 += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.85) 
					{
						num383 += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.7) 
					{
						num383 += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.6) 
					{
						num383 += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.5) 
					{
						num383 += 1f;
					}
					Vector2 vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num384 = player.position.X + (float)(player.width / 2) - vector37.X;
					float num385 = player.position.Y + (float)(player.height / 2) - vector37.Y;
					float num386 = (float)Math.Sqrt((double)(num384 * num384 + num385 * num385));
					num386 = num383 / num386;
					npc.velocity.X = num384 * num386;
					npc.velocity.Y = num385 * num386;
					npc.ai[1] = 2f;
				} 
				else if (npc.ai[1] == 2f) 
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 25f) 
					{
						npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.Y = npc.velocity.Y * 0.96f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1) 
						{
							npc.velocity.X = 0f;
						}
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1) 
						{
							npc.velocity.Y = 0f;
						}
					} 
					else 
					{
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
					}
					if (npc.ai[2] >= 70f) 
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num803;
						if (npc.ai[3] >= 2f) 
						{
							npc.ai[1] = -1f;
						} 
						else
						{
							npc.ai[1] = 1f;
						}
					}
				}
				else if (npc.ai[1] == 3f) 
				{
					npc.TargetClosest(true);
					float num412 = 32f;
					float num413 = 1.2f;
					int num414 = 1;
					if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width) 
					{
						num414 = -1;
					}
					Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num415 = player.position.X + (float)(player.width / 2) + (float)(num414 * 600) - vector40.X;
					float num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
					float num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
					num417 = num412 / num417;
					num415 *= num417;
					num416 *= num417;
					if (npc.velocity.X < num415) 
					{
						npc.velocity.X = npc.velocity.X + num413;
						if (npc.velocity.X < 0f && num415 > 0f) 
						{
							npc.velocity.X = npc.velocity.X + num413;
						}
					} 
					else if (npc.velocity.X > num415) 
					{
						npc.velocity.X = npc.velocity.X - num413;
						if (npc.velocity.X > 0f && num415 < 0f) 
						{
							npc.velocity.X = npc.velocity.X - num413;
						}
					}
					if (npc.velocity.Y < num416) 
					{
						npc.velocity.Y = npc.velocity.Y + num413;
						if (npc.velocity.Y < 0f && num416 > 0f) 
						{
							npc.velocity.Y = npc.velocity.Y + num413;
						}
					} 
					else if (npc.velocity.Y > num416) 
					{
						npc.velocity.Y = npc.velocity.Y - num413;
						if (npc.velocity.Y > 0f && num416 < 0f) 
						{
							npc.velocity.Y = npc.velocity.Y - num413;
						}
					}
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 480f) 
					{
						npc.ai[1] = -1f;
						npc.target = 255;
						npc.netUpdate = true;
					} 
					else
					{
						if (!player.dead) 
						{
							npc.ai[3] += wormAlive ? 0.5f : 1f;
						}
						if (npc.ai[3] >= 20f) 
						{
							npc.ai[3] = 0f;
							vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num415 = player.position.X + (float)(player.width / 2) - vector40.X;
							num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
							if (Main.netMode != 1) 
							{
								float num418 = 10f * uDieLul;
								int num419 = expertMode ? 150 : 200; //600 500
                                int num420 = mod.ProjectileType("BrimstoneHellblast");
								num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
								num417 = num418 / num417;
								num415 *= num417;
								num416 *= num417;
								vector40.X += num415 * 4f;
								vector40.Y += num416 * 4f;
								Projectile.NewProjectile(vector40.X, vector40.Y, num415, num416, num420, num419, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
				else if (npc.ai[1] == 4f)
				{
					int num831 = 1;
					if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
					{
						num831 = -1;
					}
					float num832 = 32f;
					float num833 = 1.2f;
					Vector2 vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num834 = player.position.X + (float)(player.width / 2) + (float)(num831 * 750) - vector83.X; //600
					float num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
					float num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
					num836 = num832 / num836;
					num834 *= num836;
					num835 *= num836;
					if (npc.velocity.X < num834)
					{
						npc.velocity.X = npc.velocity.X + num833;
						if (npc.velocity.X < 0f && num834 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num833;
						}
					}
					else if (npc.velocity.X > num834)
					{
						npc.velocity.X = npc.velocity.X - num833;
						if (npc.velocity.X > 0f && num834 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num833;
						}
					}
					if (npc.velocity.Y < num835)
					{
						npc.velocity.Y = npc.velocity.Y + num833;
						if (npc.velocity.Y < 0f && num835 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num833;
						}
					}
					else if (npc.velocity.Y > num835)
					{
						npc.velocity.Y = npc.velocity.Y - num833;
						if (npc.velocity.Y > 0f && num835 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num833;
						}
					}
					vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num834 = player.position.X + (float)(player.width / 2) - vector83.X;
					num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
					npc.rotation = (float)Math.Atan2((double)num835, (double)num834) - 1.57f;
					if (Main.netMode != 1)
					{
						npc.localAI[1] += wormAlive ? 0.5f : 1f;
						if (npc.localAI[1] > 140f)
						{
							npc.localAI[1] = 0f;
							float num837 = 5f * uDieLul;
							int num838 = expertMode ? 150 : 200; //600 500
                            int num839 = mod.ProjectileType("BrimstoneFireblast");
							num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
							num836 = num837 / num836;
							num834 *= num836;
							num835 *= num836;
							vector83.X += num834 * 15f;
							vector83.Y += num835 * 15f;
							Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838, 0f, Main.myPlayer, 0f, 0f);
						}
					}
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 300f)
					{
						npc.ai[1] = -1f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
						return;
					}
				}
				if (npc.ai[1] == -1f) 
				{
                    phaseChange++;
                    if (phaseChange > 23)
                    {
                        phaseChange = 0;
                    }
                    int phase = 0; //0 = shots above 1 = charge 2 = nothing 3 = hellblasts 4 = fireblasts
                    switch (phaseChange)
                    {
                        case 0: phase = 0; willCharge = false; break; //0341
                        case 1: phase = 3; break;
                        case 2: phase = 4; willCharge = true; break;
                        case 3: phase = 1; break;
                        case 4: phase = 1; break; //1430
                        case 5: phase = 4; willCharge = false; break;
                        case 6: phase = 3; break;
                        case 7: phase = 0; willCharge = true; break;
                        case 8: phase = 1; break; //1034
                        case 9: phase = 0; willCharge = false; break;
                        case 10: phase = 3; break;
                        case 11: phase = 4; break;
                        case 12: phase = 4; break; //4310
                        case 13: phase = 3; willCharge = true; break;
                        case 14: phase = 1; break;
                        case 15: phase = 0; willCharge = false; break;
                        case 16: phase = 4; break; //4411
                        case 17: phase = 4; willCharge = true; break;
                        case 18: phase = 1; break;
                        case 19: phase = 1; break;
                        case 20: phase = 0; break; //0101
                        case 21: phase = 1; break;
                        case 22: phase = 0; break;
                        case 23: phase = 1; break;
                    }
					npc.ai[1] = (float)phase;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					return;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.4) 
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
					return;
				}
			}
            #endregion
            #region Transition
            else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
			{
				npc.dontTakeDamage = true;
				npc.chaseable = false;
				if (npc.ai[0] == 1f) 
				{
					npc.ai[2] += 0.005f;
					if ((double)npc.ai[2] > 0.5) 
					{
						npc.ai[2] = 0.5f;
					}
				}
				else 
				{
					npc.ai[2] -= 0.005f;
					if (npc.ai[2] < 0f) 
					{
						npc.ai[2] = 0f;
					}
				}
				npc.rotation += npc.ai[2];
				npc.ai[1] += 1f;
				if (npc.ai[1] == 100f) 
				{
					npc.ai[0] += 1f;
					npc.ai[1] = 0f;
					if (npc.ai[0] == 3f) 
					{
						npc.ai[2] = 0f;
					} 
					else 
					{
						for (int num388 = 0; num388 < 50; num388++) 
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 235, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
						}
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					}
				}
				Dust.NewDust(npc.position, npc.width, npc.height, 235, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
				npc.velocity.X = npc.velocity.X * 0.98f;
				npc.velocity.Y = npc.velocity.Y * 0.98f;
				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1) 
				{
					npc.velocity.X = 0f;
				}
				if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1) 
				{
					npc.velocity.Y = 0f;
					return;
				}
			}
            #endregion
            #region LastStage
            else
			{
                npc.damage = expertMode ? 720 : 450;
                if (NPC.AnyNPCs(mod.NPCType("SCalWormHead")))
                {
                    wormAlive = true;
                    npc.dontTakeDamage = true;
                    npc.chaseable = false;
                }
                else
                {
                    if (NPC.AnyNPCs(mod.NPCType("SoulSeekerSupreme")))
                    {
                        npc.dontTakeDamage = true;
                        npc.chaseable = false;
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                        npc.chaseable = true;
                    }
                    wormAlive = false;
                }
                if (npc.ai[1] == 0f)
				{
					float num823 = 12f;
					float num824 = 0.12f;
					Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
					float num826 = player.position.Y + (float)(player.height / 2) - 550f - vector82.Y;
					float num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
					num827 = num823 / num827;
					num825 *= num827;
					num826 *= num827;
					if (npc.velocity.X < num825)
					{
						npc.velocity.X = npc.velocity.X + num824;
						if (npc.velocity.X < 0f && num825 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num824;
						}
					}
					else if (npc.velocity.X > num825)
					{
						npc.velocity.X = npc.velocity.X - num824;
						if (npc.velocity.X > 0f && num825 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num824;
						}
					}
					if (npc.velocity.Y < num826)
					{
						npc.velocity.Y = npc.velocity.Y + num824;
						if (npc.velocity.Y < 0f && num826 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num824;
						}
					}
					else if (npc.velocity.Y > num826)
					{
						npc.velocity.Y = npc.velocity.Y - num824;
						if (npc.velocity.Y > 0f && num826 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num824;
						}
					}
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 240f)
					{
						npc.ai[1] = -1f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
					}
					vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num825 = player.position.X + (float)(player.width / 2) - vector82.X;
					num826 = player.position.Y + (float)(player.height / 2) - vector82.Y;
					npc.rotation = (float)Math.Atan2((double)num826, (double)num825) - 1.57f;
					if (Main.netMode != 1)
					{
						npc.localAI[1] += wormAlive ? 0.5f : 1f;
						if (npc.localAI[1] > 60f)
						{
							npc.localAI[1] = 0f;
							float num828 = 10f * uDieLul;
							int num829 = expertMode ? 150 : 200; //600 500
                            Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
							float num181 = Math.Abs(num180) * 0.1f;
							float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
							float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
							num183 = num828 / num183;
							num180 *= num183;
							num182 *= num183;
							value9.X += num180;
							value9.Y += num182;
							int randomShot = Main.rand.Next(6);
							if (randomShot == 0)
							{
								randomShot = mod.ProjectileType("BrimstoneFireblast");
								num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
								num827 = num828 / num827;
								num825 *= num827;
								num826 *= num827;
								vector82.X += num825 * 15f;
								vector82.Y += num826 * 15f;
								int shot = Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
							}
							else if (randomShot <= 4)
							{
								randomShot = mod.ProjectileType("BrimstoneBarrage");
                                for (int num186 = 1; num186 <= 8; num186++)
                                {
                                    num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                                    num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                                    num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                    float speedBoost = (float)(num186 > 4 ? -(num186 - 4) : num186);
                                    num183 = (8f + speedBoost) / num183;
                                    num180 *= num183;
                                    num182 *= num183;
                                    Projectile.NewProjectile(value9.X, value9.Y, num180, num182, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
							else
							{
								randomShot = mod.ProjectileType("BrimstoneGigaBlast");
								num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
								num827 = num828 / num827;
								num825 *= num827;
								num826 *= num827;
								vector82.X += num825 * 15f;
								vector82.Y += num826 * 15f;
								int shot = Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
							}
							return;
						}
					}
				}
				else if (npc.ai[1] == 1f)
				{
					npc.rotation = num803; //change
					float num383 = wormAlive ? 31f : 35f;
					if ((double)npc.life < (double)npc.lifeMax * 0.3) 
					{
						num383 += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.2) 
					{
						num383 += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1) 
					{
						num383 += 1f;
					}
					Vector2 vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num384 = player.position.X + (float)(player.width / 2) - vector37.X;
					float num385 = player.position.Y + (float)(player.height / 2) - vector37.Y;
					float num386 = (float)Math.Sqrt((double)(num384 * num384 + num385 * num385));
					num386 = num383 / num386;
					npc.velocity.X = num384 * num386;
					npc.velocity.Y = num385 * num386;
					npc.ai[1] = 2f;
				} 
				else if (npc.ai[1] == 2f) 
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 25f) 
					{
						npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.Y = npc.velocity.Y * 0.96f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1) 
						{
							npc.velocity.X = 0f;
						}
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1) 
						{
							npc.velocity.Y = 0f;
						}
					} 
					else 
					{
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
					}
					if (npc.ai[2] >= 70f) 
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num803; //change
						if (npc.ai[3] >= 1f) 
						{
							npc.ai[1] = -1f;
						} 
						else
						{
							npc.ai[1] = 1f;
						}
					}
				}
				else if (npc.ai[1] == 3f) 
				{
					npc.TargetClosest(true);
					float num412 = 32f;
					float num413 = 1.2f;
					int num414 = 1;
					if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width) 
					{
						num414 = -1;
					}
					Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num415 = player.position.X + (float)(player.width / 2) + (float)(num414 * 600) - vector40.X;
					float num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
					float num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
					num417 = num412 / num417;
					num415 *= num417;
					num416 *= num417;
					if (npc.velocity.X < num415) 
					{
						npc.velocity.X = npc.velocity.X + num413;
						if (npc.velocity.X < 0f && num415 > 0f) 
						{
							npc.velocity.X = npc.velocity.X + num413;
						}
					}
					else if (npc.velocity.X > num415) 
					{
						npc.velocity.X = npc.velocity.X - num413;
						if (npc.velocity.X > 0f && num415 < 0f) 
						{
							npc.velocity.X = npc.velocity.X - num413;
						}
					}
					if (npc.velocity.Y < num416) 
					{
						npc.velocity.Y = npc.velocity.Y + num413;
						if (npc.velocity.Y < 0f && num416 > 0f) 
						{
							npc.velocity.Y = npc.velocity.Y + num413;
						}
					}
					else if (npc.velocity.Y > num416) 
					{
						npc.velocity.Y = npc.velocity.Y - num413;
						if (npc.velocity.Y > 0f && num416 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num413;
						}
					}
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 300f) 
					{
						npc.ai[1] = -1f;
						npc.target = 255;
						npc.netUpdate = true;
					}
					else
					{
						if (!player.dead) 
						{
							npc.ai[3] += wormAlive ? 0.5f : 1f;
                        }
						if (npc.ai[3] >= 24f)
						{
							npc.ai[3] = 0f;
							vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num415 = player.position.X + (float)(player.width / 2) - vector40.X;
							num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
							if (Main.netMode != 1) 
							{
								float num418 = 10f * uDieLul;
								int num419 = expertMode ? 150 : 200; //600 500
                                int num420 = mod.ProjectileType("BrimstoneHellblast");
								num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
								num417 = num418 / num417;
								num415 *= num417;
								num416 *= num417;
								vector40.X += num415 * 4f;
								vector40.Y += num416 * 4f;
								Projectile.NewProjectile(vector40.X, vector40.Y, num415, num416, num420, num419, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
				else if (npc.ai[1] == 4f)
				{
					int num831 = 1;
					if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
					{
						num831 = -1;
					}
					float num832 = 32f;
					float num833 = 1.2f;
					Vector2 vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num834 = player.position.X + (float)(player.width / 2) + (float)(num831 * 750) - vector83.X; //600
					float num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
					float num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
					num836 = num832 / num836;
					num834 *= num836;
					num835 *= num836;
					if (npc.velocity.X < num834)
					{
						npc.velocity.X = npc.velocity.X + num833;
						if (npc.velocity.X < 0f && num834 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num833;
						}
					}
					else if (npc.velocity.X > num834)
					{
						npc.velocity.X = npc.velocity.X - num833;
						if (npc.velocity.X > 0f && num834 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num833;
						}
					}
					if (npc.velocity.Y < num835)
					{
						npc.velocity.Y = npc.velocity.Y + num833;
						if (npc.velocity.Y < 0f && num835 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num833;
						}
					}
					else if (npc.velocity.Y > num835)
					{
						npc.velocity.Y = npc.velocity.Y - num833;
						if (npc.velocity.Y > 0f && num835 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num833;
						}
					}
					vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num834 = player.position.X + (float)(player.width / 2) - vector83.X;
					num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
					npc.rotation = (float)Math.Atan2((double)num835, (double)num834) - 1.57f;
					if (Main.netMode != 1)
					{
						npc.localAI[1] += wormAlive ? 0.5f : 1f;
                        if (npc.localAI[1] > 100f)
						{
							npc.localAI[1] = 0f;
							float num837 = 5f * uDieLul;
							int num838 = expertMode ? 150 : 200; //600 500
                            int num839 = mod.ProjectileType("BrimstoneFireblast");
							num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
							num836 = num837 / num836;
							num834 *= num836;
							num835 *= num836;
							vector83.X += num834 * 15f;
							vector83.Y += num835 * 15f;
							int shot = Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838, 0f, Main.myPlayer, 0f, 0f);
						}
					}
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 240f)
					{
						npc.ai[1] = -1f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
						return;
					}
				}
				if (npc.ai[1] == -1f) 
				{
                    phaseChange++;
                    if (phaseChange > 23)
                    {
                        phaseChange = 0;
                    }
                    int phase = 0; //0 = shots above 1 = charge 2 = nothing 3 = hellblasts 4 = fireblasts
                    switch (phaseChange)
                    {
                        case 0: phase = 0; willCharge = false; break; //0341
                        case 1: phase = 3; break;
                        case 2: phase = 4; willCharge = true; break;
                        case 3: phase = 1; break;
                        case 4: phase = 1; break; //1430
                        case 5: phase = 4; willCharge = false; break;
                        case 6: phase = 3; break;
                        case 7: phase = 0; willCharge = true; break;
                        case 8: phase = 1; break; //1034
                        case 9: phase = 0; willCharge = false; break;
                        case 10: phase = 3; break;
                        case 11: phase = 4; break;
                        case 12: phase = 4; break; //4310
                        case 13: phase = 3; willCharge = true; break;
                        case 14: phase = 1; break;
                        case 15: phase = 0; willCharge = false; break;
                        case 16: phase = 4; break; //4411
                        case 17: phase = 4; willCharge = true; break;
                        case 18: phase = 1; break;
                        case 19: phase = 1; break;
                        case 20: phase = 0; break; //0101
                        case 21: phase = 1; break;
                        case 22: phase = 0; break;
                        case 23: phase = 1; break;
                    }
                    npc.ai[1] = (float)phase;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    return;
                }
			}
            #endregion
        }

        #region Loot
        public override void NPCLoot()
		{
            if (lootTimer < 6000) //75 seconds for bullet hells + 25 seconds for normal phases
            {
                string key = "Mods.CalamityMod.SupremeBossText2";
                Color messageColor = Color.Orange;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                return;
            }
            if (Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).sCalKillCount == 0) //first time you kill her
            {
                if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalDeathCount == 0)
                {
                    string key = "Mods.CalamityMod.SupremeBossText16"; //die no times
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                else if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalDeathCount == 1)
                {
                    string key = "Mods.CalamityMod.SupremeBossText17"; //die one time
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                else if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalDeathCount == 2)
                {
                    string key = "Mods.CalamityMod.SupremeBossText18"; //die two times
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                else if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).sCalDeathCount == 3)
                {
                    string key = "Mods.CalamityMod.SupremeBossText19"; //die three times
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CheatTestThing"));
                }
                else //die however many times
                {
                    string key = "Mods.CalamityMod.SupremeBossText10";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
            }
            else //all times after first kill
            {
                string key = "Mods.CalamityMod.SupremeBossText10";
                Color messageColor = Color.Orange;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }
            if (Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).sCalKillCount < 5)
            {
                Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).sCalKillCount++;
            }
            if (Main.expertMode)
			{
                npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("CalamitousEssence"), Main.rand.Next(30, 41), true);
                if (CalamityWorld.revenge)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Vehemenc"));
                }
                int itemChoice = Main.rand.Next(14);
				if (itemChoice == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Animus")); //done
				}
				else if (itemChoice == 1)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Azathoth")); //done
				}
				else if (itemChoice == 2)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Contagion")); //done
				}
				else if (itemChoice == 3)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DraconicDestruction")); //done
				}
				else if (itemChoice == 4)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Earth")); //done
				}
				else if (itemChoice == 5)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Megafleet")); //done
				}
				else if (itemChoice == 6)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RedSun")); //done
				}
				else if (itemChoice == 7)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RoyalKnives")); //done
				}
				else if (itemChoice == 8)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RoyalKnivesMelee")); //done
				}
				else if (itemChoice == 9)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Svantechnical")); //done
				}
				else if (itemChoice == 10)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TriactisTruePaladinianMageHammerofMight")); //done
				}
				else if (itemChoice == 11)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TriactisTruePaladinianMageHammerofMightMelee")); //done
				}
				else if (itemChoice == 12)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Svantechnical")); //done
				}
				else if (itemChoice == 13)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrystylCrusher")); //done
				}
			}
            else
            {
                npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("CalamitousEssence"), Main.rand.Next(20, 31), true);
            }
		}
        #endregion

        public override Color? GetAlpha(Color drawColor)
        {
            if (willCharge)
                return new Color(100, 0, 0, 0);
            return null;
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = mod.ItemType("SupremeHealingPotion");
		}

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == mod.ProjectileType("AngryChicken"))
            {
                damage /= 2;
            }
            if (projectile.type == mod.ProjectileType("ApothMark") || projectile.type == mod.ProjectileType("ApothJaws"))
            {
                damage /= 25;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
            if (damage > npc.lifeMax / 10)
            {
                damage = 0;
                return false;
            }
            double newDamage = (damage + (int)((double)defense * 0.25));
            float protection = (CalamityWorld.death ? 0.75f : 0.7f); //45%
            if (CalamityWorld.bossRushActive)
            {
                protection = 0.6f;
            }
            if (newDamage < 1.0)
            {
                newDamage = 1.0;
            }
            if (npc.ichor)
            {
                protection *= 0.9f; //41%
            }
            else if (npc.onFire2)
            {
                protection *= 0.91f;
            }
            if (startFifthAttack)
            {
                protection *= 1.2f; //90% or 84%
            }
            if (protectionBoost)
            {
                protection = 0.99f; //99%
            }
            if (newDamage >= 1.0)
			{
				newDamage = (double)((int)((double)(1f - protection) * newDamage));
				if (newDamage < 1.0)
				{
					newDamage = 1.0;
				}
			}
			damage = newDamage;
			return true;
		}

        public override bool CheckActive()
        {
            return canDespawn;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
            scale = 1.5f;
			return null;
		}
		
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}
		
		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 1.0;
			if (npc.frameCounter < 7.0)
			{
				npc.frame.Y = 0;
			}
			else if (npc.frameCounter < 14.0)
			{
				npc.frame.Y = frameHeight;
			}
			else if (npc.frameCounter < 21.0)
			{
				npc.frame.Y = frameHeight * 2;
			}
			else
			{
				npc.frameCounter = 0.0;
				npc.frame.Y = 0;
			}
			if (npc.ai[0] > 1f)
			{
				npc.frame.Y = npc.frame.Y + frameHeight * 3;
				return;
			}
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 100;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("VulnerabilityHex"), 600, true);
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 600, true);
			}
		}
	}
}