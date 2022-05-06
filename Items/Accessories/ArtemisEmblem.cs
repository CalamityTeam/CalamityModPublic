using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class ArtemisEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Artemis Emblem");
            Tooltip.SetDefault("10% increased ranged damage, 5% increased ranged critical strike chance and 20% reduced ammo usage\n" +
                "Greatly increases all ranged projectile velocity");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().artemisEmblem = true;
            player.Calamity().rangedAmmoCost *= 0.8f;
            player.GetDamage(DamageClass.Ranged) += 0.1f;
            player.GetCritChance(DamageClass.Ranged) += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RangerEmblem).
                AddIngredient<CoreofCalamity>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
