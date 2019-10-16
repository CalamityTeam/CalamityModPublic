using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class StatigelHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statigel Helm");
            Tooltip.SetDefault("10% increased melee damage and melee speed\n" +
                "7% increased melee critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = 4;
            item.defense = 9; //27
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StatigelArmor>() && legs.type == ModContent.ItemType<StatigelGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
                "Grants an extra jump and increased jump height";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.statigelSet = true;
            player.doubleJumpSail = true;
            player.jumpBoost = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.1f;
            player.meleeCrit += 7;
            player.meleeSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 5);
            recipe.AddIngredient(ItemID.HellstoneBar, 9);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
