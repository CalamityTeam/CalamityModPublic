using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class FaceMelter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 165;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 56;
            Item.height = 50;
            Item.useTime = 5;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<MelterNote1>();
            Item.UseSound = SoundID.Item47;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 20;
                Item.useAnimation = 20;
            }
            else
            {
                Item.useTime = 5;
                Item.useAnimation = 10;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MelterAmp>(), damage, knockback, player.whoAmI);
                return false;
            }
            else
            {
                int note = Main.rand.Next(2);
                if (note == 0)
                {
                    damage = (int)(damage * 1.5f);
                    type = ModContent.ProjectileType<MelterNote1>();
                }
                else
                {
                    velocity.X *= 1.5f;
                    velocity.Y *= 1.5f;
                    type = ModContent.ProjectileType<MelterNote2>();
                }
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TheAxe).
                AddIngredient(ItemID.SparkleGuitar). // Stellar Tune from Empress of Light
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<NightmareFuel>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
