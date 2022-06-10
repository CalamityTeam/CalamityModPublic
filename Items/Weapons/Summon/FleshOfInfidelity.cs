using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Summon
{
    public class FleshOfInfidelity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flesh of Infidelity");
            Tooltip.SetDefault("Summons a tentacled ball of flesh that splashes blood onto enemies");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.mana = 10;
            Item.width = Item.height = 48;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Zombie24;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FleshBallMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.UnitY * -1f, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BelladonnaSpiritStaff>().
                AddIngredient<StaffOfNecrosteocytes>().
                AddIngredient<ScabRipper>().
                AddIngredient(ItemID.ImpStaff).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
