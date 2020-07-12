using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverMask : ModItem
    {
		//Damage and Crit Helm
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Mask");
            Tooltip.SetDefault("15% increased magic damage, 12% reduced mana cost, and 5% increased magic critical strike chance\n" +
                "10% increased movement speed, can move freely through liquids, and +80 max mana");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 22;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 7;
            item.defense = 7; //40
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.reaverBurst = true;
            player.setBonus = "5% increased magic damage\n" +
                "Your magic projectiles emit a burst of spore gas on enemy hits";
            player.magicDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.magicDamage += 0.15f;
            player.magicCrit += 5;
            player.manaCost *= 0.88f;
            player.moveSpeed += 0.1f;
            player.statManaMax2 += 80;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 8);
			recipe.AddIngredient(ItemID.JungleSpores, 6);
			recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
