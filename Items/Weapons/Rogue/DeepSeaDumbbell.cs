using Terraria.DataStructures;
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
            Item.width = 38;
            Item.height = 24;
            Item.damage = 466;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.shoot = ModContent.ProjectileType<DeepSeaDumbbell1>();
            Item.shootSpeed = 20f;
            Item.Calamity().rogue = true;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.HoldUp;
                Item.noMelee = false;
                Item.noUseGraphic = false;
                Item.autoReuse = false;
                Item.UseSound = SoundID.Item1;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.autoReuse = true;
                Item.UseSound = SoundID.Item1;
            }
            return base.CanUseItem(player);
        }

        public override float SafeSetUseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 5f / 9f : 1f;

        public override void SafeModifyWeaponDamage(Player player, ref StatModifier damage, ref float flat)
        {
            damage *= flexMult;
        }

        // Reset flex multiplier on direct hits.
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit) => flexMult = 1f;
        public override void OnHitPvp(Player player, Player target, int damage, bool crit) => flexMult = 1f;


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Alt fire doesn't actually shoot anything. It flexes, increasing the damage of the next attack.
            if (player.altFunctionUse == 2)
            {
                flexMult = MathHelper.Clamp(flexMult + 1f, 1f, FlexMultMax);
                return false;
            }

            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 1.3);

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = true;

            // Reset flex damage on any throw, stealth strike or not.
            flexMult = 1f;
            return false;
        }
    }
}
