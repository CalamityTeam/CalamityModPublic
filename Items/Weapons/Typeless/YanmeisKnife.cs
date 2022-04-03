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
            Item.height = 44;
            Item.width = 48;
            Item.damage = 8;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4.5f;
            Item.autoReuse = false;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.UseSound = SoundID.Item71;
            Item.shoot = ModContent.ProjectileType<YanmeisKnifeSlash>();
            Item.shootSpeed = 24f;
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
            CreateRecipe(1).AddIngredient(ItemID.PsychoKnife).AddIngredient(ItemID.Obsidian, 10).AddIngredient(ItemID.IronBar, 20).AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 50).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
