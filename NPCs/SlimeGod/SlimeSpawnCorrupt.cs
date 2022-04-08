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
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 14;
            NPC.GetNPCDamage();
            NPC.width = 40;
            NPC.height = 30;
            NPC.defense = 6;
            NPC.lifeMax = 180;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 10000;
            }
            NPC.knockBackResist = 0f;
            AnimationType = 121;
            NPC.alpha = 55;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.life <= 0)
            {
                Vector2 spawnAt = NPC.Center + new Vector2(0f, (float)NPC.height / 2f);
                NPC.NewNPC(NPC.GetItemSource_Loot(), (int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<SlimeSpawnCorrupt2>());
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Weak, 90, true);
        }
    }
}
