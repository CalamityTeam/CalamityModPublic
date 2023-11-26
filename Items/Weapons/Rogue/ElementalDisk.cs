using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    [LegacyName("AccretionDisk")]
    public class ElementalDisk : RogueWeapon
    {
        public static int stealthTimeMult = 2;
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.damage = 100;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 16;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<ElementalDiskProj>();
            Item.shootSpeed = 15f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthVelocityMultiplier => 0.8f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    Main.projectile[proj].timeLeft *= stealthTimeMult;
                    Main.projectile[proj].localNPCHitCooldown *= 2;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TerraDisk>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
