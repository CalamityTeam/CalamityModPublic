using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.damage = 416;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 126;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            // No use sound is intentional
            Item.autoReuse = true;
            Item.shootSpeed = 14.5f;
            Item.shoot = ModContent.ProjectileType<ChickenRocket>();
            Item.useAmmo = AmmoID.Rocket;
            Item.channel = true;
        }

        public override void HoldItem(Player player)
        {
            Item.channel = player.altFunctionUse != 2;
            Item.noUseGraphic = player.altFunctionUse != 2;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override bool AltFunctionUse(Player player) => true;

        // Right click doesn't use ammo because it's a detonation signal.
        public override bool ConsumeAmmo(Player player) => player.altFunctionUse != 2;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ChickenCannonHeld>()] <= 0 || player.altFunctionUse == 2;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Play a sound and detonate all in-flight rockets, but don't shoot anything.
            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(SoundID.Item110, position);
                DetonateRockets(player);
                return false;
            }

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ChickenCannonHeld>(), 0, 0f, player.whoAmI, 1);
            // Otherwise just play a grenade launcher sound and fire a rocket.
            //Main.PlaySound(SoundID.Item61, position);
            //Projectile.NewProjectile(source, position, velocity, item.shoot, damage, knockback, player.whoAmI);
            return false;
        }

        private void DetonateRockets(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != player.whoAmI || p.type != Item.shoot)
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
