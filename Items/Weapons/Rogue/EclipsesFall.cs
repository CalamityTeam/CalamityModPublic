using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class EclipsesFall : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.knockBack = 3.5f;
            Item.useAnimation = Item.useTime = 22;
            Item.autoReuse = true;
            Item.DamageType = RogueDamageClass.Instance;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<EclipsesFallMain>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 82;
            Item.height = 88;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override float StealthDamageMultiplier => 1.05f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
                type = ModContent.ProjectileType<EclipsesStealth>();

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>(Texture + "Glow").Value);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NightsGaze>().
                AddIngredient<CoreofSunlight>(12).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<DarksunFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
