using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SigilofCalamitas : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 8));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
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
            player.manaMagnet = true;
            player.statManaMax2 += 100;
            player.GetDamage<MagicDamageClass>() += 0.15f;
            player.manaCost *= 0.9f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CelestialEmblem).
                AddIngredient<ScoriaBar>(5).
                AddIngredient<AshesofCalamity>(15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
