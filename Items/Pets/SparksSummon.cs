using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class SparksSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Butterfly");
            Tooltip.SetDefault("Feed him butterflies to keep him strong!\n" +
                "Summons a mysterious dragonfly light pet\n" +
                "Provides a small amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.shoot = ModContent.ProjectileType<Sparks>();
            item.buffType = ModContent.BuffType<SparksBuff>();
            item.value = Item.sellPrice(gold: 1);
            item.rare = 5;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldButterfly);
            recipe.AddIngredient(ItemID.MonarchButterfly);
            recipe.AddIngredient(ItemID.PurpleEmperorButterfly);
            recipe.AddIngredient(ItemID.RedAdmiralButterfly);
            recipe.AddIngredient(ItemID.UlyssesButterfly);
            recipe.AddIngredient(ItemID.SulphurButterfly);
            recipe.AddIngredient(ItemID.TreeNymphButterfly);
            recipe.AddIngredient(ItemID.ZebraSwallowtailButterfly);
            recipe.AddIngredient(ItemID.JuliaButterfly);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
