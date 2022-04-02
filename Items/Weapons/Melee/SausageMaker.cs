using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SausageMaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sausage Maker");
            Tooltip.SetDefault("Sprays homing blood on hit");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.damage = 36;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 20;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 42;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<SausageMakerSpear>();
            item.shootSpeed = 6f;
            item.Calamity().trueMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodSample>(), 8);
            recipe.AddIngredient(ItemID.Vertebrae, 4);
            recipe.AddIngredient(ItemID.CrimtaneBar, 5);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
