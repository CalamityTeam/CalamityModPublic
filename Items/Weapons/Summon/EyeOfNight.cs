using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EyeOfNight : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.mana = 10;
            Item.width = Item.height = 36;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;

            Item.UseSound = SoundID.NPCHit1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EyeOfNightSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.UnitY * -3f, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VileFeeder>().
                AddIngredient<StaffOfNecrosteocytes>().
                AddIngredient<BelladonnaSpiritStaff>().
                AddIngredient(ItemID.ImpStaff).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                Register();
            CreateRecipe().
                AddIngredient<ScabRipper>().
                AddIngredient<StaffOfNecrosteocytes>().
                AddIngredient<BelladonnaSpiritStaff>().
                AddIngredient(ItemID.ImpStaff).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
