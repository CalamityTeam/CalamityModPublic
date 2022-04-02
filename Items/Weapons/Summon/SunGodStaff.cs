using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SunGodStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun God Staff");
            Tooltip.SetDefault("Summons a solar god spirit to protect you\n" +
                "There can only be one spirit");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.mana = 10;
            item.width = 72;
            item.height = 72;
            item.useTime = item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 1.25f;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = ItemRarityID.LightPurple;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<SolarGod>();
            item.summon = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SunSpiritStaff>());
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 5);
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddTile(TileID.MythrilAnvil);
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
