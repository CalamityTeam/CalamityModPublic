using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Cryocore2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryocore");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 40;
            npc.width = 66;
            npc.height = 66;
            npc.defense = 10;
            npc.lifeMax = 300;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.knockBackResist = 0.5f;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath15;
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = false;
            npc.Calamity().VulnerableToSickness = false;
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
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.02f, 0.5f, 0.5f);
            npc.TargetClosest();
            float speed = 12f;
            Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
            float num1373 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector167.X;
            float num1374 = Main.player[npc.target].Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
            float num1376 = speed / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            npc.ai[0] -= 1f;
            if (num1375 < 200f || npc.ai[0] > 0f)
            {
                if (num1375 < 200f)
                {
                    npc.ai[0] = 20f;
                }
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                npc.rotation += (float)npc.direction * 0.3f;
                return;
            }
            npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
            npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
            }
            npc.rotation = npc.velocity.X * 0.15f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.ZoneSnow &&
                spawnInfo.player.ZoneOverworldHeight &&
                !spawnInfo.player.PillarZone() &&
                !spawnInfo.player.ZoneDungeon &&
                !spawnInfo.player.InSunkenSea() &&
                Main.hardMode && !spawnInfo.playerInTown && !spawnInfo.player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.01f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 90, true);
            player.AddBuff(BuffID.Chilled, 60, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Cryocore2"), 1f);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofEleum>(), 0.5f);
        }
    }
}
