
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles;  

namespace CalamityMod.NPCs
{
    [AutoloadBossHead]
    public class StormWeaverHeadNaked : ModNPC
    {
        private const float BoltAngleSpread = 280;
        private int BoltCountdown = 0;
        private const float speed = 13f;
        private const float turnSpeed = 0.35f;
        private bool tail = false;
        private int minLength = 40;
        private int maxLength = 41;
        private int invinceTime = 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Weaver");
        }

        public override void SetDefaults()
        {
            npc.damage = 180;
            npc.npcSlots = 5f;
            npc.width = 74;
            npc.height = 74;
            npc.lifeMax = 100000;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            else
                music = MusicID.Boss3;
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                npc.value = Item.buyPrice(0, 35, 0, 0);
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Weaver");
                else
                    music = MusicID.Boss3;
                npc.lifeMax = 600000;
            }
            if (CalamityWorld.bossRushActive) // 30 seconds
            {
                npc.lifeMax = 3500000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath13;
            npc.netAlways = true;
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BoltCountdown);
            writer.Write(invinceTime);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BoltCountdown = reader.ReadInt32();
            invinceTime = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            if (invinceTime > 0)
            {
                invinceTime--;
                npc.damage = 0;
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.damage = npc.defDamage;
                npc.dontTakeDamage = false;
            }
            if (!Main.raining && !CalamityWorld.bossRushActive && CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                RainStart();
            }
            double lifeRatio = (double)npc.life / (double)npc.lifeMax;
            int lifePercentage = (int)(100.0 * lifeRatio);
            int BoltProjectiles = 2;
            if (lifePercentage < 33)
            {
                BoltProjectiles = 4;
            }
            else if (lifePercentage < 66)
            {
                BoltProjectiles = 3;
            }
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
            if (npc.alpha != 0)
            {
                for (int num934 = 0; num934 < 2; num934++)
                {
                    int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                    Main.dust[num935].noGravity = true;
                    Main.dust[num935].noLight = true;
                }
            }
            npc.alpha -= 12;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tail && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverBodyNaked>(), npc.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverTailNaked>(), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = (float)npc.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        npc.netUpdate = true;
                        Previous = lol;
                    }
                    tail = true;
                }
                int damage = expertMode ? 62 : 75;
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 180f : 300f))
                {
                    npc.localAI[0] = 0f;
                    npc.TargetClosest(true);
                    npc.netUpdate = true;
                    float xPos = Main.rand.NextBool(2) ? npc.position.X + 300f : npc.position.X - 300f;
                    Vector2 vector2 = new Vector2(xPos, npc.position.Y + Main.rand.Next(-300, 301));
                    Projectile.NewProjectile(vector2.X, vector2.Y, 0f, 0f, 465, damage, 0f, Main.myPlayer, 0f, 0f);
                }
                if (BoltCountdown == 0)
                {
                    BoltCountdown = 600;
                }
                if (BoltCountdown > 0)
                {
                    BoltCountdown--;
                    if (BoltCountdown == 0)
                    {
                        int speed2 = revenge ? 8 : 7;
                        if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                        {
                            speed2 += 1;
                        }
                        float spawnX2 = (float)Main.rand.Next(2001) - 1000f + Main.player[npc.target].Center.X;
                        float spawnY2 = -1000f + Main.player[npc.target].Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX2, spawnY2);
                        Vector2 baseVelocity = Main.player[npc.target].Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= speed2;
                        for (int i = 0; i < BoltProjectiles; i++)
                        {
                            Vector2 spawn2 = baseSpawn;
                            spawn2.X = spawn2.X + i * 30 - (BoltProjectiles * 15);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-BoltAngleSpread / 2 + (BoltAngleSpread * i / (float)BoltProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            Vector2 vector94 = Main.player[npc.target].Center - spawn2;
                            float ai = (float)Main.rand.Next(100);
                            Projectile.NewProjectile(spawn2.X, spawn2.Y, velocity.X, velocity.Y, 466, damage, 0f, Main.myPlayer, vector94.ToRotation(), ai);
                        }
                    }
                }
            }
            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = npc.velocity.Y - 10f;
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    npc.velocity.Y = npc.velocity.Y - 10f;
                }
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    CalamityWorld.DoGSecondStageCountdown = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                        netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                        netMessage.Send();
                    }
                    for (int num957 = 0; num957 < 200; num957++)
                    {
                        if (Main.npc[num957].aiStyle == npc.aiStyle)
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 10000f)
            {
                CalamityWorld.DoGSecondStageCountdown = 0;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
                for (int num957 = 0; num957 < 200; num957++)
                {
                    if (Main.npc[num957].aiStyle == npc.aiStyle)
                    {
                        Main.npc[num957].active = false;
                    }
                }
            }
            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }
            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);
            }

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
            float num188 = revenge ? 14f : 13f;
            float num189 = revenge ? 0.44f : 0.4f;
            if (!Main.player[npc.target].ZoneSkyHeight && CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                num188 *= 2f;
                num189 *= 2f;
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
            for (int num52 = 0; num52 < 200; num52++)
            {
                if (Main.npc[num52].active && Main.npc[num52].type == npc.type && num52 != npc.whoAmI)
                {
                    Vector2 vector4 = Main.npc[num52].Center - npc.Center;
                    if (vector4.Length() < 60f)
                    {
                        vector4.Normalize();
                        vector4 *= 200f;
                        num191 -= vector4.X;
                        num192 -= vector4.Y;
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
            npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
        }

        private void RainStart()
        {
            int num = 86400;
            int num2 = num / 24;
            Main.rainTime = Main.rand.Next(num2 * 8, num);
            if (Main.rand.NextBool(3))
            {
                Main.rainTime += Main.rand.Next(0, num2);
            }
            if (Main.rand.NextBool(4))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 2);
            }
            if (Main.rand.NextBool(5))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 2);
            }
            if (Main.rand.NextBool(6))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 3);
            }
            if (Main.rand.NextBool(7))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 4);
            }
            if (Main.rand.NextBool(8))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 5);
            }
            float num3 = 1f;
            if (Main.rand.NextBool(2))
            {
                num3 += 0.05f;
            }
            if (Main.rand.NextBool(3))
            {
                num3 += 0.1f;
            }
            if (Main.rand.NextBool(4))
            {
                num3 += 0.15f;
            }
            if (Main.rand.NextBool(5))
            {
                num3 += 0.2f;
            }
            Main.rainTime = (int)((float)Main.rainTime * num3);
            Main.raining = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Color color24 = npc.GetAlpha(drawColor);
            Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Texture2D texture2D3 = Main.npcTexture[npc.type];
            int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int y3 = num156 * (int)npc.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
            {
                Color color26 = npc.GetAlpha(color25);
                {
                    goto IL_6899;
                }
                IL_6881:
                num161 += num158;
                continue;
                IL_6899:
                float num164 = (float)(num157 - num161);
                if (num158 < 0)
                {
                    num164 = (float)(num159 - num161);
                }
                color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
                Vector2 value4 = npc.oldPos[num161];
                float num165 = npc.rotation;
                Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
                goto IL_6881;
            }
            var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.penetrate == -1 && !projectile.minion)
            {
                damage = (int)((double)damage * 0.2);
            }
            else if (projectile.penetrate > 1)
            {
                damage /= projectile.penetrate;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWNude"), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 30;
                npc.height = 30;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 20; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 40; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
            }
        }

        public override bool CheckDead()
        {
            for (int num569 = 0; num569 < 200; num569++)
            {
                if (Main.npc[num569].active && (Main.npc[num569].type == ModContent.NPCType<StormWeaverBodyNaked>() || Main.npc[num569].type == ModContent.NPCType<StormWeaverTailNaked>()))
                {
                    Main.npc[num569].active = false;
                }
            }
            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<StormWeaverHeadNaked>(),
                ModContent.NPCType<StormWeaverBodyNaked>(),
                ModContent.NPCType<StormWeaverTailNaked>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            // Only drop items if fought alone
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<ArmoredShell>(), true, 5, 8);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<TheStorm>(), 3);
                DropHelper.DropItemChance(npc, ModContent.ItemType<StormDragoon>(), 3);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<WeaverTrophy>(), 10);

                // Other
                bool lastSentinelKilled = CalamityWorld.downedSentinel1 && !CalamityWorld.downedSentinel2 && CalamityWorld.downedSentinel3;
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSentinels>(), true, lastSentinelKilled);
                DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSentinel2, 5, 2, 1);
            }

            // If DoG's fight is active, set the timer for Signus' phase
            if (CalamityWorld.DoGSecondStageCountdown > 7260)
            {
                CalamityWorld.DoGSecondStageCountdown = 7260;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

            // Mark Storm Weaver as dead
            CalamityWorld.downedSentinel2 = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
