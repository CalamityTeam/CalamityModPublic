using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DaedalusHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Helm");
            Tooltip.SetDefault("10% increased melee damage and critical strike chance\n" +
                "10% increased melee and movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.defense = 21; //51
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DaedalusBreastplate>() && legs.type == ModContent.ItemType<DaedalusLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased melee damage\n" +
                "Enemies are more likely to target you\n" +
                "You reflect projectiles back at enemies\n" +
                "Reflected projectiles deal 50% less damage to you\n" +
                "This reflect has a 90 second cooldown which is shared with all other dodges and reflects\n" +
                "If you reflect a projectile you are also healed for 20% of that projectile's damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.daedalusReflect = true;
            player.meleeDamage += 0.05f;
            player.aggro += 500;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeSpeed += 0.1f;
            player.moveSpeed += 0.1f;
            player.meleeDamage += 0.1f;
            player.meleeCrit += 10;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 8);
            recipe.AddIngredient(ItemID.CrystalShard, 3);
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
