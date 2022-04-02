using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AtaxiaMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Mask");
            Tooltip.SetDefault("12% increased magic damage, +100 max mana, and 10% increased magic critical strike chance\n" +
                "Temporary immunity to lava, and immunity to fire damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.defense = 9; //45
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AtaxiaArmor>() && legs.type == ModContent.ItemType<AtaxiaSubligar>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
            player.Calamity().hydrothermalSmoke = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased magic damage and 15% reduced mana usage\n" +
                "Inferno effect when below 50% life\n" +
                "Magic attacks summon damaging and healing flare orbs on hit\n" +
                "You emit a blazing explosion when you are hit";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ataxiaBlaze = true;
            modPlayer.ataxiaMage = true;
            player.magicDamage += 0.05f;
            player.manaCost *= 0.85f;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 100;
            player.magicDamage += 0.12f;
            player.magicCrit += 10;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 7);
            recipe.AddIngredient(ItemID.HellstoneBar, 4);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
