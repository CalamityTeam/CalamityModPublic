using CalamityMod.Projectiles.Damageable;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class RelicOfResilience : ModItem
    {
        public const int CooldownSeconds = 5;
        public const float WeaknessDR = 0.45f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Relic of Resilience");
            Tooltip.SetDefault("Summons a bulwark at the mouse position\n" +
                               "The bulwark is killed by enemies and all projectiles.\n" +
                               "On death, the bulwark explodes into a rotating burst of shards\n" +
                               "If an enemy is in the area of the shards, its next attack is much weaker. This effect has a cooldown\n" +
                               "After a bit of time, the shards come together to reform the original bulwark.\n" +
                               $"This reformation can only happen {ArtifactOfResilienceBulwark.MaxReformations} times.\n" +
                               "You gain a small cooldown when summoning a new bulwark.\n" +
                               "If a bulwark already exists, using this item will relocate it");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 34;
            Item.useAnimation = Item.useTime = 28;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item45;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<ArtifactOfResilienceBulwark>();
            Item.shootSpeed = 0f;
        }
        public override bool CanUseItem(Player player) => !player.HasCooldown(Cooldowns.RelicOfResilience.ID);
        public override bool? UseItem(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddCooldown(Cooldowns.RelicOfResilience.ID, CalamityUtils.SecondsToFrames(CooldownSeconds));
            int[] shardTypes = new int[]
            {
                ModContent.ProjectileType<ArtifactOfResilienceShard1>(),
                ModContent.ProjectileType<ArtifactOfResilienceShard2>(),
                ModContent.ProjectileType<ArtifactOfResilienceShard3>(),
                ModContent.ProjectileType<ArtifactOfResilienceShard4>(),
                ModContent.ProjectileType<ArtifactOfResilienceShard5>(),
                ModContent.ProjectileType<ArtifactOfResilienceShard6>(),
            };
            if (player.ownedProjectileCounts[Item.shoot] > 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].type == Item.shoot)
                    {
                        Main.projectile[i].Center = Main.MouseWorld;
                        Main.projectile[i].netUpdate = true;
                    }
                }
            }
            else if (shardTypes.All(proj => player.ownedProjectileCounts[proj] == 0))
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
