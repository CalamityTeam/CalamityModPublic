using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Reaver
{
    [AutoloadEquip(EquipType.Body)]
    public class ReaverScaleMail : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";
        public override void SetStaticDefaults()
        {
           
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 19;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.GetDamage<GenericDamageClass>() += 0.09f;
            player.GetCritChance<GenericDamageClass>() += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(15).
                AddIngredient<LivingShard>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
