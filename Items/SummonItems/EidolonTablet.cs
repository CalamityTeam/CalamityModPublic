using System;
using CalamityMod.Events;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class EidolonTablet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[NPCID.CultistBoss] = true;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 18; // Bloody Tear (1 below Celestial Sigil fsr)
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            // This is quite ugly, but MP does not sync NPC.LunarApocalypseIsUp
            // So we should check it ourselves
            if (BossRushEvent.BossRushActive)
                return false;

            if (NPC.MoonLordCountdown > 0)
                return false;

            foreach (var npc in Main.ActiveNPCs)
            {
                switch (npc.type)
                {
                    case NPCID.CultistTablet:
                    case NPCID.CultistBoss:
                    case NPCID.LunarTowerSolar:
                    case NPCID.LunarTowerVortex:
                    case NPCID.LunarTowerNebula:
                    case NPCID.LunarTowerStardust:
                    case NPCID.MoonLordCore:
                        return false;
                }
            }

            return true;
        }

        public override bool? UseItem(Player player)
        {
            int posX = (int)player.Center.X + 30;
            int posY = (int)player.Center.Y - 90;
            NPC npc = CalamityUtils.SpawnBossOnPosUsingItem(player, NPCID.CultistBoss, posX, posY);
            if (npc != null)
            {
                WorldGen.GetRidOfCultists();

                int newDir = Math.Sign(player.Center.X - player.Center.X - 30f);
                npc.direction = newDir;
                npc.spriteDirection = newDir;
            }
            return true;
        }
    }
}
