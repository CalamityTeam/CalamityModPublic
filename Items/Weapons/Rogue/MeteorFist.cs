using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class MeteorFist : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.damage = 15;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 15;
            Item.knockBack = 5.75f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.height = 28;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<MeteorFistProj>();
            Item.shootSpeed = 5f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 3.6f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MeteorFistStealth>(), damage, knockback, player.whoAmI, 0f, 4f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MeteorFistProj>(), damage, knockback, player.whoAmI, 0f, 4f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MeteoriteBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
