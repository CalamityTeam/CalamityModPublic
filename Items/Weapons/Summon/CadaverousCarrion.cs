using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 384;
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.width = 54;
            Item.height = 56;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.NPCDeath13;
            Item.shoot = ModContent.ProjectileType<OldDukeHeadCorpse>();

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //CalamityUtils.OnlyOneSentry(player, type);
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
