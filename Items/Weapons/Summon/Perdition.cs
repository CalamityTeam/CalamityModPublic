using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Perdition : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Perdition");
            Tooltip.SetDefault("Summons a beacon at the position of the mouse\n" +
                "When a target is manually selected via right click it releases torrents of souls from below onto the target\n" +
                "Only one beacon may exist at a time");
        }

        public override void SetDefaults()
        {
            item.damage = 444;
            item.mana = 10;
            item.width = item.height = 56;
            item.useTime = item.useAnimation = 10; // 9 because of useStyle 1
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4f;
            item.UseSound = SoundID.DD2_EtherianPortalOpen;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PerditionBeacon>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.sentry = true;

            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                CalamityUtils.OnlyOneSentry(player, type);
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                player.UpdateMaxTurrets();
            }
            else
            {
                // Play some demonic noises prior to a target being selected.
                Main.PlaySound(SoundID.Zombie, player.Center, 93);
                Main.PlaySound(SoundID.Item119, player.Center);
            }
            return false;
        }
    }
}
