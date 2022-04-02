using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CadaverousCarrion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cadaverous Carrion");
            Tooltip.SetDefault("Summons a gross Old Duke head on the ground");
        }

        public override void SetDefaults()
        {
            item.damage = 384;
            item.mana = 10;
            item.summon = true;
            item.sentry = true;
            item.width = 54;
            item.height = 56;
            item.useTime = item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.autoReuse = true;
            item.knockBack = 4f;
            item.UseSound = SoundID.NPCDeath13;
            item.shoot = ModContent.ProjectileType<OldDukeHeadCorpse>();

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
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
