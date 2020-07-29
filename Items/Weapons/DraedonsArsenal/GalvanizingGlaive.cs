using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GalvanizingGlaive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galvanizing Glaive");
            Tooltip.SetDefault("Swings a spear which envelops struck foes in an energy field\n" + "When done swinging, the spear discharges an extra pulse of energy");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 52;
            item.damage = 84;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.melee = true;
            item.useAnimation = 21;
            item.useTime = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.rare = 8;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.shoot = ModContent.ProjectileType<GalvanizingGlaiveProjectile>();
            item.shootSpeed = 21f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float swingOffset = Main.rand.NextFloat(0.5f, 1f) * item.shootSpeed * 1.6f * player.direction;
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, swingOffset);
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
