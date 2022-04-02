using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Magic;

namespace CalamityMod.NPCs.Abyss
{
    public class BoxJellyfish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Box Jellyfish");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 44;
            npc.width = 30;
            npc.height = 33;
            npc.defense = 5;
            npc.lifeMax = 90;
            npc.alpha = 20;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 0, 80);
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath28;
            banner = npc.type;
            bannerItem = ModContent.ItemType<BoxJellyfishBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (!npc.wet)
            {
                npc.rotation += npc.velocity.X * 0.1f;
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.98f;
                    if (npc.velocity.X > -0.01 && npc.velocity.X < 0.01)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                npc.velocity.Y = npc.velocity.Y + 0.2f;
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
                npc.ai[0] = 1f;
                return;
            }
            if (npc.collideX)
            {
                npc.velocity.X = npc.velocity.X * -1f;
                npc.direction *= -1;
            }
            if (npc.collideY)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                    npc.directionY = -1;
                    npc.ai[0] = -1f;
                }
                else if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y = Math.Abs(npc.velocity.Y);
                    npc.directionY = 1;
                    npc.ai[0] = 1f;
                }
            }
            bool flag16 = false;
            npc.TargetClosest(false);
            if (Main.player[npc.target].wet && !Main.player[npc.target].dead && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
            {
                flag16 = true;
            }
            if (flag16)
            {
                npc.localAI[2] = 1f;
                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
                npc.velocity *= 0.975f;
                float num263 = 0.8f;
                if (npc.velocity.X > -num263 && npc.velocity.X < num263 && npc.velocity.Y > -num263 && npc.velocity.Y < num263)
                {
                    npc.TargetClosest(true);
                    float num264 = CalamityWorld.death ? 12f : 8f;
                    Vector2 vector31 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num265 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector31.X;
                    float num266 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector31.Y;
                    float num267 = (float)Math.Sqrt(num265 * num265 + num266 * num266);
                    num267 = num264 / num267;
                    num265 *= num267;
                    num266 *= num267;
                    npc.velocity.X = num265;
                    npc.velocity.Y = num266;
                }
            }
            else
            {
                npc.localAI[2] = 0f;
                npc.velocity.X = npc.velocity.X + npc.direction * 0.02f;
                npc.rotation = npc.velocity.X * 0.4f;
                if (npc.velocity.X < -1f || npc.velocity.X > 1f)
                {
                    npc.velocity.X = npc.velocity.X * 0.95f;
                }
                if (npc.ai[0] == -1f)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.01f;
                    if (npc.velocity.Y < -1f)
                    {
                        npc.ai[0] = 1f;
                    }
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y + 0.01f;
                    if (npc.velocity.Y > 1f)
                    {
                        npc.ai[0] = -1f;
                    }
                }
                int num268 = (int)(npc.position.X + (npc.width / 2)) / 16;
                int num269 = (int)(npc.position.Y + (npc.height / 2)) / 16;
                if (Main.tile[num268, num269 - 1] == null)
                {
                    Main.tile[num268, num269 - 1] = new Tile();
                }
                if (Main.tile[num268, num269 + 1] == null)
                {
                    Main.tile[num268, num269 + 1] = new Tile();
                }
                if (Main.tile[num268, num269 + 2] == null)
                {
                    Main.tile[num268, num269 + 2] = new Tile();
                }
                if (Main.tile[num268, num269 - 1].liquid > 128)
                {
                    if (Main.tile[num268, num269 + 1].active())
                    {
                        npc.ai[0] = -1f;
                    }
                    else if (Main.tile[num268, num269 + 2].active())
                    {
                        npc.ai[0] = -1f;
                    }
                }
                else
                {
                    npc.ai[0] = 1f;
                }
                if (npc.velocity.Y > 1.2 || npc.velocity.Y < -1.2)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.99f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer1 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.1f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Venom, 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            int abyssShockerChance = Main.expertMode ? 40 : 50;
            DropHelper.DropItemCondition(npc, ModContent.ItemType<AbyssShocker>(), NPC.downedBoss3, abyssShockerChance, 1, 1);
            DropHelper.DropItemChance(npc, ItemID.JellyfishNecklace, 0.01f);
        }
    }
}
