using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AncientCrusher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Crusher");
            Tooltip.SetDefault("Summons fossil spikes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 62;
			item.height = 62;
			item.scale = 2f;
			item.damage = 55;
            item.melee = true;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 30;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Amber, 8);
            recipe.AddIngredient(ItemID.FossilOre, 35);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
			if (crit)
				damage /= 2;

			Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<FossilSpike>(), damage, knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
			if (crit)
				damage /= 2;

			Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<FossilSpike>(), damage, item.knockBack, Main.myPlayer);
        }
    }
}
