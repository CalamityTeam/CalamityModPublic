using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class CrimsonFlask : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Crimson Flask");
            Tooltip.SetDefault("4% increased damage reduction and +6 defense while in the crimson\n" +
                "Grants immunity to the Burning Blood debuff");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<BurningBlood>()] = true;
            if (player.ZoneCrimson)
            {
                player.statDefense += 6;
                player.endurance += 0.04f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ViciousPowder, 15).
                AddIngredient(ItemID.Vertebrae, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
