using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TruePaladinsHammerMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fallen Paladin's Hammer");
            Tooltip.SetDefault("Explodes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.damage = 87;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 13;
            item.knockBack = 20f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 28;
            item.value = Item.buyPrice(gold: 80);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<FallenPaladinsHammerProj>();
            item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PaladinsHammer);
            recipe.AddIngredient(ModContent.ItemType<PwnagehammerMelee>());
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
