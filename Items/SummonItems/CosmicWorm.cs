using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class CosmicWorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Worm");
            Tooltip.SetDefault("Summons the Devourer of Gods\n" +
                "SENTINEL WARNING TOOLTIP LINE HERE\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    bool sentinelsNotDefeated = !CalamityWorld.downedSentinel1 || !CalamityWorld.downedSentinel2 || !CalamityWorld.downedSentinel3;
                    line2.text = sentinelsNotDefeated ? "WARNING! Some sentinels have not been truly defeated yet and will spawn at full power during this fight!" : "";
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) && CalamityWorld.DoGSecondStageCountdown <= 0 && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            string key = "Mods.CalamityMod.EdgyBossText12";
            Color messageColor = Color.Cyan;
            CalamityUtils.DisplayLocalizedText(key, messageColor);

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerSpawn"), (int)player.position.X, (int)player.position.Y);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DevourerofGodsHead>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<DevourerofGodsHead>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 3);
            recipe.AddIngredient(ModContent.ItemType<TwistingNether>());
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 30);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 20);
            recipe.AddIngredient(ItemID.SoulofLight, 20);
            recipe.AddIngredient(ItemID.SoulofNight, 20);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 30);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
