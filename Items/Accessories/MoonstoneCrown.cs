using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class MoonstoneCrown : ModItem, ILocalizedModType, IHoldShiftTooltipItem
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
            Item.width = 46;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<FeatherCrown>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.moonCrown = true;
            modPlayer.mageCrownVisibility = !hideVisual;
            player.statManaMax2 += 40;
            player.GetDamage<MagicDamageClass>() += (0.02f * modPlayer.mageCrownCount); //2% per moon sigil, up to 20%
            player.manaCost -= (0.01f * modPlayer.mageCrownCount); //1% per moon sigil, up to 10%
            if (modPlayer.mageCrownCount >= 5) //At 5 sigils, grant a mana regen bonus
            {
                player.manaRegenBonus += 10;
            }
            if (modPlayer.mageCrownCount >= 10) //At 10 sigils, grant 10% crit. Nightwither infliction is handed in CalPlayerOnHit
            {
                player.GetCritChance<MagicDamageClass>() += 10;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FeatherCrown>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient(ItemID.FragmentNebula, 5).
                AddIngredient<CoreofCalamity>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
