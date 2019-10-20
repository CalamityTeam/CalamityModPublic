using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using CalamityMod.NPCs;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.SummonItems
{
    public class CosmicWorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Worm");
            Tooltip.SetDefault("Summons the Devourer of Gods\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = false;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) && !NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHeadS>()) && CalamityWorld.DoGSecondStageCountdown <= 0 && CalamityWorld.downedBossAny;
        }

        public override bool UseItem(Player player)
        {
            string key = "Mods.CalamityMod.EdgyBossText12";
            Color messageColor = Color.Cyan;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(key), messageColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DevourerofGodsHead>());
            Main.PlaySound(SoundID.Roar, player.position, 0);
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
        }
    }
}
