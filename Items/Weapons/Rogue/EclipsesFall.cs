using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class EclipsesFall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Fall");
            Tooltip.SetDefault("When the sun goes dark, you will know judgment\n" +
            "Summons spears from the sky on hit\n" +
            "Stealth strikes impale enemies and summon a constant barrage of spears over time");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 564;
            Item.knockBack = 3.5f;
            Item.useAnimation = Item.useTime = 21;
            Item.autoReuse = true;
            Item.DamageType = RogueDamageClass.Instance;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<EclipsesFallMain>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = Item.height = 72;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
                type = ModContent.ProjectileType<EclipsesStealth>();

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NightsGaze>().
                AddIngredient<CoreofCinder>(12).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<DarksunFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
