using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SunSpiritStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun Spirit Staff");
            Tooltip.SetDefault("Summons a solar spirit to protect you\n" +
				"There can only be one spirit");
        }

        public override void SetDefaults()
        {
            item.damage = 12;
            item.mana = 10;
            item.width = 44;
            item.height = 48;
            item.useTime = item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 1.15f;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<SolarPixie>();
            item.summon = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SandstoneBrick, 20);
            recipe.AddIngredient(ModContent.ItemType<DesertFeather>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			CalamityUtils.KillShootProjectiles(true, type, player);
            Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
