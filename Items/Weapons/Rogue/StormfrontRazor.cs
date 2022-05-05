using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StormfrontRazor : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormfront Razor");
            Tooltip.SetDefault("Throws a throwing knife that leaves sparks as it travels.\n" +
                               "Stealth strikes cause the knife to be faster and leave a huge shower of sparks as it travels");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.damage = 50;
            Item.knockBack = 7f;
            Item.shoot = ModContent.ProjectileType<StormfrontRazorProjectile>();
            Item.shootSpeed = 7f;
            Item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 8;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity * 1.6f, ModContent.ProjectileType<StormfrontRazorProjectile>(), (int)(damage * 1.1f), knockback, player.whoAmI, 0, 10f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<StormfrontRazorProjectile>(), damage, knockback, player.whoAmI, 0, 1f);
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Cinquedea>().
                AddRecipeGroup("AnyMythrilBar", 6).
                AddIngredient<EssenceofCinder>(4).
                AddIngredient<SeaPrism>(15).
                AddIngredient<StormlionMandible>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
