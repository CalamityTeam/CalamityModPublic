using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeSlimeGod : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Slime God");
            Tooltip.SetDefault("It is a travesty, one of the most threatening biological terrors ever created.\n" +
                "If this creature were allowed to combine every slime on the planet it would become nearly unstoppable.\n" +
                "Favorite this item to become slimed and able to slide around on tiles quickly, at the cost of reduced defense.\n" +
                "This effect makes dashing more difficult and does not work with mounts.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 4;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.mount.Active || !item.favorited || modPlayer.slimeGodLoreProcessed)
                return;

            modPlayer.slimeGodLoreProcessed = true;

            if (player.dashDelay < 0 || (player.velocity.Length() >= 11f && CalamityPlayer.areThereAnyDamnBosses)) //If you go over 52.8 mph
                player.velocity.X *= 0.9f;

            player.slippy2 = true;

            if (Main.myPlayer == player.whoAmI)
                player.AddBuff(BuffID.Slimed, 2);

            player.statDefense -= 10;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<SlimeGodTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
