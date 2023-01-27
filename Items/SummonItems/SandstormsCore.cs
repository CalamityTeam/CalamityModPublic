using CalamityMod.Items.Materials;
using CalamityMod.NPCs.GreatSandShark;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class SandstormsCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Sandstorm's Core");
            Tooltip.SetDefault("Summons the Great Sand Shark when used in the desert\n" +
                "Not consumable");
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // Frost Legion
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
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
            return player.ZoneDesert && !(CalamityMod.Instance.legendaryMode && !player.Calamity().ZoneAstral) && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark>());
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<GreatSandShark>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<GreatSandShark>());

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            TooltipLine line0 = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if (CalamityMod.Instance.legendaryMode)
            {
                line0.Text = "Summons the Great Sand Shark when used in the astral desert";
            }
            else
            {
                line0.Text = "Summons the Great Sand Shark when used in the desert";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AncientBattleArmorMaterial, 3).
                AddIngredient<CoreofSunlight>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
