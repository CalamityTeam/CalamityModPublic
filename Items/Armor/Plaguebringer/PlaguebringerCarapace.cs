using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Plaguebringer
{
    [AutoloadEquip(EquipType.Body)]
    public class PlaguebringerCarapace : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
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
            Item.width = 18;
            Item.height = 18;
            Item.defense = 17;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().plaguebringerCarapace = true;
            player.GetDamage<SummonDamageClass>() += 0.15f;
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeeBreastplate).
                AddIngredient<AlchemicalFlask>(2).
                AddIngredient<PlagueCellCanister>(7).
                AddIngredient<InfectedArmorPlating>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
