using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Sandslasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandslasher");
            Tooltip.SetDefault("Throws a huge shuriken made out of fused sand unaffected by gravity which slowly accelerates horizontally\n"
                              +"It does more damage depending on how fast it goes horizontally and how long it has been flying for\n"
                              +"Stealth strikes periodically release sand clouds");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 115;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<SandslasherProj>();
            Item.shootSpeed = 7f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GrandScale>().
                AddIngredient<CoreofCinder>(6).
                AddRecipeGroup("AnyGoldBar", 10).
                AddIngredient(ItemID.HardenedSand, 25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
