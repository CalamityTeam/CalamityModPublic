using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DeepSeaDumbbell : RogueWeapon
    {
        private static int BaseDamage = 900;
        private static float MeleeFlexMult = 25f;
        private float flexBonusDamageMult = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Sea Dumbbell");
            Tooltip.SetDefault("Throws a dumbbell that bounces and flings weights with each bounce\n" +
                "Right click to flex with it, increasing the power of your next stealth strike\n" +
                "Flexes are melee attacks, and are boosted by flexing and stealth");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = BaseDamage;
            item.crit -= 2;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.useTime = 25;
            item.knockBack = 16f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = false;
            item.height = 24;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<DeepSeaDumbbell1>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 21;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useStyle = 4;
                item.useAnimation = 45;
                item.useTime = 45;
                item.noMelee = false;
                item.noUseGraphic = false;
                item.autoReuse = false;
                item.UseSound = SoundID.Item1;
            }
            else
            {
                item.useStyle = 1;
                item.useAnimation = 25;
                item.useTime = 25;
                item.noMelee = true;
                item.noUseGraphic = true;
                item.autoReuse = true;
                item.UseSound = SoundID.Item1;
            }
            return base.CanUseItem(player);
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            if (player.Calamity().StealthStrikeAvailable())
                mult += flexBonusDamageMult;
            base.ModifyWeaponDamage(player, ref add, ref mult, ref flat);
        }

        // Flexes deal 25x damage if you actually hit with them directly.
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            damage = (int)(damage * MeleeFlexMult);
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            damage = (int)(damage * MeleeFlexMult);
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Alt fire doesn't actually shoot anything. It flexes, increasing the damage of the next stealth strike
            if (player.altFunctionUse == 2)
            {
                flexBonusDamageMult += 1f;
                if (flexBonusDamageMult > 10f)
                    flexBonusDamageMult = 10f;
                return false;
            }

            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            if (player.Calamity().StealthStrikeAvailable())
            {
                Main.projectile[proj].Calamity().stealthStrike = true;
                flexBonusDamageMult = 0f;
            }
            return false;
        }
    }
}
