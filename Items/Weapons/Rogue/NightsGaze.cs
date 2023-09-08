using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class NightsGaze : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 82;
            Item.damage = 531;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 82;
            Item.maxStack = 1;
            Item.shoot = ModContent.ProjectileType<NightsGazeProjectile>();
            Item.shootSpeed = 30f;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override float StealthDamageMultiplier => 1.22f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/NightsGazeGlow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ProfanedPartisan>().
                AddIngredient<Lumenyl>(7).
                AddIngredient<RuinousSoul>(4).
                AddIngredient<ExodiumCluster>(12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
