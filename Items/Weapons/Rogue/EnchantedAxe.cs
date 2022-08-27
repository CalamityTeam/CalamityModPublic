using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class EnchantedAxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Axe");
            Tooltip.SetDefault("Throws a high velocity axe that returns to you after travelling a short distance or hitting a wall\n" +
                               "At the furthest point from the player, a magical axe that travels through walls will be cast towards the nearest enemy\n" +
                               "Stealth strikes make the axe fly further, throwing out many other magical axes in all directions");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.damage = 20;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 36;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.shoot = ModContent.ProjectileType<EnchantedAxeProj>();
            Item.shootSpeed = 30f;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<IronFrancisca>(100).
                AddIngredient(ItemID.FallenStar, 5).
                AddIngredient<PearlShard>(10).
                AddIngredient(ItemID.Bone, 30).
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe().
                AddIngredient<LeadTomahawk>(100).
                AddIngredient(ItemID.FallenStar, 5).
                AddIngredient<PearlShard>(10).
                AddIngredient(ItemID.Bone, 30).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
