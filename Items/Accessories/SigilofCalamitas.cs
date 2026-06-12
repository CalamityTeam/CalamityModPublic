using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SigilofCalamitas : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int MaxManaBoost = 60;
        public static float MagicDamageBoost = 0.15f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaBoost, MagicDamageBoost.ToPercent());

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 8));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaMagnet = true;
            player.statManaMax2 += MaxManaBoost;
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
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
