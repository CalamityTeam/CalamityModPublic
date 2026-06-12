using CalamityMod.Projectiles.Melee;
using System;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Lionfish : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 33;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<LionfishProj>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;

                for (int s = 0; s < 5; s++)
                {
                    Vector2 spikeVel = velocity;
                    spikeVel *= Main.rand.NextFloat(0.85f, 1.25f);
                    spikeVel = spikeVel.RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI * 0.25, default);
                    int spike = Projectile.NewProjectile(source, position, spikeVel, ModContent.ProjectileType<UrchinSpikeFugu>(), (int)(damage * 0.5), (int)(knockback * 0.5f), player.whoAmI, -10f, 1f);
                    if (spike.WithinBounds(Main.maxProjectiles))
                        Main.projectile[spike].DamageType = RogueDamageClass.Instance;
                }
                return false;
            }
            return true;
        }
    }
}
