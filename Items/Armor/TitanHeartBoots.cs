using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class TitanHeartBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Heart Boots");
            Tooltip.SetDefault("4% increased rogue damage, 10% increased rogue velocity, and 5% increased rogue knockback");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 14;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().titanHeartBoots = true;
            player.Calamity().throwingVelocity += 0.1f;
            player.Calamity().throwingDamage += 0.04f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralMonolith>(14).
                AddIngredient<TitanHeart>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
