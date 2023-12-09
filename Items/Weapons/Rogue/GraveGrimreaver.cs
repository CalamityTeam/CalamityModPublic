using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GraveGrimreaver : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.damage = 84;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 48;
            Item.useTime = 48;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 68;
            Item.shoot = ModContent.ProjectileType<GraveGrimreaverProjectile>();
            Item.shootSpeed = 16.5f;
            Item.DamageType = RogueDamageClass.Instance;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().donorItem = true;
        }

		public override float StealthDamageMultiplier => 0.40f;
        public override float StealthVelocityMultiplier => 1.75f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Sickle).
                AddRecipeGroup("AnyTombstone").
                AddIngredient(ItemID.Bone, 50).
                AddIngredient(ItemID.CursedFlame, 5).
                AddIngredient(ItemID.SoulofFright, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
