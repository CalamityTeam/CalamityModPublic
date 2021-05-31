using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DeepSeaDumbbell : RogueWeapon
    {
        private const float FlexMultMax = 5f;
        private float flexMult = 1f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Sea Dumbbell");
            Tooltip.SetDefault("Throws a dumbbell that bounces and flings weights with each bounce\n" +
                "Right click to flex, increasing the damage of your next attack up to 5 times damage\n" +
                "Flexes can hit enemies directly");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.height = 24;
            item.damage = 466;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 25;
            item.useAnimation = 25;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = false;
            item.shoot = ModContent.ProjectileType<DeepSeaDumbbell1>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useStyle = ItemUseStyleID.HoldingUp;
                item.noMelee = false;
                item.noUseGraphic = false;
                item.autoReuse = false;
                item.UseSound = SoundID.Item1;
            }
            else
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.noMelee = true;
                item.noUseGraphic = true;
                item.autoReuse = true;
                item.UseSound = SoundID.Item1;
            }
            return base.CanUseItem(player);
        }

        public override float SafeSetUseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 5f / 9f : 1f;

        public override void SafeModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            mult += flexMult - 1f;
        }

        // Reset flex multiplier on direct hits.
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit) => flexMult = 1f;
        public override void OnHitPvp(Player player, Player target, int damage, bool crit) => flexMult = 1f;


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Alt fire doesn't actually shoot anything. It flexes, increasing the damage of the next attack.
            if (player.altFunctionUse == 2)
            {
                flexMult = MathHelper.Clamp(flexMult + 1f, 1f, FlexMultMax);
                return false;
            }

            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = true;

            // Reset flex damage on any throw, stealth strike or not.
            flexMult = 1f;
            return false;
        }
    }
}
