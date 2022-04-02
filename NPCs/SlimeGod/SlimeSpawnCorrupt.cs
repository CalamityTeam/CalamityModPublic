using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    public class SlimeSpawnCorrupt : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrupt Slime Spawn");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 14;
            npc.GetNPCDamage();
            npc.width = 40;
            npc.height = 30;
            npc.defense = 6;
            npc.lifeMax = 180;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 10000;
            }
            npc.knockBackResist = 0f;
            animationType = 121;
            npc.alpha = 55;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override bool PreNPCLoot() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.life <= 0)
            {
                Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<SlimeSpawnCorrupt2>());
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Weak, 90, true);
        }
    }
}
