using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class FeatherCrown : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {

            if (!Main.dedServ)
            {
                int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
                ArmorIDs.Face.Sets.OverrideHelmet[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 38;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<MoonstoneCrown>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.featherCrown = true;
            modPlayer.mageCrownVisibility = !hideVisual;
            player.GetDamage<MagicDamageClass>() += (0.02f * modPlayer.mageCrownCount); //2% per feather, up to 10%
            player.manaCost -= (0.01f * modPlayer.mageCrownCount); //1% per feather, up to 5%
            if (modPlayer.mageCrownCount >= 5) //At 5 feathers, grant 5% crit. Wind Chilled infliction is handed in CalPlayerOnHit
            {
                player.GetCritChance<MagicDamageClass>() += 5;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldCrown").
                AddIngredient<AerialiteBar>(6).
                AddIngredient(ItemID.Feather, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
