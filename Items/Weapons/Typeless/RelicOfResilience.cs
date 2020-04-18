using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Projectiles.Damageable;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class RelicOfResilience : ModItem
    {
        public const int CooldownSeconds = 5;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Relic of Resilience");
            Tooltip.SetDefault("Summons a bulwark at the mouse position\n" +
                               "The bulwark takes damage from enemies and all projectiles.\n" +
                               "On death, the bulwark explodes into a burst of shards\n" +
                               "After a bit of time, the shards come together to reform the original bulwark.\n" +
                               $"This reformation can only happen {ArtifactOfResilienceBulwark.MaxReformations} times.\n" +
                               "You gain a small cooldown when summoning a new bulwark.\n" +
                               "If a bulwark or shard already exists, using this item will relocate them");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 34;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.useAnimation = item.useTime = 28;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item45;
            item.autoReuse = true;
            item.noMelee = true;
            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.shoot = ModContent.ProjectileType<ArtifactOfResilienceBulwark>();
            item.shootSpeed = 0f;
        }
        public override bool CanUseItem(Player player) => !player.HasBuff(ModContent.BuffType<ResilienceCooldown>());
        public override bool UseItem(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(ModContent.BuffType<ResilienceCooldown>(), CooldownSeconds * 60);
            if (player.ownedProjectileCounts[item.shoot] > 0 ||
                player.ownedProjectileCounts[ModContent.ProjectileType<ArtifactOfResilienceShard1>()] > 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].type == item.shoot)
                    {
                        Main.projectile[i].Center = Main.MouseWorld;
                        Main.projectile[i].netUpdate = true;
                    }
                    else
                    {
                        if (Main.projectile[i].modProjectile is ArtifactOfResilienceShard1)
                        {
                            ((ArtifactOfResilienceShard1)Main.projectile[i].modProjectile).StartingPosition = Main.MouseWorld;
                        }
                        if (Main.projectile[i].modProjectile is ArtifactOfResilienceShard2)
                        {
                            ((ArtifactOfResilienceShard2)Main.projectile[i].modProjectile).StartingPosition = Main.MouseWorld;
                        }
                        if (Main.projectile[i].modProjectile is ArtifactOfResilienceShard3)
                        {
                            ((ArtifactOfResilienceShard3)Main.projectile[i].modProjectile).StartingPosition = Main.MouseWorld;
                        }
                        if (Main.projectile[i].modProjectile is ArtifactOfResilienceShard4)
                        {
                            ((ArtifactOfResilienceShard4)Main.projectile[i].modProjectile).StartingPosition = Main.MouseWorld;
                        }
                        if (Main.projectile[i].modProjectile is ArtifactOfResilienceShard5)
                        {
                            ((ArtifactOfResilienceShard5)Main.projectile[i].modProjectile).StartingPosition = Main.MouseWorld;
                        }
                        if (Main.projectile[i].modProjectile is ArtifactOfResilienceShard6)
                        {
                            ((ArtifactOfResilienceShard6)Main.projectile[i].modProjectile).StartingPosition = Main.MouseWorld;
                        }
                    }
                }
            }
            else
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
