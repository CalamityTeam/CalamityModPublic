using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    public class AbyssalDivingSuit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Diving Suit");
            Tooltip.SetDefault("Transforms the holder into an armored diver\n" +
                "Increases movement speed while underwater and moves slowly outside of water\n" +
                "The suits' armored plates reduce damage taken by 15%\n" +
                "The plates will only take damage if the damage taken is over 50\n" +
                "After the suit has taken too much damage its armored plates will take 3 minutes to regenerate\n" +
                "Reduces the damage caused by the pressure of the abyss while out of breath\n" +
                "Removes the bleed effect caused by the abyss in all layers except the deepest one\n" +
                "Grants the ability to swim and greatly extends underwater breathing\n" +
                "Provides light underwater and extra mobility on ice\n" +
                "Provides a moderate amount of light in the abyss\n" +
                "Greatly reduces breath loss in the abyss\n" +
                "Reduces creature's ability to detect you in the abyss\n" +
                "Reduces the defense reduction that the abyss causes\n" +
                "Allows you to fall faster while in liquids");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.accessory = true;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.rare = 10;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>();
            modPlayer.abyssalDivingSuit = true;
            if (hideVisual)
            {
                modPlayer.abyssalDivingSuitHide = true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AbyssalDivingGear");
            recipe.AddIngredient(null, "AnechoicPlating");
            recipe.AddIngredient(null, "IronBoots");
            recipe.AddIngredient(null, "Lumenite", 40);
            recipe.AddIngredient(null, "DepthCells", 40);
            recipe.AddIngredient(null, "Tenebris", 15);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class AbyssalDivingSuitHead : EquipTexture
    {
        public override bool DrawHead()
        {
            return false;
        }
    }

    public class AbyssalDivingSuitBody : EquipTexture
    {
        public override bool DrawBody()
        {
            return false;
        }
    }

    public class AbyssalDivingSuitLegs : EquipTexture
    {
        public override bool DrawLegs()
        {
            return false;
        }
    }
}
