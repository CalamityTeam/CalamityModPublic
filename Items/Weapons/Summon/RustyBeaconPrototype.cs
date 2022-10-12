using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class RustyBeaconPrototype : ModItem
    {
        public const int PulseReleaseRate = 120;

        public const int PulseLifetime = 95;

        public const int IrradiatedDebuffTime = 180;

        public const int PoisonedDebuffTime = 360;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Beacon Prototype");
            Tooltip.SetDefault("Summons a long-abandoned drone to support you\n" +
                               "The drone hovers in place and releases toxic waves that inflict irradiated and poisoned");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.width = 28;
            Item.height = 20;
            Item.damage = 10;
            Item.useTime = Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item15; // Phaseblade sound effect
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RustyDrone>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 16f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SulphuricScale>(20).
                AddRecipeGroup("AnySilverBar", 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
