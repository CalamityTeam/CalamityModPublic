using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Cnidrion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.8f,
            };
            value.Position.X += 48f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 3f;
            NPC.aiStyle = -1;
            NPC.damage = 20;
            NPC.width = 160;
            NPC.height = 80;
            NPC.defense = 6;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DR_NERD(0.05f);
            NPC.lifeMax = 280;
            NPC.knockBackResist = 0.05f;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 25, 0);
            NPC.HitSound = SoundID.NPCHit12;
            NPC.DeathSound = SoundID.NPCDeath18;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CnidrionBanner>();
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Cnidrion")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.PillarZone() ||
                spawnInfo.Player.InAstral() ||
                spawnInfo.Player.ZoneCorrupt ||
                spawnInfo.Player.ZoneCrimson ||
                spawnInfo.Player.ZoneOldOneArmy ||
                spawnInfo.Player.ZoneSkyHeight ||
                spawnInfo.PlayerSafe ||
                !spawnInfo.Player.ZoneDesert ||
                !spawnInfo.Player.ZoneOverworldHeight ||
                Main.eclipse ||
                Main.snowMoon ||
                Main.pumpkinMoon ||
                Main.invasionType != InvasionID.None)
                return 0f;

            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;

            return 0.05f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            bool expertMode = Main.expertMode;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            float num823 = 1f;
            NPC.TargetClosest(true);
            bool flag51 = false;
            int offsetX = 80;
            int projectileDamage = expertMode ? 9 : 12;
            if (NPC.life < NPC.lifeMax * 0.33 && CalamityWorld.death)
            {
                num823 = 2f;
            }
            if (NPC.life < NPC.lifeMax * 0.1 && CalamityWorld.death)
            {
                num823 = 4f;
            }
            if (Main.zenithWorld)
            {
                num823 = 8f;
            }
            if (NPC.ai[0] == 0f)
            {
                NPC.ai[1] += 1f;
                if (NPC.life < NPC.lifeMax * 0.33 && CalamityWorld.death)
                {
                    NPC.ai[1] += 1f;
                }
                if (NPC.life < NPC.lifeMax * 0.1 && CalamityWorld.death)
                {
                    NPC.ai[1] += 1f;
                }
                if (NPC.ai[1] >= 300f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[1] = 0f;
                    if (NPC.life < NPC.lifeMax * 0.25)
                    {
                        NPC.ai[0] = Main.rand.Next(3, 5);
                    }
                    else if (CalamityWorld.death && NPC.life < NPC.lifeMax * 0.6)
                    {
                        NPC.ai[0] = Main.rand.Next(1, 5);
                    }
                    else
                    {
                        NPC.ai[0] = Main.rand.Next(1, 3);
                    }
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                flag51 = true;
                NPC.ai[1] += 1f;
                if (NPC.ai[1] % 30f == 0f)
                {
                    Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + 20f);
                    vector18.X += offsetX * NPC.direction;
                    float num829 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[NPC.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt(num829 * num829 + num830 * num830);
                    float num832 = 6f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                    num830 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), projectileDamage, 0f, Main.myPlayer);
                }
                if (NPC.ai[1] >= 120f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                flag51 = true;
                NPC.ai[1] += 1f;
                if (NPC.ai[1] > 60f && NPC.ai[1] < 240f && NPC.ai[1] % 16f == 0f)
                {
                    Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + 20f);
                    vector18.X += offsetX * NPC.direction;
                    float num829 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[NPC.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt(num829 * num829 + num830 * num830);
                    float num832 = 8f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + Main.rand.Next(-15, 16) * 0.01f;
                    num830 *= 1f + Main.rand.Next(-15, 16) * 0.01f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), projectileDamage, 0f, Main.myPlayer);
                }
                if (NPC.ai[1] >= 300f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                num823 = 4f;
                NPC.ai[1] += 1f;
                if (NPC.ai[1] % 30f == 0f)
                {
                    Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + 20f);
                    vector18.X += offsetX * NPC.direction;
                    float num844 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - vector18.X;
                    float num845 = Main.player[NPC.target].position.Y - vector18.Y;
                    float num846 = (float)Math.Sqrt(num844 * num844 + num845 * num845);
                    float num847 = 10f;
                    num846 = num847 / num846;
                    num844 *= num846;
                    num845 *= num846;
                    num844 *= 1f + Main.rand.Next(-10, 11) * 0.001f;
                    num845 *= 1f + Main.rand.Next(-10, 11) * 0.001f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), vector18.X, vector18.Y, num844, num845, ModContent.ProjectileType<HorsWaterBlast>(), projectileDamage, 0f, Main.myPlayer);
                }
                if (NPC.ai[1] >= 120f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                }
            }
            else if (NPC.ai[0] == 4f)
            {
                num823 = 4f;
                NPC.ai[1] += 1f;
                if (NPC.ai[1] % 20f == 0f)
                {
                    Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + 20f);
                    vector18.X += offsetX * NPC.direction;
                    float num829 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[NPC.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt(num829 * num829 + num830 * num830);
                    float num832 = 11f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + Main.rand.Next(-5, 6) * 0.01f;
                    num830 *= 1f + Main.rand.Next(-5, 6) * 0.01f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), projectileDamage, 0f, Main.myPlayer);
                }
                if (NPC.ai[1] >= 240f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                }
            }

            if (Math.Abs(NPC.Center.X - player.Center.X) < 50f)
                flag51 = true;

            if (flag51)
            {
                NPC.velocity.X *= 0.9f;
                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;
            }
            else
            {
                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;

                if (NPC.direction > 0)
                    NPC.velocity.X = (NPC.velocity.X * 20f + num823) / 21f;
                if (NPC.direction < 0)
                    NPC.velocity.X = (NPC.velocity.X * 20f - num823) / 21f;
            }

            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && player.position.Y <= NPC.position.Y + NPC.height && !NPC.collideX)
            {
                NPC.noGravity = false;
                NPC.noTileCollide = false;
            }
            else
            {
                NPC.noGravity = true;
                NPC.noTileCollide = true;

                int num854 = 80;
                int num855 = 20;
                Vector2 position2 = new Vector2(NPC.Center.X - (num854 / 2), NPC.position.Y + NPC.height - num855);
                if (Collision.SolidCollision(position2, num854, num855))
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y = 0f;

                    if (NPC.velocity.Y > -0.2f)
                        NPC.velocity.Y -= 0.025f;
                    else
                        NPC.velocity.Y -= 0.2f;

                    if (NPC.velocity.Y < -2f)
                        NPC.velocity.Y = -2f;
                }
                else
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y = 0f;

                    if (NPC.velocity.Y < 0.5f)
                        NPC.velocity.Y += 0.025f;
                    else
                        NPC.velocity.Y += 0.25f;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, hit.HitDirection, -1f, 0, default, 2f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<AmidiasSpark>(), 4);
            npcLoot.Add(ItemID.FossilOre, 1, 4, 5);
        }
    }
}
