using CalamityMod.Events;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    public class SlimeSpawnCorrupt2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrupt Slime Spawn");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 1;
            npc.damage = 30;
            npc.width = 40;
            npc.height = 30;
            npc.defense = 4;
            npc.lifeMax = 90;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 50000;
            }
            npc.knockBackResist = 0f;
            animationType = NPCID.CorruptSlime;
            npc.alpha = 55;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[BuffID.OnFire] = true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ItemID.Vitamins, Main.expertMode ? 50 : 100);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Weak, 90, true);
        }
    }
}
