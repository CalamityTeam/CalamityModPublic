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
            "50% chance to not consume gel\n" +
            "Right click to zoom out");
        }

        public override void SetDefaults()
        {
            item.damage = 1350;
            item.knockBack = 5f;
            item.ranged = true;
            item.useTime = item.useAnimation = 30;
            item.autoReuse = true;
            item.useAmmo = AmmoID.Gel;
            item.shootSpeed = 14.6f;
            item.shoot = ModContent.ProjectileType<HalleysComet>();

            item.width = 84;
            item.height = 34;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(100) >= 50;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SniperScope);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 6);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 4);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
