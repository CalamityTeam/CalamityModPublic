using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Daedalus
{
    [AutoloadEquip(EquipType.Body)]
    public class DaedalusBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static float DamageBoost = 0.08f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent());

        public override void Load()
        {
            if (Main.dedServ)
                return;

            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Daedalus/DaedalusBreastplate_Waist", EquipType.Waist, this);
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 19;
        }

        public override void UpdateEquip(Player player) => player.GetDamage<GenericDamageClass>() += DamageBoost;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
