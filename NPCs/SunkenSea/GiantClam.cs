using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.SunkenSea
{
    [AutoloadBossHead]
    public class GiantClam : ModNPC
    {
        private int hitAmount = 0;
        private int attack = -1; //-1 doing nothing, 0 = shell hiding, 1 = telestomp, 2 = pearl burst, 3 = pearl rain
        private bool attackAnim = false;
        private bool hasBeenHit = false;
        private bool statChange = false;
        private bool hide = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Clam");
            Main.npcFrameCount[npc.type] = 12;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.lavaImmune = true;
            npc.npcSlots = 5f;
            npc.damage = 50;
            npc.width = 160;
            npc.height = 120;
            npc.defense = 9999;
            npc.DR_NERD(0.3f);
            npc.lifeMax = Main.hardMode ? 7500 : 1250;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Main.hardMode ? Item.buyPrice(0, 8, 0, 0) : Item.buyPrice(0, 1, 0, 0);
            npc.HitSound = SoundID.NPCHit4;
            npc.knockBackResist = 0f;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<GiantClamBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hitAmount);
            writer.Write(attack);
            writer.Write(attackAnim);
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);
            writer.Write(hasBeenHit);
            writer.Write(statChange);
            writer.Write(hide);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hitAmount = reader.ReadInt32();
            attack = reader.ReadInt32();
            attackAnim = reader.ReadBoolean();
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
            statChange = reader.ReadBoolean();
            hide = reader.ReadBoolean();
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            if (npc.justHit && hitAmount < 5)
            {
                ++hitAmount;
                hasBeenHit = true;
            }
            npc.chaseable = hasBeenHit;
            if (hitAmount == 5)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[npc.target].dead && Main.player[npc.target].active)
                    {
                        player.AddBuff(ModContent.BuffType<Clamity>(), 2); //CLAM INVASION
                    }
                }

                if (!hide)
                    Lighting.AddLight(npc.Center, 0f, (255 - npc.alpha) * 2.5f / 255f, (255 - npc.alpha) * 2.5f / 255f);

                if (!statChange)
                {
                    npc.defense = 10;
                    npc.damage = Main.expertMode ? 100 : 50;
                    if (Main.hardMode)
                    {
                        npc.defense = 35;
                        npc.damage = Main.expertMode ? 200 : 100;
                    }
                    statChange = true;
                }

                if (npc.ai[0] < 240f)
                {
                    npc.ai[0] += 1f;
                    hide = false;
                }
                else
                {
                    if (attack == -1)
                    {
                        attack = Main.rand.Next(2);
                        if (attack == 0)
                        {
                            attack = Main.rand.Next(2); //rarer chance of doing the hiding clam
                        }
                    }
                    else if (attack == 0)
                    {
                        hide = true;
                        npc.defense = 9999;
                        npc.ai[1] += 1f;
                        if (npc.ai[1] >= 90f)
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            hide = false;
                            attack = -1;
                            npc.defense = Main.hardMode ? 35 : 10;
                            NPC.NewNPC((int)(npc.Center.X + 5), (int)npc.Center.Y, ModContent.NPCType<Clam>(), 0, 0f, 0f, 0f, 0f, 255);
                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<Clam>(), 0, 0f, 0f, 0f, 0f, 255);
                            NPC.NewNPC((int)(npc.Center.X - 5), (int)npc.Center.Y, ModContent.NPCType<Clam>(), 0, 0f, 0f, 0f, 0f, 255);
                        }
                    }
                    else if (attack == 1)
                    {
                        if (npc.ai[2] == 0f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                npc.TargetClosest(true);
                                npc.ai[2] = 1f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (npc.ai[2] == 1f)
                        {
                            npc.damage = 0;
                            npc.chaseable = false;
                            npc.dontTakeDamage = true;
                            npc.noGravity = true;
                            npc.noTileCollide = true;
                            npc.alpha += Main.hardMode ? 8 : 5;
                            if (npc.alpha >= 255)
                            {
                                npc.alpha = 255;
                                npc.position.X = player.Center.X - (float)(npc.width / 2);
                                npc.position.Y = player.Center.Y - (float)(npc.height / 2) + player.gfxOffY - 200f;
                                npc.position.X = npc.position.X - 15f;
                                npc.position.Y = npc.position.Y - 100f;
                                npc.ai[2] = 2f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (npc.ai[2] == 2f)
                        {
                            if (Main.rand.NextBool(2))
                            {
                                int num5 = Dust.NewDust(npc.position, npc.width, npc.height, 226, 0f, 0f, 200, default, 1.5f);
                                Main.dust[num5].noGravity = true;
                                Main.dust[num5].velocity *= 0.75f;
                                Main.dust[num5].fadeIn = 1.3f;
                                Vector2 vector = new Vector2((float)Main.rand.Next(-200, 201), (float)Main.rand.Next(-200, 201));
                                vector.Normalize();
                                vector *= (float)Main.rand.Next(100, 200) * 0.04f;
                                Main.dust[num5].velocity = vector;
                                vector.Normalize();
                                vector *= 34f;
                                Main.dust[num5].position = npc.Center - vector;
                            }
                            npc.alpha -= Main.hardMode ? 7 : 4;
                            if (npc.alpha <= 0)
                            {
                                npc.damage = Main.expertMode ? 100 : 50;
                                if (Main.hardMode)
                                {
                                    npc.damage = Main.expertMode ? 200 : 100;
                                }
                                npc.chaseable = true;
                                npc.dontTakeDamage = false;
                                npc.alpha = 0;
                                npc.ai[2] = 3f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (npc.ai[2] == 3f)
                        {
                            npc.velocity.Y += 0.8f;
                            attackAnim = true;
                            if (npc.Center.Y > (player.Center.Y - (float)(npc.height / 2) + player.gfxOffY - 15f))
                            {
                                npc.noTileCollide = false;
                                npc.ai[2] = 4f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (npc.ai[2] == 4f)
                        {
                            if (npc.velocity.Y == 0f)
                            {
                                npc.ai[2] = 0f;
                                npc.ai[0] = 0f;
                                npc.netUpdate = true;
                                npc.noGravity = false;
                                attack = -1;
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/ClamImpact"), (int)npc.position.X, (int)npc.position.Y);
                                for (int stompDustArea = (int)npc.position.X - 30; stompDustArea < (int)npc.position.X + npc.width + 60; stompDustArea += 30)
                                {
                                    for (int stompDustAmount = 0; stompDustAmount < 5; stompDustAmount++)
                                    {
                                        int stompDust = Dust.NewDust(new Vector2(npc.position.X - 30f, npc.position.Y + (float)npc.height), npc.width + 30, 4, 33, 0f, 0f, 100, default, 1.5f);
                                        Main.dust[stompDust].velocity *= 0.2f;
                                    }
                                    int stompGore = Gore.NewGore(new Vector2((float)(stompDustArea - 30), npc.position.Y + (float)npc.height - 12f), default, Main.rand.Next(61, 64), 1f);
                                    Main.gore[stompGore].velocity *= 0.4f;
                                }
                            }
                            npc.velocity.Y += 0.8f;
                        }
                    }
                }

                if (npc.ai[3] < 180f && Main.hardMode)
                {
                    npc.ai[3] += 1f;
                }
                else if (Main.hardMode)
                {
                    if (attack == -1)
                    {
                        attack = Main.rand.Next(2, 4);
                    }
                    else if (attack == 2)
                    {
                        Main.PlaySound(SoundID.Item67, npc.position);
                        Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int projectileShot = ModContent.ProjectileType<PearlBurst>();
                        int damage = Main.expertMode ? 28 : 35;
                        float speed = 5f;
                        Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                        float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                        float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                        num8 = speed / num8;
                        num6 *= num8;
                        num7 *= num8;
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, num6, num7, projectileShot, damage, 0f, Main.myPlayer, 0f, 0f);
                        for (int i = 0; i < 4; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(value9.X, value9.Y, (float)(Math.Sin(offsetAngle) * 3f), (float)(Math.Cos(offsetAngle) * 3f), projectileShot, damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(value9.X, value9.Y, (float)(-Math.Sin(offsetAngle) * 3f), (float)(-Math.Cos(offsetAngle) * 3f), projectileShot, damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        attack = -1;
                        npc.ai[3] = 0f;
                    }
                    else if (attack == 3)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Main.PlaySound(SoundID.Item68, npc.position);
                            int damage = Main.expertMode ? 28 : 35;
                            float shotSpacing = 750f;
                            for (int i = 0; i < 11; i++)
                            {
                                Projectile.NewProjectile(player.Center.X + shotSpacing, player.Center.Y - 750f, 0f, 8f, ModContent.ProjectileType<PearlRain>(), damage, 0f, Main.myPlayer, 0f, 0f);
                                shotSpacing -= 150f;
                            }
                        }
                        attack = -1;
                        npc.ai[3] = 0f;
                    }
                }
            }
        }

        public override bool CheckActive()
        {
            return Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 5600f;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.frameCounter > (attackAnim ? 2.0 : 5.0))
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = npc.frame.Y + frameHeight;
            }
            if (hitAmount < 5 || hide)
            {
                npc.frame.Y = frameHeight * 11;
            }
            else if (attackAnim)
            {
                if (npc.frame.Y < frameHeight * 3)
                {
                    npc.frame.Y = frameHeight * 3;
                }
                if (npc.frame.Y > frameHeight * 10)
                {
                    hide = true;
                    attackAnim = false;
                }
            }
            else
            {
                if (npc.frame.Y > frameHeight * 3)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water && CalamityWorld.downedDesertScourge && !NPC.AnyNPCs(ModContent.NPCType<GiantClam>()))
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.24f;
            }
            return 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 37, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 37, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantClam/GiantClam1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantClam/GiantClam2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantClam/GiantClam3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantClam/GiantClam4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantClam/GiantClam5"), 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor, true);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GiantClamGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GiantClamGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightBlue);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GiantClamGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void NPCLoot()
        {
            // Spawn Amidias if he isn't in the world
            //This doesn't check for Desert Scourge because Giant Clam only spawns post-Desert Scourge
            int amidiasNPC = NPC.FindFirstNPC(ModContent.NPCType<SEAHOE>());
            if (amidiasNPC == -1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SEAHOE>(), 0, 0f, 0f, 0f, 0f, 255);
            }

            // Materials
            DropHelper.DropItem(npc, ModContent.ItemType<Navystone>(), 25, 35);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<MolluskHusk>(), Main.hardMode, 6, 11);

            // Weapons
            if (Main.hardMode)
            {
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<ClamCrusher>(w),
                    DropHelper.WeightStack<ClamorRifle>(w),
                    DropHelper.WeightStack<Poseidon>(w),
                    DropHelper.WeightStack<ShellfishStaff>(w)
                );
            }

            // Equipment
            DropHelper.DropItemCondition(npc, ModContent.ItemType<GiantPearl>(), CalamityWorld.downedDesertScourge, 3, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<AmidiasPendant>(), CalamityWorld.downedDesertScourge, 3, 1, 1);

            // Mark Giant Clam as dead
            CalamityWorld.downedCLAM = true;
            CalamityWorld.downedCLAMHardMode = Main.hardMode || CalamityWorld.downedCLAMHardMode;
            CalamityNetcode.SyncWorld();
        }
    }
}
