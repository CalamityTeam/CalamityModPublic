using CalamityMod.CalPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RaidersTalisman : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static int RaiderBonus => 15;
        public static int RaiderCooldown => 10; //seconds
        public static readonly SoundStyle StealthHitSound = new("CalamityMod/Sounds/Custom/RaidersTalismanStealthHit");

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.raiderTalisman = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Leather, 5).
                AddIngredient(ItemID.Obsidian, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
