using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BlazingStar : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = RogueDamageClass.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;

            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<BlazingStarProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;

		public override float StealthDamageMultiplier => 0.575f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-MathHelper.ToRadians(8f), MathHelper.ToRadians(8f), i / 2f));
                    Projectile proj = Projectile.NewProjectileDirect(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
                    if (proj.whoAmI.WithinBounds(Main.maxProjectiles))
                        proj.Calamity().stealthStrike = true;

                    Projectile projectile = Projectile.NewProjectileDirect(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
                    if (projectile.whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        projectile.penetrate = -1;
                        projectile.Calamity().stealthStrike = true;
                    }
                }
                return false;
            }
            return true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 3;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Glaive>().
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient<EssenceofHavoc>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
