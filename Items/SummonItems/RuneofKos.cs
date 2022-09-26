using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.Rarities;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("RuneofCos")]
    public class RuneofKos : ModItem
    {
        public static readonly SoundStyle CVSound = new("CalamityMod/Sounds/Item/CeaselessVoidSpawn");
        public static readonly SoundStyle SignutSound = new("CalamityMod/Sounds/Item/SignusSpawn");
        public static readonly SoundStyle StormSound = new("CalamityMod/Sounds/Item/StormWeaverSpawn");
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Rune of Kos");
            Tooltip.SetDefault("A relic of the profaned flame\n" +
                "Contains the power hunted relentlessly by the sentinels of the cosmic devourer\n" +
                "When used in certain areas of the world, it will unleash them\n" +
                "Not consumable");
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 17; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = null;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}

        public override bool CanUseItem(Player player)
        {
            return (player.ZoneSkyHeight || player.ZoneUnderworldHeight || player.ZoneDungeon) &&
                !NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()) && !NPC.AnyNPCs(ModContent.NPCType<CeaselessVoid>()) && !NPC.AnyNPCs(ModContent.NPCType<Signus>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            if (player.ZoneDungeon)
            {
                SoundEngine.PlaySound(CVSound, player.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CeaselessVoid>());
                else
                    NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<CeaselessVoid>());
            }
            else if (player.ZoneUnderworldHeight)
            {
                SoundEngine.PlaySound(SignutSound, player.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Signus>());
                else
                    NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Signus>());
            }
            else if (player.ZoneSkyHeight)
            {
                SoundEngine.PlaySound(StormSound, player.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<StormWeaverHead>());
                else
                    NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<StormWeaverHead>());
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null) {
                if (player.ZoneDungeon)
                {
                    line.Text = "Summons the Ceaseless Void" +
                        "\nEnrages on the surface";
                }
                else if (player.ZoneSkyHeight)
                    line.Text = "Summons the Storm Weaver";
                else if (player.ZoneUnderworldHeight)
                    line.Text = "Summons Signus, Envoy of the Devourer";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UnholyEssence>(40).
                AddIngredient(ItemID.FragmentSolar, 5).
                AddIngredient(ItemID.LunarBar, 3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
