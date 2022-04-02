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
            npc.GetNPCDamage();
            npc.width = 40;
            npc.height = 30;
            npc.defense = 4;
            npc.lifeMax = 90;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 5000;
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
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
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

        public override bool PreNPCLoot()
        {
            if (!CalamityWorld.revenge)
            {
                int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
                if (Main.rand.Next(8) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
            }

            DropHelper.DropItemChance(npc, ItemID.Vitamins, Main.expertMode ? 50 : 100);

            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Weak, 90, true);
        }
    }
}
