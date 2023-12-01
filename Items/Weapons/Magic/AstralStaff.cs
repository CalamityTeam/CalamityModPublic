using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AstralStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 245;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 26;
            Item.width = 86;
            Item.height = 72;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AstralCrystal>();
            Item.shootSpeed = 15f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 15;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spawnPos = new Vector2(player.MountedCenter.X + Main.rand.Next(-200, 201), player.MountedCenter.Y - 600f);
            Vector2 targetPos = Main.MouseWorld + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31));
            Vector2 velocityReal = targetPos - spawnPos;
            velocityReal.Normalize();
            velocityReal *= 13f;

            int p = Projectile.NewProjectile(source, spawnPos, velocityReal, type, damage, knockback, player.whoAmI);
            Main.projectile[p].ai[0] = targetPos.Y - 120;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
