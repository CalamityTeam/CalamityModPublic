using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Summon
{
	public class EyeOfNight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Night");
            Tooltip.SetDefault("Summons a diseased eyeball that fires cells which attach to enemies and inflict cursed flames");
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.mana = 10;
            item.width = item.height = 36;
            item.useTime = item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;

            item.UseSound = SoundID.NPCHit1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<EyeOfNightSummon>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
                Projectile.NewProjectile(Main.MouseWorld, Vector2.UnitY * -3f, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BelladonnaSpiritStaff>());
            recipe.AddIngredient(ModContent.ItemType<StaffOfNecrosteocytes>());
            recipe.AddIngredient(ModContent.ItemType<VileFeeder>());
            recipe.AddIngredient(ItemID.ImpStaff);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
