using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class EventHorizon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 46;

            Item.damage = 275;
            Item.knockBack = 3.5f;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;

            Item.useTime = Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();

            Item.UseSound = SoundID.Item84;
            Item.shoot = ModContent.ProjectileType<EventHorizonStar>();
            Item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (float i = 0; i < 8; i++)
            {
                float angle = MathHelper.TwoPi / 8f * i;
                Projectile.NewProjectile(source, player.Center, angle.ToRotationVector2() * 8f, type, damage, knockback, player.whoAmI, angle, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StarShower>().
                AddIngredient<NuclearFury>().
                AddIngredient<RelicofRuin>().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<DarksunFragment>(8).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
