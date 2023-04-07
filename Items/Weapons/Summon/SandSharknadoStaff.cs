using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SandSharknadoStaff : ModItem
    {
        #region Other stats for easy modifcation

        public const float ProjVel = 30f;

        public const float FireSpeed = 120f; // In frames. 60 frames = 2 seconds.

        #endregion

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sand Sharknado Staff");
            // Tooltip.SetDefault("Summons a sandnado to fight for you");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 47;
            Item.knockBack = 2f;
            Item.mana = 10;

            Item.shoot = ModContent.ProjectileType<SandnadoMinion>();

            Item.width = 48;
            Item.height = 56;
            Item.useTime = Item.useAnimation = 20;

            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Lime;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int sandnado = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(sandnado))
                    Main.projectile[sandnado].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ForgottenApexWand>().
                AddIngredient<GrandScale>().
                AddIngredient<AerialiteBar>(10).
                AddIngredient(ItemID.AncientCloth, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
