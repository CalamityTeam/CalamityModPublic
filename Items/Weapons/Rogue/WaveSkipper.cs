using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{   
    [LegacyName("DuneHopper")]
    public class WaveSkipper : RogueWeapon
    {
        public static int SpreadAngle = 8;
        
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.damage = 80;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 44;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<WaveSkipperProjectile>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = -SpreadAngle; i < SpreadAngle * 2; i += SpreadAngle)
                {
                    Vector2 spreadVelocity = player.SafeDirectionTo(Main.MouseWorld).RotatedBy(MathHelper.ToRadians(i)) * Item.shootSpeed;
                    int stealth = Projectile.NewProjectile(source, position, spreadVelocity, ModContent.ProjectileType<WaveSkipperProjectile>(), damage, knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScourgeoftheDesert>().
                AddIngredient<MolluskHusk>(5).
                AddIngredient<SeaPrism>(15).
                AddIngredient<PrismShard>(20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
