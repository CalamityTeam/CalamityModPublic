using Terraria.DataStructures;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FrostcrushValari : RogueWeapon
    {
        public static float Speed = 14f;

        public override void SetDefaults()
        {
            Item.damage = 89;
            Item.knockBack = 12;
            Item.DamageType = DamageClass.Throwing;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.width = 32;
            Item.height = 46;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = Speed;
            Item.shoot = ModContent.ProjectileType<ValariBoomerang>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = RogueDamageClass.Instance;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 16;

		public override float StealthDamageMultiplier => 0.35f;
        public override float StealthKnockbackMultiplier => 0.3333f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
            //If stealth is full, shoot a spread of 3 boomerangs with reduced range and 6 to 10 icicles
            if (modPlayer.StealthStrikeAvailable())
            {
                int spread = 10;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = velocity.RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(source, position, perturbedspeed, type, damage, knockback, player.whoAmI, 0f, 1f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= 10;
                }
                int spread2 = 3;
                int icicleAmt = Main.rand.Next(6,11);
                for (int i = 0; i < icicleAmt; i++)
                {
                    Vector2 perturbedspeed = new Vector2(velocity.X + Main.rand.Next(-3,4), velocity.Y + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread2));
                    Projectile.NewProjectile(source, position, perturbedspeed, (Main.rand.NextBool() ? ModContent.ProjectileType<Valaricicle>() : ModContent.ProjectileType<Valaricicle2>()), damage, 0f, player.whoAmI, 0f, 0f);
                    spread2 -= Main.rand.Next(1,4);
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Kylie>().
                AddIngredient<CryonicBar>(6).
                AddIngredient<Voidstone>(40).
                AddIngredient<CoreofEleum>(5).
                AddTile(TileID.IceMachine).
                Register();
        }
    }
}
