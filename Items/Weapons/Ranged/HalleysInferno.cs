using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HalleysInferno : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halley's Inferno");
            Tooltip.SetDefault("Halley came sooner than expected\n" +
            "Fires a flaming comet\n" +
            "50% chance to not consume gel");
        }

        public override void SetDefaults()
        {
            item.damage = 1750;
            item.crit += 20;
            item.ranged = true;
            item.width = 84;
            item.height = 34;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<HalleysComet>();
            item.shootSpeed = 14.6f;
            item.useAmmo = 23;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 4);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12);
            recipe.AddIngredient(ItemID.SniperScope);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
