using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.SummonItems;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod;
namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianBoss : ModNPC
    {
        private int spearType = 0;
        private int dustTimer = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Guardian");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 20f;
            npc.aiStyle = -1;
            npc.damage = 140;
            npc.width = 100;
            npc.height = 80;
            npc.defense = 50;
            npc.Calamity().RevPlusDR(0.4f);
            npc.LifeMaxNERB(102500, 112500, 1650000);
            double HPBoost = CalamityMod.CalamityConfig.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            aiType = -1;
            npc.boss = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Guardians");
            else
                music = MusicID.Boss1;
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.value = Item.buyPrice(0, 25, 0, 0);
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spearType);
            writer.Write(dustTimer);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spearType = reader.ReadInt32();
            dustTimer = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC.doughnutBoss = npc.whoAmI;

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            Player player = Main.player[npc.target];
            Vector2 vector = npc.Center;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[1] == 0f)
            {
                npc.localAI[1] = 1f;
                NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<ProfanedGuardianBoss2>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<ProfanedGuardianBoss3>(), 0, 0f, 0f, 0f, 0f, 255);
            }
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool isHoly = player.ZoneHoly;
            bool isHell = player.ZoneUnderworldHeight;
            npc.defense = (isHoly || isHell || CalamityWorld.bossRushActive) ? 50 : 99999;
            Vector2 vectorCenter = npc.Center;
            npc.TargetClosest(false);
            if (!Main.dayTime || !player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!Main.dayTime || !player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -10f);
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            if (npc.timeLeft < 1800)
            {
                npc.timeLeft = 1800;
            }
            bool flag100 = false;
            for (int num569 = 0; num569 < 200; num569++)
            {
                if ((Main.npc[num569].active && Main.npc[num569].type == ModContent.NPCType<ProfanedGuardianBoss2>()) || (Main.npc[num569].active && Main.npc[num569].type == ModContent.NPCType<ProfanedGuardianBoss3>()))
                {
                    flag100 = true;
                }
            }
            if (flag100)
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }
            if (Math.Sign(npc.velocity.X) != 0)
            {
                npc.spriteDirection = -Math.Sign(npc.velocity.X);
            }
            npc.spriteDirection = Math.Sign(npc.velocity.X);
            float num998 = 8f;
            float scaleFactor3 = 300f;
            float num999 = 800f;
            float num1000 = (lifeRatio < 0.75f || death) ? 14f : 16f;
            if (revenge)
            {
                num1000 *= 1.15f;
            }
            float num1001 = 5f;
            float scaleFactor4 = 0.8f;
            int num1002 = 0;
            float scaleFactor5 = 10f;
            float num1003 = 30f;
            float num1004 = 150f;
            float num1005 = (lifeRatio < 0.75f || death) ? 14f : 16f;
            if (revenge)
            {
                num1005 *= 1.15f;
            }
            float num1006 = 0.333333343f;
            float num1007 = 8f;
            num1006 *= num1005;
            int num1009 = (npc.ai[0] == 2f) ? 2 : 1;
            int num1010 = (npc.ai[0] == 2f) ? 80 : 60;
            for (int num1011 = 0; num1011 < 2; num1011++)
            {
                if (Main.rand.Next(3) < num1009)
                {
                    int num1012 = Dust.NewDust(npc.Center - new Vector2((float)num1010), num1010 * 2, num1010 * 2, 244, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
                    Main.dust[num1012].noGravity = true;
                    Main.dust[num1012].velocity *= 0.2f;
                    Main.dust[num1012].fadeIn = 1f;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
				float shootBoost = death ? 2f : 2f * (1f - lifeRatio);
                npc.localAI[0] += 1f + shootBoost;
                if (npc.localAI[0] >= (CalamityWorld.bossRushActive ? 210f : 240f))
                {
                    npc.localAI[0] = 0f;
                    npc.TargetClosest(true);

                    Main.PlaySound(SoundID.Item20, npc.position);

                    Vector2 center = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    int totalProjectiles = 18;
                    float spread = MathHelper.ToRadians(20);
                    double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
                    double deltaAngle = spread / (float)totalProjectiles; // Angle between each projectile, 0.04363325
                    float velocity = 5f;
                    int damage = expertMode ? 50 : 60;
                    int projectileType = ModContent.ProjectileType<ProfanedSpear>();

                    double offsetAngle;
                    int i;
                    switch (spearType)
                    {
                        case 0:
                            for (i = 0; i < 9; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i; // Used to be 32
                                Projectile.NewProjectile(center.X, center.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(center.X, center.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                            break;
                        case 1:
                            totalProjectiles = 12;
                            spread = MathHelper.ToRadians(30); // 30 degrees in radians = 0.523599
                            startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
                            deltaAngle = spread / (float)totalProjectiles; // Angle between each projectile, 0.04363325
                            velocity = 5.5f;
                            for (i = 0; i < 6; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i; // Used to be 32
                                Projectile.NewProjectile(center.X, center.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(center.X, center.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                            break;
                        case 2:
                            totalProjectiles = 8;
                            spread = MathHelper.ToRadians(45); // 30 degrees in radians = 0.523599
                            startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
                            deltaAngle = spread / (float)totalProjectiles; // Angle between each projectile, 0.04363325
                            velocity = 6f;
                            for (i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i; // Used to be 32
                                Projectile.NewProjectile(center.X, center.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(center.X, center.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                            break;
                        default:
                            break;
                    }

                    spearType++;
                    if (spearType > 2)
                        spearType = 0;
                }
            }
            if (npc.ai[0] == 0f)
            {
                float scaleFactor6 = num998;
                Vector2 center4 = npc.Center;
                Vector2 center5 = Main.player[npc.target].Center;
                Vector2 vector126 = center5 - center4;
                Vector2 vector127 = vector126 - Vector2.UnitY * scaleFactor3;
                float num1013 = vector126.Length();
                vector126 = Vector2.Normalize(vector126) * scaleFactor6;
                vector127 = Vector2.Normalize(vector127) * scaleFactor6;
                bool flag64 = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
                if (npc.ai[3] >= 120f)
                {
                    flag64 = true;
                }
                float num1014 = 8f;
                flag64 = flag64 && vector126.ToRotation() > 3.14159274f / num1014 && vector126.ToRotation() < 3.14159274f - 3.14159274f / num1014;
                if (num1013 > num999 || !flag64)
                {
                    npc.velocity.X = (npc.velocity.X * (num1000 - 1f) + vector127.X) / num1000;
                    npc.velocity.Y = (npc.velocity.Y * (num1000 - 1f) + vector127.Y) / num1000;
                    if (!flag64)
                    {
                        npc.ai[3] += 1f;
                        if (npc.ai[3] == 120f)
                        {
                            npc.netUpdate = true;
                        }
                    }
                    else
                    {
                        npc.ai[3] = 0f;
                    }
                }
                else
                {
                    npc.ai[0] = 1f;
                    npc.ai[2] = vector126.X;
                    npc.ai[3] = vector126.Y;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.velocity *= scaleFactor4;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= num1001)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                    Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]) + new Vector2((float)Main.rand.Next(-num1002, num1002 + 1), (float)Main.rand.Next(-num1002, num1002 + 1)) * 0.04f;
                    velocity.Normalize();
                    velocity *= scaleFactor5;
                    npc.velocity = velocity;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    dustTimer--;
                    if ((lifeRatio < 0.5f || death) && dustTimer <= 0)
                    {
                        Main.PlaySound(SoundID.Item20, npc.position);
                        int damage = expertMode ? 55 : 70;
                        Vector2 vector173 = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
                        int projectile = Projectile.NewProjectile((int)vector173.X, (int)vector173.Y, (float)(npc.direction * 2), 4f, ModContent.ProjectileType<FlareDust>(), damage, 0f, Main.myPlayer, 0f, 0f); //changed
                        Main.projectile[projectile].timeLeft = 120;
                        Main.projectile[projectile].velocity.X = 0f;
                        Main.projectile[projectile].velocity.Y = 0f;
                        dustTimer = 3;
                    }
                }
                float num1016 = num1003;
                npc.ai[1] += 1f;
                bool flag65 = Vector2.Distance(npc.Center, Main.player[npc.target].Center) > num1004 && npc.Center.Y > Main.player[npc.target].Center.Y;
                if ((npc.ai[1] >= num1016 && flag65) || npc.velocity.Length() < num1007)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.velocity /= 2f;
                    npc.netUpdate = true;
                    npc.ai[1] = 45f;
                    npc.ai[0] = 3f;
                }
                else
                {
                    Vector2 center6 = npc.Center;
                    Vector2 center7 = Main.player[npc.target].Center;
                    Vector2 vec2 = center7 - center6;
                    vec2.Normalize();
                    if (vec2.HasNaNs())
                    {
                        vec2 = new Vector2((float)npc.direction, 0f);
                    }
                    npc.velocity = (npc.velocity * (num1005 - 1f) + vec2 * (npc.velocity.Length() + num1006)) / num1005;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.ai[1] -= 1f;
                if (npc.ai[1] <= 0f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
                npc.velocity *= 0.98f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
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
            spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0);
            while (npc.ai[0] == 2f && Lighting.NotRetro && ((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)))
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
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Profaned Guardian";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void NPCLoot()
        {
            // Profaned Guardians have no actual drops and no treasure bag
            DropHelper.DropItemChance(npc, ModContent.ItemType<ProfanedGuardianMask>(), 7);
            DropHelper.DropItem(npc, ModContent.ItemType<ProfanedCore>());
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeProfanedGuardians>(), true, !CalamityWorld.downedGuardians);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedGuardians, 5, 2, 1);

            // Mark the Profaned Guardians as dead
            CalamityWorld.downedGuardians = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.OnFire, 600, true);
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
                Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA4"), 1f);
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
