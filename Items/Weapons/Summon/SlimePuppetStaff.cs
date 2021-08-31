using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SlimePuppetStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Puppet Staff");
            Tooltip.SetDefault("Summons a slime ball that follows you\n" +
                                "The ball flies toward nearby enemies and explodes into slime on enemy hits\n" +
                                "Does not consume minion slots"); // In other words, bootleg mage :TaxEvasion:
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 26;
            item.mana = 10;
            item.width = 48;
            item.height = 56;
            item.useTime = item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.6f;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SlimeGodPossession");
            item.shoot = ModContent.ProjectileType<SlimePuppet>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(12f);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, Main.myPlayer);

            return false;
        }
    }
}
