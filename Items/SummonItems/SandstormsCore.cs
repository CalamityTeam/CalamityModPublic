using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.World;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class SandstormsCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
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
            return player.ZoneDesert && !(Main.zenithWorld && !player.Calamity().ZoneAstral) && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark>());
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<GreatSandShark>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<GreatSandShark>());

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[BIOME]", Main.zenithWorld ? CalamityUtils.GetTextValue("Biomes.AstralDesert") : Language.GetTextValue("Bestiary_Biomes.Desert"));

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
