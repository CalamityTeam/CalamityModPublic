using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class PurpleSeekingMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Seeking Mechanism");
            Tooltip.SetDefault("Moves swiftly towards a lab within the sky");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 26;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = item.useAnimation = 36;
            item.shoot = ModContent.ProjectileType<PurpleLabSeeker>();
            item.Calamity().MaxCharge = 100;
            item.Calamity().ChargePerUse = 15;
            item.Calamity().UsesCharge = true;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0 && CalamityWorld.PlanetoidLabCenter != Vector2.Zero;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousMechanism>());
            recipe.AddIngredient(ItemID.Cloud, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
