using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StarofDestruction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star of Destruction");
            Tooltip.SetDefault("Fires a huge destructive mine that explodes into destruction bolts\n" +
            "Amount of bolts scales with enemies hit, up to 16\n" +
            "Stealth strikes always explode into the max amount of bolts");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 94;
            Item.damage = 150;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 38;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<DestructionStar>();
            Item.shootSpeed = 5f;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 0.8f), knockback, player.whoAmI, 0f, 1f);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldConstruct>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
