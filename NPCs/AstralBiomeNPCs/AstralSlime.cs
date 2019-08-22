
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using CalamityMod.World;

namespace CalamityMod.NPCs.AstralBiomeNPCs
{
    public class AstralSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Slime");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.damage = 40;
            npc.width = 44;
            npc.height = 28;
            npc.aiStyle = 1;
            npc.defense = 8;
            npc.lifeMax = 200;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.alpha = 60;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            animationType = NPCID.BlueSlime;
			banner = npc.type;
			bannerItem = mod.ItemType("AstralSlimeBanner");
			if (CalamityWorld.downedAstrageldon)
			{
				npc.damage = 65;
				npc.defense = 18;
				npc.lifeMax = 310;
			}
		}

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 44, frameHeight, mod.DustType("AstralOrange"), new Rectangle(4, 4, 36, 24), Vector2.Zero, 0.15f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            CalamityGlobalNPC.DoHitDust(npc, hitDirection, mod.DustType("AstralOrange"), 1f, 4, 24);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && !spawnInfo.player.ZoneRockLayerHeight)
            {
                return 0.21f;
            }
            return 0f;
        }

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("AstralInfectionDebuff"), 120, true);
		}

		public override void NPCLoot()
		{
			int oreMin = Main.expertMode ? 11 : 8;
			int oreMax = Main.expertMode ? 16 : 12;
			DropHelper.DropItemCondition(npc, mod.ItemType("AstralOre"), CalamityWorld.downedStarGod, oreMin, oreMax);
		}
	}
}
