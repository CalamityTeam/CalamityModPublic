using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class CyanSeekingMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Cyan Seeking Mechanism");
            Tooltip.SetDefault("Moves swiftly towards a lab deep below the desert");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 36;
            Item.shoot = ModContent.ProjectileType<CyanLabSeeker>();
            Item.Calamity().MaxCharge = 100;
            Item.Calamity().ChargePerUse = 15;
            Item.Calamity().UsesCharge = true;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && CalamityWorld.SunkenSeaLabCenter != Vector2.Zero;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LabSeekingMechanism>().
                AddIngredient(ItemID.WaterBucket).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
