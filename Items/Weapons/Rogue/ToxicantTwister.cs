using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ToxicantTwister : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<SulphuricPoisoning>()];
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 46;
            Item.damage = 272;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ToxicantTwisterProj>();
            Item.shootSpeed = 20f;
            Item.DamageType = RogueDamageClass.Instance;

            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override float StealthDamageMultiplier => 0.4f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rotationAngle = MathHelper.ToRadians(20f);
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = 0; i < 2; i++)
                {
                    int proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(rotationAngle), type, damage, knockback, player.whoAmI, 0, 0, 0 + i);
                    int proj2 = Projectile.NewProjectile(source, position, velocity.RotatedBy(rotationAngle * 0.5f) * 0.9f, type, damage, knockback, player.whoAmI, 0, 0, 2 + i);
                    rotationAngle *= -1;
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().stealthStrike = true;
                    if (proj2.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj2].Calamity().stealthStrike = true;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(rotationAngle), type, damage, knockback, player.whoAmI, 0, 0, 0 + i);
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(rotationAngle * 0.5f) * 0.9f, type, damage, knockback, player.whoAmI, 0, 0, 2 + i);
                    rotationAngle *= -1;
                }
            }
            return false;
        }
    }
}
