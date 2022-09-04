using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Empyrean
{
    [AutoloadEquip(EquipType.Body)]
    [LegacyName("XerocPlateMail")]
    public class EmpyreanCloak : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/EmpyreanCloak_Neck", EquipType.Neck, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/EmpyreanCloak_Back", EquipType.Back, this);
            }
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Empyrean Cloak");
            Tooltip.SetDefault("Armor of the cosmos\n" +
                "+20 max life\n" +
                "7% increased rogue damage and critical strike chance");

            if (Main.netMode != NetmodeID.Server)
            {
                var equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
                ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.defense = 27;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.GetCritChance<ThrowingDamageClass>() += 7;
            player.GetDamage<ThrowingDamageClass>() += 0.07f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldConstruct>(22).
                AddIngredient(ItemID.LunarBar, 16).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
