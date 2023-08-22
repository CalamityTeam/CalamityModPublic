using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class LivingDew : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //20% Movespeed and Jumpspeed, 12% reduced damage
            player.moveSpeed += 0.2f;
            player.jumpSpeedBoost += 1f;
            player.GetDamage<GenericDamageClass>() -= 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleSpores, 12).
                AddIngredient<MurkyPaste>(5).
                AddIngredient<TrapperBulb>(3).
                //replace with water essence later
                AddIngredient<EssenceofSunlight>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
