using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BrimstoneFury : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Fury");
            Tooltip.SetDefault("Converts wooden arrows into spreads of 3 brimstone bolts");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 58;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.75f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneBolt>();
            Item.shootSpeed = 13f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numProj = 3;
            float rotation = MathHelper.ToRadians(2);
            for (int i = 0; i < numProj; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));

                if (type == ProjectileID.WoodenArrowFriendly)
                    Projectile.NewProjectile(source, position, perturbedSpeed, ModContent.ProjectileType<BrimstoneBolt>(), damage, knockback, player.whoAmI);
                else
                {
                    int proj = Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UnholyCore>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
