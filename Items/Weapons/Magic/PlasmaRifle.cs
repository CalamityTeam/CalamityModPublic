using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PlasmaRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Rifle");
            Tooltip.SetDefault("Fires a plasma blast that explodes\n" +
                "Right click to fire plasma bolts");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.mana = 40;
            Item.DamageType = DamageClass.Magic;
            Item.width = 72;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = CommonCalamitySounds.PlasmaBlastSound;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<PlasmaShot>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = CommonCalamitySounds.PlasmaBoltSound;
            }
            else
            {
                Item.UseSound = CommonCalamitySounds.PlasmaBlastSound;
            }
            return base.CanUseItem(player);
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 0.25f;
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1f;
            return 0.2f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * 1.15);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PlasmaBolt>(), damage, knockback, player.whoAmI);
            }
            else
                Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 0.88), knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ToxicFlask).
                AddIngredient(ItemID.Musket).
                AddIngredient<UeliaceBar>(7).
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.ToxicFlask).
                AddIngredient(ItemID.TheUndertaker).
                AddIngredient<UeliaceBar>(7).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
