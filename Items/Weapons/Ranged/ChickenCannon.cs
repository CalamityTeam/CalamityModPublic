using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ChickenCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken Cannon");
            Tooltip.SetDefault("Fires chicken flares which create enormous incendiary explosions\n" +
                "Right click to detonate all airborne rockets");
        }

        public override void SetDefaults()
        {
            item.damage = 530;
            item.ranged = true;
            item.width = 76;
            item.height = 24;
            item.useTime = item.useAnimation = 33;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            // No use sound is intentional
            item.autoReuse = true;
            item.shootSpeed = 17.5f;
            item.shoot = ModContent.ProjectileType<ChickenRocket>();
            item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Play a sound and detonate all in-flight rockets, but don't shoot anything.
            if (player.altFunctionUse == 2)
            {
                Main.PlaySound(SoundID.Item110, position);
                DetonateRockets(player);
                return false;
            }

            // Otherwise just play a grenade launcher sound and fire a rocket.
            Main.PlaySound(SoundID.Item61, position);
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), item.shoot, damage, knockBack, player.whoAmI);
            return false;
        }

        private void DetonateRockets(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != player.whoAmI || p.type != item.shoot)
                    continue;

                // All rockets will instantly explode on the next frame and send packets to indicate as such.
                p.timeLeft = 1;
                p.netUpdate = true;
                p.netSpam = 0;
            }
        }

        // Right clicking to detonate all rockets is very fast.
        public override float UseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 5f : 1f;
    }
}
