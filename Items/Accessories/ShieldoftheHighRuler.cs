using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class ShieldoftheHighRuler : ModItem
    {
        public const int ShieldSlamIFrames = 12;
        public bool dashVelocityBoosted = false;
        private const float EoCDashVelocity = 14.5f;
        private const float TabiDashVelocity = 18.9f;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Shield of the High Ruler");
            Tooltip.SetDefault("For the fate of the kingdom\n" +
                "Grants immunity to knockback\n" +
                "Immune to most debuffs\n" +
                "+10 max life\n" +
                "Grants an improved Shield of Cthulhu dash\n" +
                "Bonking an enemy reduces the delay before you can dash again by 50%\n" +
                "If you are facing a projectile when it hits you it will deal 15% less damage");
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 30;
            Item.knockBack = 9f;
            Item.width = 36;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.defense = 12;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.dashType = 2;
            modPlayer.DashID = string.Empty;
            modPlayer.copyrightInfringementShield = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Stoned] = true;
            player.statLifeMax2 += 10;

            // If the player hasn't hit anything with the shield and a dash is currently happening, increase velocity on the first frame of the dash to be on par with Tabi.
            // EoC dash decelerates faster than Tabi, so compensate for it by increasing the Tabi dash velocity value by an approximate amount.
            if (player.eocHit == -1 && player.dashDelay == -1)
            {
                if (!dashVelocityBoosted)
                {
                    dashVelocityBoosted = true;
                    player.velocity.X *= TabiDashVelocity / EoCDashVelocity;
                }
            }
            else
                dashVelocityBoosted = false;

            // Dash delay reduced to 15 frames (half the original 30) if an enemy is bonked.
            if (player.eocHit != -1)
            {
                if (player.dashDelay > 15)
                    player.dashDelay = 15;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EoCShield).
                AddIngredient(ItemID.CobaltShield).
                AddIngredient<LifeAlloy>(4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
