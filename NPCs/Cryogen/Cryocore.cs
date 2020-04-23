using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Cryogen
{
    public class Cryocore : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryocore");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.damage = 35;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 6;
            npc.lifeMax = 220;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 30000;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = CalamityWorld.bossRushActive ? 0f : 0.75f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath15;
			npc.coldDamage = true;
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
            bool revenge = CalamityWorld.revenge;
            float speed = revenge ? 12f : 11f;
            if (CalamityWorld.bossRushActive)
                speed = 24f;
            CalamityAI.CryocoreAI(npc, mod, speed, true);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 90, true);
            player.AddBuff(BuffID.Chilled, 60, true);
        }

        public override bool PreNPCLoot()
        {
            return false;
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
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Cryocore"), 1f);
            }
        }
    }
}
