using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TruePaladinsHammer : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fallen Paladin's Hammer");
            Tooltip.SetDefault("Explodes on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 160;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 13;
            item.useStyle = 1;
            item.useTime = 13;
            item.knockBack = 20f;
            item.UseSound = SoundID.Item1;
            item.height = 28;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<OPHammer>();
            item.shootSpeed = 14f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PaladinsHammer);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
