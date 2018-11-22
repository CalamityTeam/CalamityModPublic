using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using CalamityMod.Tiles;

namespace CalamityMod.NPCs.AbyssNPCs
{
    [AutoloadBossHead]
    public class AquaticScourgeHead : ModNPC
	{
        public bool detectsPlayer = false;
        public bool flies = true;
        public const int minLength = 30;
        public const int maxLength = 31;
        public float speed = 5f; //10
        public float turnSpeed = 0.08f; //0.15
        bool TailSpawned = false;
        public bool despawning = false;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aquatic Scourge");
		}
		
		public override void SetDefaults()
		{
            npc.npcSlots = 16f;
			npc.damage = 100;
			npc.width = 100; //36
			npc.height = 90; //20
			npc.defense = 40;
			npc.aiStyle = -1;
            aiType = -1;
            npc.lifeMax = CalamityWorld.revenge ? 85000 : 73000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 100000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 8600000 : 7900000;
            }
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 15, 0, 0);
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            bossBag = mod.ItemType("AquaticScourgeBag");
            if (Main.expertMode)
            {
                npc.scale = 1.15f;
            }
        }
		
		public override void AI()
		{
            if (npc.justHit || (double)npc.life <= (double)npc.lifeMax * 0.99 || CalamityWorld.bossRushActive)
            {
                detectsPlayer = true;
                npc.damage = Main.expertMode ? 250 : 80;
                npc.boss = true;
                music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/AquaticScourge");
            }
            else
            {
                npc.damage = 0;
            }
            npc.chaseable = detectsPlayer;
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
            if (Main.netMode != 1)
            {
                if (!TailSpawned && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol = 0;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            if (num36 % 2 == 0)
                            {
                                lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AquaticScourgeBody"), npc.whoAmI);
                            }
                            else
                            {
                                lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AquaticScourgeBodyAlt"), npc.whoAmI);
                            }
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AquaticScourgeTail"), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = (float)npc.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        NetMessage.SendData(23, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
                if (detectsPlayer)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 120f : 180f))
                    {
                        int npcPoxX = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                        int npcPoxY = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                        if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 300f && 
                            !Main.tile[npcPoxX, npcPoxY].active())
                        {
                            npc.localAI[0] = 0f;
                            npc.TargetClosest(true);
                            npc.netUpdate = true;
                            int random = Main.rand.Next(3);
                            Main.PlaySound(3, (int)npc.Center.X, (int)npc.Center.Y, 8);
                            Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
                            if (random == 0 && NPC.CountNPCS(mod.NPCType("AquaticSeekerHead")) < 1)
                            {
                                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("AquaticSeekerHead"));
                            }
                            else if (random == 1 && NPC.CountNPCS(mod.NPCType("AquaticUrchin")) < 3)
                            {
                                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("AquaticUrchin"));
                            }
                            else if (NPC.CountNPCS(mod.NPCType("AquaticParasite")) < 8)
                            {
                                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("AquaticParasite"));
                            }
                        }
                    }
                    npc.localAI[1] += 1f;
                    if (npc.localAI[1] >= (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 300f : 450f))
                    {
                        int npcPoxX = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                        int npcPoxY = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                        if (!Main.tile[npcPoxX, npcPoxY].active() && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 300f)
                        {
                            npc.localAI[1] = 0f;
                            npc.TargetClosest(true);
                            npc.netUpdate = true;
                            BarfShitUp();
                        }
                    }
                }
            }
            bool notOcean = Main.player[npc.target].position.Y < 800f || 
                (double)Main.player[npc.target].position.Y > Main.worldSurface * 16.0 || 
                (Main.player[npc.target].position.X > 6400f && Main.player[npc.target].position.X < (float)(Main.maxTilesX * 16 - 6400));
            if (Main.player[npc.target].dead)
            {
                despawning = true;
                npc.TargetClosest(false);
                flies = false;
                npc.velocity.Y = npc.velocity.Y + 5f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 5f;
                }
                if ((double)npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int a = 0; a < 200; a++)
                    {
                        if (Main.npc[a].aiStyle == npc.aiStyle)
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }
            else
            {
                despawning = false;
            }
            int num180 = (int)(npc.position.X / 16f) - 1;
            int num181 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num182 = (int)(npc.position.Y / 16f) - 1;
            int num183 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
            if (num180 < 0)
            {
                num180 = 0;
            }
            if (num181 > Main.maxTilesX)
            {
                num181 = Main.maxTilesX;
            }
            if (num182 < 0)
            {
                num182 = 0;
            }
            if (num183 > Main.maxTilesY)
            {
                num183 = Main.maxTilesY;
            }
            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }
            bool canFly = flies;
            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);
            }
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
            if (!NPC.AnyNPCs(mod.NPCType("AquaticScourgeTail")))
            {
                npc.active = false;
            }
            float num188 = speed;
            float num189 = turnSpeed;
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            int num42 = -1;
            int num43 = (int)(Main.player[npc.target].Center.X / 16f);
            int num44 = (int)(Main.player[npc.target].Center.Y / 16f);
            for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
            {
                for (int num46 = num44; num46 <= num44 + 15; num46++)
                {
                    if (WorldGen.SolidTile2(num45, num46))
                    {
                        num42 = num46;
                        break;
                    }
                }
                if (num42 > 0)
                {
                    break;
                }
            }
            if (num42 > 0)
            {
                num42 *= 16;
                float num47 = (float)(num42 + (notOcean ? 800 : 400)); //800
                if (!detectsPlayer)
                {
                    num192 = num47;
                    if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < (notOcean ? 500f : 400f)) //500
                    {
                        if (npc.velocity.X > 0f)
                        {
                            num191 = Main.player[npc.target].Center.X + (notOcean ? 600f : 480f); //600
                        }
                        else
                        {
                            num191 = Main.player[npc.target].Center.X - (notOcean ? 600f : 480f); //600
                        }
                    }
                }
            }
            if (detectsPlayer)
            {
                num188 = 7f;
                num189 = 0.11f;
                if (!Main.player[npc.target].wet)
                {
                    num188 = 11f;
                    num189 = 0.14f;
                }
                if (notOcean)
                {
                    num188 = 15f;
                    num189 = 0.17f;
                }
            }
            float num48 = num188 * 1.3f;
            float num49 = num188 * 0.7f;
            float num50 = npc.velocity.Length();
            if (num50 > 0f)
            {
                if (num50 > num48)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num48;
                }
                else if (num50 < num49)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num49;
                }
            }
            if (!detectsPlayer)
            {
                for (int num51 = 0; num51 < 200; num51++)
                {
                    if (Main.npc[num51].active && Main.npc[num51].type == npc.type && num51 != npc.whoAmI)
                    {
                        Vector2 vector3 = Main.npc[num51].Center - npc.Center;
                        if (vector3.Length() < 400f) //400
                        {
                            vector3.Normalize();
                            vector3 *= 1000f;
                            num191 -= vector3.X; //-
                            num192 -= vector3.Y; //-
                        }
                    }
                }
            }
            else
            {
                for (int num52 = 0; num52 < 200; num52++)
                {
                    if (Main.npc[num52].active && Main.npc[num52].type == npc.type && num52 != npc.whoAmI)
                    {
                        Vector2 vector4 = Main.npc[num52].Center - npc.Center;
                        if (vector4.Length() < 60f) //60
                        {
                            vector4.Normalize();
                            vector4 *= 200f;
                            num191 -= vector4.X;
                            num192 -= vector4.Y;
                        }
                    }
                }
            }
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
                }
                catch
                {
                }
                npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = npc.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;
                if (num191 < 0f)
                {
                    npc.spriteDirection = -1;
                }
                else if (num191 > 0f)
                {
                    npc.spriteDirection = 1;
                }
            }
            else
            {
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                float num196 = System.Math.Abs(num191);
                float num197 = System.Math.Abs(num192);
                float num198 = num188 / num193;
                num191 *= num198;
                num192 *= num198;
                if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189;
                    }
                    else
                    {
                        if (npc.velocity.X > num191)
                        {
                            npc.velocity.X = npc.velocity.X - num189;
                        }
                    }
                    if (npc.velocity.Y < num192)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189;
                    }
                    else
                    {
                        if (npc.velocity.Y > num192)
                        {
                            npc.velocity.Y = npc.velocity.Y - num189;
                        }
                    }
                    if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                        }
                    }
                    if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num189 * 2f; //changed from 2
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num189 * 2f; //changed from 2
                        }
                    }
                }
                else
                {
                    if (num196 > num197)
                    {
                        if (npc.velocity.X < num191)
                        {
                            npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
                        }
                        else if (npc.velocity.X > num191)
                        {
                            npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
                        }
                        if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y + num189;
                            }
                            else
                            {
                                npc.velocity.Y = npc.velocity.Y - num189;
                            }
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num192)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                        }
                        else if (npc.velocity.Y > num192)
                        {
                            npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                        }
                        if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                            {
                                npc.velocity.X = npc.velocity.X + num189;
                            }
                            else
                            {
                                npc.velocity.X = npc.velocity.X - num189;
                            }
                        }
                    }
                }
            }
            npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
        }

        public void BarfShitUp()
        {
            Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 13);
            if (Main.netMode != 1)
            {
                Vector2 valueBoom = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float spreadBoom = 15f * 0.0174f;
                double startAngleBoom = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spreadBoom / 2;
                double deltaAngleBoom = spreadBoom / 8f;
                double offsetAngleBoom;
                int iBoom;
                int damageBoom = (Main.expertMode || CalamityWorld.bossRushActive) ? 28 : 33;
                for (iBoom = 0; iBoom < 15; iBoom++)
                {
                    int projectileType = (Main.rand.Next(2) == 0 ? mod.ProjectileType("SandTooth") : mod.ProjectileType("SandBlast"));
                    if (projectileType == mod.ProjectileType("SandTooth"))
                    {
                        damageBoom = Main.expertMode ? 24 : 30;
                    }
                    offsetAngleBoom = (startAngleBoom + deltaAngleBoom * (iBoom + iBoom * iBoom) / 2f) + 32f * iBoom;
                    int boom1 = Projectile.NewProjectile(valueBoom.X, valueBoom.Y, (float)(Math.Sin(offsetAngleBoom) * 6.5f), (float)(Math.Cos(offsetAngleBoom) * 6.5f), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                    int boom2 = Projectile.NewProjectile(valueBoom.X, valueBoom.Y, (float)(-Math.Sin(offsetAngleBoom) * 6.5f), (float)(-Math.Cos(offsetAngleBoom) * 6.5f), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                }
                damageBoom = Main.expertMode ? 31 : 36;
                int num320 = Main.rand.Next(5, 9);
                int num3;
                for (int num321 = 0; num321 < num320; num321 = num3 + 1)
                {
                    Vector2 vector15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector15.Normalize();
                    vector15 *= (float)Main.rand.Next(50, 401) * 0.01f;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, vector15.X, vector15.Y, mod.ProjectileType("SandPoisonCloud"), damageBoom, 0f, Main.myPlayer, 0f, (float)Main.rand.Next(-45, 1));
                    num3 = num321;
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if ((projectile.type == ProjectileID.HallowStar || projectile.type == ProjectileID.CrystalShard) && projectile.ranged)
            {
                damage /= 2;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion)
            {
                return detectsPlayer;
            }
            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur && spawnInfo.water && !NPC.AnyNPCs(mod.NPCType("AquaticScourgeHead")))
            {
                return 0.01f;
            }
            return 0f;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = mod.ItemType("SulphurousSand");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}

        public override bool CheckActive()
        {
            if (detectsPlayer && !Main.player[npc.target].dead && !despawning)
            {
                return false;
            }
            if (npc.timeLeft <= 0 && Main.netMode != 1)
            {
                for (int k = (int)npc.ai[0]; k > 0; k = (int)Main.npc[k].ai[0])
                {
                    if (Main.npc[k].active)
                    {
                        Main.npc[k].active = false;
                        if (Main.netMode == 2)
                        {
                            Main.npc[k].life = 0;
                            Main.npc[k].netSkip = -1;
                            NetMessage.SendData(23, -1, -1, null, k, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 360, true);
            player.AddBuff(BuffID.Venom, 360, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(mod.BuffType("MarkedforDeath"), 180);
            }
        }
	}
}