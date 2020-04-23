using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Cryogen
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
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 40000;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = CalamityWorld.bossRushActive ? 0f : 0.5f;
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
            float speed = revenge ? 14f : 12f;
            if (CalamityWorld.bossRushActive)
                speed = 28f;
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
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Cryocore2"), 1f);
            }
        }
    }
}
