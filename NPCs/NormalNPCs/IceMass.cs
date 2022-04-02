using CalamityMod.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class IceMass : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aurora Spirit");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 86;
            npc.damage = 40;
            npc.width = 40;
            npc.height = 24;
            npc.defense = 8;
            npc.alpha = 100;
            npc.lifeMax = 50;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath15;
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = false;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            int num1 = 1;
            if (!Main.dedServ)
            {
                if (!Main.NPCLoaded[npc.type] || Main.npcTexture[npc.type] == null)
                    return;
                num1 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            }
            if (npc.velocity.X < 0f)
                npc.direction = -1;
            else
                npc.direction = 1;
            if (npc.direction == 1)
                npc.spriteDirection = 1;
            if (npc.direction == -1)
                npc.spriteDirection = -1;
            npc.rotation = (float)Math.Atan2((double)npc.velocity.Y * (double)npc.direction, (double)npc.velocity.X * (double)npc.direction);
            npc.frameCounter++;
            if (npc.frameCounter > 4)
            {
              npc.frame.Y += num1;
              npc.frameCounter = 0;
            }
            if (npc.frame.Y / num1 >= Main.npcFrameCount[npc.type])
              npc.frame.Y = 0;
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

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.02f, 0.7f, 0.7f);
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
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CryoSpirit"), 1f);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofEleum>(), 0.25f);
        }
    }
}
