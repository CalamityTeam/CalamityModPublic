using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class Phaseslayer : ModItem
    {
        public const int MinDamage = 4500;
        public const int MaxDamage = 12500;
        public const float SmallPhaseDamageMultiplier = 0.66f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phaseslayer");
            Tooltip.SetDefault("Creates a blade that aims towards the cursor. The faster you swing the blade, the more damage it does.\n" +
                               "Swinging quickly causes the blade to release energy forward");
        }
        public override void SetDefaults()
        {
            item.damage = MinDamage;
            item.melee = true;
            item.width = 26;
            item.height = 26;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTurn = false;
            item.knockBack = 7f;

            item.noUseGraphic = true;

            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.RareVariant;

            item.UseSound = SoundID.Item1;
            item.autoReuse = true;

            item.Calamity().Chargeable = true;
            item.Calamity().ChargeMax = 250;

            item.shoot = ModContent.ProjectileType<PhaseslayerProjectile>();

            item.channel = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile blade = Projectile.NewProjectileDirect(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            blade.rotation = blade.AngleTo(Main.MouseWorld);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddIngredient(ItemID.RedPhasesaber);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
