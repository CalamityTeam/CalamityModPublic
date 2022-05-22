using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Exorcism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exorcism");
            Tooltip.SetDefault("Throws a hallowed cross which explodes into a flash of light that damages nearby enemies, closer enemies receiving more damage\n" +
                               "As the cross travels downwards, the damage inflicted by both the cross and flash increases constantly\n" +
                               "Stealth strikes cause the cross to be thrown with full damage immediately. Hallowed stars fall when the cross explodes");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.damage = 55;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 42;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<ExorcismProj>();
            Item.shootSpeed = 10f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, (int)(damage * 1.15f), knockback, player.whoAmI, 2f, damage);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
            else
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 1f, damage);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HolyWater, 10).
                AddIngredient(ItemID.HallowedBar, 12).
                AddIngredient(ItemID.SoulofMight, 6).
                AddIngredient(ItemID.SoulofSight, 6).
                AddIngredient(ItemID.SoulofFright, 6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
