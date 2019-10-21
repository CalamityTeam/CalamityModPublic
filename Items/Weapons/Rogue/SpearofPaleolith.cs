using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SpearofPaleolith : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear of Paleolith");
            Tooltip.SetDefault("Throws an ancient spear that shatters enemy armor\n" +
                "Spears rain fossil shards as they travel");
        }

        public override void SafeSetDefaults()
        {
            item.width = 54;
            item.damage = 65;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 27;
            item.useStyle = 1;
            item.useTime = 27;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 54;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<SpearofPaleolithProj>();
            item.shootSpeed = 35f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.AdamantiteBar, 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.TitaniumBar, 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
