using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class XerocPlateMail : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/XerocPlateMail_Neck", EquipType.Neck, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/XerocPlateMail_Back", EquipType.Back, this);
            }
        }

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Empyrean Cloak");
            Tooltip.SetDefault("Armor of the cosmos\n" +
                "+20 max life\n" +
                "7% increased rogue damage and critical strike chance");

            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
                ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 32, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 27;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.Calamity().throwingCrit += 7;
            player.Calamity().throwingDamage += 0.07f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldiateBar>(22).
                AddIngredient(ItemID.LunarBar, 16).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
