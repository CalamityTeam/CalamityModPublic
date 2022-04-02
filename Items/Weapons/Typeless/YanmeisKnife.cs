using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class YanmeisKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yanmei's Knife");
            Tooltip.SetDefault("When hitting a boss, miniboss, or their minions, you gain various boosts and cripple the enemy hit\n" +
                "A knife from an unknown world\n" +
                "An owner whose heart is pure and free of taint\n" +
                "A heart of iron and valor");
        }

        public override void SetDefaults()
        {
            item.height = 44;
            item.width = 48;
            item.damage = 8;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = item.useTime = 32;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4.5f;
            item.autoReuse = false;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.Calamity().donorItem = true;
            item.UseSound = SoundID.Item71;
            item.shoot = ModContent.ProjectileType<YanmeisKnifeSlash>();
            item.shootSpeed = 24f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 6;

        public override bool CanUseItem(Player player)
        {
            if (player.Calamity().KameiBladeUseDelay > 0)
                return false;
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.Calamity().KameiBladeUseDelay = 180;
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PsychoKnife);
            recipe.AddIngredient(ItemID.Obsidian, 10);
            recipe.AddIngredient(ItemID.IronBar, 20);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 50);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
