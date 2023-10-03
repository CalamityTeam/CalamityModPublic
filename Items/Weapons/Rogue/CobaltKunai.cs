using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class CobaltKunai : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.damage = 52;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.value = 900;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<CobaltKunaiProjectile>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 1.85f;

        public override float StealthVelocityMultiplier => 0.9f;

        public override float StealthKnockbackMultiplier => 0.5f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = -1; i <= 1; i++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i * 9f));
                    int stealth = Projectile.NewProjectile(source, position, perturbedSpeed, ModContent.ProjectileType<CobaltEnergy>(), damage, knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.CobaltBar).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
