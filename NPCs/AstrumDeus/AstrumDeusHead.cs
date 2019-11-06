using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    [AutoloadBossHead]
    public class AstrumDeusHead : ModNPC
    {
        private bool flies = false;
        private const int minLength = 4;
        private const int maxLength = 5;
        private int addOrbiter = 0;
        private int addOrbiter2 = 0;
        private float speed = 0.12f;
        private float turnSpeed = 0.08f;
        private bool tailSpawned = false;
        private bool secondStage = false;
        private bool thirdStage = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Deus");
        }

        public override void SetDefaults()
        {
            npc.damage = 100;
            npc.npcSlots = 5f;
            npc.width = 56;
            npc.height = 56;
            npc.defense = 20;
            npc.Calamity().RevPlusDR(0.1f);
            npc.lifeMax = CalamityWorld.revenge ? 18000 : 12000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 29100;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 420000 : 360000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.aiStyle = 6;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.scale = 1.2f;
            if (Main.expertMode)
            {
                npc.scale = 1.35f;
            }
            npc.alpha = 255;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/AstrumDeus");
            else
                music = MusicID.Boss3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(addOrbiter);
            writer.Write(addOrbiter2);
            writer.Write(secondStage);
            writer.Write(thirdStage);
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            addOrbiter = reader.ReadInt32();
            addOrbiter2 = reader.ReadInt32();
            secondStage = reader.ReadBoolean();
            thirdStage = reader.ReadBoolean();
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.astrumDeusHeadMain != -1)
            {
                if (Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].active)
                {
                    npc.dontTakeDamage = !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].dontTakeDamage;
                    npc.chaseable = !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].chaseable;
                }
            }
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                if (npc.life <= npc.lifeMax * 0.9f)
                {
                    if (secondStage == false && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 3; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 120) * 75)), (int)(npc.Center.Y + (Math.Cos(I * 120) * 75)), ModContent.NPCType<AstrumDeusProbe>(), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 120;
                            Eye.ai[3] = I * 120;
                        }
                        secondStage = true;
                    }
                }
                if (npc.life <= npc.lifeMax * 0.8f)
                {
                    if (thirdStage == false && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 5; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 72) * 150)), (int)(npc.Center.Y + (Math.Cos(I * 72) * 150)), ModContent.NPCType<AstrumDeusProbe2>(), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 72;
                            Eye.ai[3] = I * 72;
                        }
                        thirdStage = true;
                    }
                }
            }
            else
            {
                if (npc.life <= npc.lifeMax * 0.65f)
                {
                    if (secondStage == false && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 3; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 120) * 75)), (int)(npc.Center.Y + (Math.Cos(I * 120) * 75)), ModContent.NPCType<AstrumDeusProbe>(), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 120;
                            Eye.ai[3] = I * 120;
                        }
                        secondStage = true;
                    }
                }
                if (npc.life <= npc.lifeMax * 0.3f)
                {
                    if (thirdStage == false && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 5; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 72) * 150)), (int)(npc.Center.Y + (Math.Cos(I * 72) * 150)), ModContent.NPCType<AstrumDeusProbe2>(), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 72;
                            Eye.ai[3] = I * 72;
                        }
                        thirdStage = true;
                    }
                }
            }
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
            float speedAdd = expertMode ? 0.06f : 0.045f;
            float turnSpeedAdd = expertMode ? 0.04f : 0.03f;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                speedAdd *= 1.25f;
                turnSpeedAdd *= 1.25f;
            }
            float life = (float)npc.life;
            float totalLife = (float)npc.lifeMax;
            float newSpeed = speed + speedAdd * (1f - (life / totalLife));
            float newTurnSpeed = turnSpeed + turnSpeedAdd * (1f - (life / totalLife));
            if (npc.alpha != 0)
            {
                for (int num934 = 0; num934 < 2; num934++)
                {
                    int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                    Main.dust[num935].noGravity = true;
                    Main.dust[num935].noLight = true;
                }
            }
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tailSpawned && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AstrumDeusBody>(), npc.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AstrumDeusTail>(), npc.whoAmI);
                        }
                        if (num36 % 2 == 0)
                        {
                            Main.npc[lol].localAI[3] = 1f;
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = (float)npc.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        Previous = lol;
                    }
                    tailSpawned = true;
                }
                if (!npc.active && Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
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
            bool flag94 = flies;
            if (!flag94)
            {
                for (int num952 = num180; num952 < num181; num952++)
                {
                    for (int num953 = num182; num953 < num183; num953++)
                    {
                        if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[(int)Main.tile[num952, num953].type] || (Main.tileSolidTop[(int)Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
                        {
                            Vector2 vector105;
                            vector105.X = (float)(num952 * 16);
                            vector105.Y = (float)(num953 * 16);
                            if (npc.position.X + (float)npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + (float)npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
                            {
                                flag94 = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!flag94)
            {
                Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
                npc.localAI[1] = 1f;
                Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int rectX = 300;
                int rectY = (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 100 : 200;
                bool flag95 = true;
                if (npc.position.Y > Main.player[npc.target].position.Y)
                {
                    for (int num955 = 0; num955 < 255; num955++)
                    {
                        if (Main.player[num955].active)
                        {
                            Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - rectX, (int)Main.player[num955].position.Y - rectY, rectX * 2, rectY * 2);
                            if (rectangle12.Intersects(rectangle13))
                            {
                                flag95 = false;
                                break;
                            }
                        }
                    }
                    if (flag95)
                    {
                        flag94 = true;
                    }
                }
            }
            else
            {
                npc.localAI[1] = 0f;
            }
            float maxSpeed = 20f;
            if (Main.dayTime || Main.player[npc.target].dead || CalamityGlobalNPC.astrumDeusHeadMain < 0 || !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].active)
            {
                flag94 = true;
                npc.velocity.Y = npc.velocity.Y - 2f;
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    npc.velocity.Y = npc.velocity.Y - 2f;
                    maxSpeed = 40f;
                }
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    for (int num957 = 0; num957 < 200; num957++)
                    {
                        if (Main.npc[num957].aiStyle == npc.aiStyle)
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }
            float num188 = newSpeed;
            float num189 = newTurnSpeed;
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (!flag94)
            {
                npc.TargetClosest(true);
                npc.velocity.Y = npc.velocity.Y + 0.15f;
                if (npc.velocity.Y > maxSpeed)
                {
                    npc.velocity.Y = maxSpeed;
                }
                if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.4)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - num188 * 1.1f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X + num188 * 1.1f;
                    }
                }
                else if (npc.velocity.Y == maxSpeed)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num188;
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num188;
                    }
                }
                else if (npc.velocity.Y > 4f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num188 * 0.9f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num188 * 0.9f;
                    }
                }
            }
            else
            {
                if (!flies && npc.behindTiles && npc.soundDelay == 0)
                {
                    float num195 = num193 / 40f;
                    if (num195 < 10f)
                    {
                        num195 = 10f;
                    }
                    if (num195 > 20f)
                    {
                        num195 = 20f;
                    }
                    npc.soundDelay = (int)num195;
                    Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 1);
                }
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                float num196 = System.Math.Abs(num191);
                float num197 = System.Math.Abs(num192);
                float num198 = maxSpeed / num193;
                num191 *= num198;
                num192 *= num198;
                if (((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f)) && ((npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f)))
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189;
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189;
                    }
                    if (npc.velocity.Y < num192)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189;
                    }
                    else if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189;
                    }
                }
                if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num188;
                    }
                    else
                    {
                        if (npc.velocity.X > num191)
                        {
                            npc.velocity.X = npc.velocity.X - num188;
                        }
                    }
                    if (npc.velocity.Y < num192)
                    {
                        npc.velocity.Y = npc.velocity.Y + num188;
                    }
                    else
                    {
                        if (npc.velocity.Y > num192)
                        {
                            npc.velocity.Y = npc.velocity.Y - num188;
                        }
                    }
                    if ((double)System.Math.Abs(num192) < (double)maxSpeed * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num188 * 2f;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num188 * 2f;
                        }
                    }
                    if ((double)System.Math.Abs(num191) < (double)maxSpeed * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num188 * 2f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num188 * 2f;
                        }
                    }
                }
                else
                {
                    if (num196 > num197)
                    {
                        if (npc.velocity.X < num191)
                        {
                            npc.velocity.X = npc.velocity.X + num188 * 1.1f;
                        }
                        else if (npc.velocity.X > num191)
                        {
                            npc.velocity.X = npc.velocity.X - num188 * 1.1f;
                        }
                        if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y + num188;
                            }
                            else
                            {
                                npc.velocity.Y = npc.velocity.Y - num188;
                            }
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num192)
                        {
                            npc.velocity.Y = npc.velocity.Y + num188 * 1.1f;
                        }
                        else if (npc.velocity.Y > num192)
                        {
                            npc.velocity.Y = npc.velocity.Y - num188 * 1.1f;
                        }
                        if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                            {
                                npc.velocity.X = npc.velocity.X + num188;
                            }
                            else
                            {
                                npc.velocity.X = npc.velocity.X - num188;
                            }
                        }
                    }
                }
                npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
                if (flag94)
                {
                    if (npc.localAI[0] != 1f)
                    {
                        npc.netUpdate = true;
                    }
                    npc.localAI[0] = 1f;
                }
                else
                {
                    if (npc.localAI[0] != 0f)
                    {
                        npc.netUpdate = true;
                    }
                    npc.localAI[0] = 0f;
                }
                if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                {
                    npc.netUpdate = true;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !npc.dontTakeDamage;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (npc.dontTakeDamage)
                return new Color(125, 75, Main.DiscoB, npc.alpha);
            return null;
        }

        public override bool CheckActive()
        {
            return false;
        }

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (projectile.type == ModContent.ProjectileType<RainbowBoom>() || ProjectileID.Sets.StardustDragon[projectile.type])
			{
				damage = (int)((double)damage * 0.1);
			}
			else if (projectile.type == ModContent.ProjectileType<BigNuke>() || projectile.type == ModContent.ProjectileType<RainBolt>() ||
				projectile.type == ModContent.ProjectileType<AtlantisSpear2>() || projectile.type == ModContent.ProjectileType<MalachiteBolt>())
			{
				damage = (int)((double)damage * 0.2);
			}
			else if (projectile.type == ProjectileID.DD2BetsyArrow)
			{
				damage = (int)((double)damage * 0.3);
			}
			else if (projectile.type == ModContent.ProjectileType<SpikecragSpike>())
			{
				damage = (int)((double)damage * 0.5);
			}

			if (projectile.penetrate == -1 && !projectile.minion)
			{
				if (projectile.type == ModContent.ProjectileType<CosmicFire>())
					damage = (int)((double)damage * 0.3);
				else
					damage = (int)((double)damage * 0.2);
			}
			else if (projectile.penetrate > 1 && projectile.type != ModContent.ProjectileType<BrinySpout>())
			{
				damage /= projectile.penetrate;
			}
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
