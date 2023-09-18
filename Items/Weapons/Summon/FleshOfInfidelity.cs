using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class FleshOfInfidelity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<FleshBallMinion>();
            Item.knockBack = 1f;
            
            Item.useTime = Item.useAnimation = 10;
            Item.mana = 10;
            Item.width = 42;
            Item.height = 42;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Zombie24;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.UnitY * -1f, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrimtaneBar, 4).
                AddIngredient<BloodSample>(12).
                AddIngredient(ItemID.Vertebrae, 4).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
