using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Summon
{
    public class ViridVanguard : ModItem
    {
        public const float MaxTargetingDistance = 1550f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Virid Vanguard");
            Tooltip.SetDefault("Update this");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 210;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 36;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ViridVanguardBlade>();
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (Main.projectile.IndexInRange(p))
                {
                    Main.projectile[p].originalDamage = Item.damage;
                    Main.projectile[p].ModProjectile<ViridVanguardBlade>().BladeIndex = player.ownedProjectileCounts[type];
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type == type && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].ModProjectile<ViridVanguardBlade>().AITimer = 0f;
                        Main.projectile[i].netUpdate = true;
                    }
                }
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EmpressBlade).
                AddIngredient<IgneousExaltation>().
                AddIngredient<UelibloomBar>(8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
