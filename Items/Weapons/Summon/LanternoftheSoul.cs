using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class LanternoftheSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guidelight of Oblivion");
            Tooltip.SetDefault("Shadows dream of endless fire, flames devour and embers swoop\n" +
            "Summons a lantern turret to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 188;
            item.summon = true;
            item.sentry = true;
            item.mana = 10;
            item.width = 42;
            item.height = 60;
            item.useTime = item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LanternSoul>();
            item.UseSound = SoundID.Item44;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //CalamityUtils.OnlyOneSentry(player, type);
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
