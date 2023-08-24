using Terraria.DataStructures;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Kylie : RogueWeapon
    {
        public static float Speed = 14f;
        public override void SetDefaults()
        {
            Item.damage = 63;
            Item.knockBack = 12;
            Item.DamageType = DamageClass.Throwing;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.width = 34;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = Speed;
            Item.shoot = ModContent.ProjectileType<KylieBoomerang>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = RogueDamageClass.Instance;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 16;

		public override float StealthDamageMultiplier => 0.3333f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer p = Main.player[Main.myPlayer].Calamity();
            //If stealth is full, shoot a spread of 3 boomerangs with reduced range
            if (p.StealthStrikeAvailable())
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
                return false;
            }
            return true;
        }
    }
}
