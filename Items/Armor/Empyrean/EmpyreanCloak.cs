using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Empyrean
{
    [AutoloadEquip(EquipType.Body)]
    [LegacyName("XerocPlateMail")]
    public class EmpyreanCloak : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float RogueDamageBoost = 0.12f;
        public static int RogueCritBoost = 8;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RogueDamageBoost.ToPercent(), RogueCritBoost);

        public override void Load()
        {
            if (!Main.dedServ)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/EmpyreanCloak_Neck", EquipType.Neck, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/EmpyreanCloak_Back", EquipType.Back, this);
            }
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
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
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.defense = 24;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += RogueDamageBoost;
            player.GetCritChance<ThrowingDamageClass>() += RogueCritBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldBlob>(20).
                AddIngredient(ItemID.LunarBar, 16).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
