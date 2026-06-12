using System.Collections.Generic;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ElephantKiller : RogueWeapon
    {
        public static readonly SoundStyle Throw = new("CalamityMod/Sounds/Item/ElephantKillerThrow");
        public static readonly SoundStyle Catch = new("CalamityMod/Sounds/Item/ElephantKillerCatch");
        public static readonly SoundStyle Shot = new("CalamityMod/Sounds/Item/ElephantKillerShot");
        public static readonly SoundStyle ShotFail = new("CalamityMod/Sounds/Item/ElephantKillerShotFail");
        public static readonly SoundStyle Hit = new("CalamityMod/Sounds/Item/ElephantKillerHit");
        public static readonly SoundStyle BoostedShotHit = new("CalamityMod/Sounds/Item/ElephantKillerBoostedShotHit");
        public static readonly SoundStyle ElephantSound = new("CalamityMod/Sounds/Item/ElephantKillerElephant");
        public static readonly SoundStyle Shine = new("CalamityMod/Sounds/Item/ElephantKillerShine");
        public static readonly SoundStyle Woosh = new("CalamityMod/Sounds/Item/ElephantKillerWoosh");

        public static float stealthGainOnThrowHit => 0.25f; // Amount of stealth gained on a thrown hit based on max stealth
        public static float stealthCostToShoot => 0.1f; // Amount of stealth it costs to fire the gun with right click based on max stealth
        public static float stealthShotDamageMult => 1.9f;
        public static float elephantBoostedShotDamageMult => 1.6f; // This is multiplicative with the stealth shot damage boost
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 58;
            Item.damage = 120;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 38;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<ElephantKillerThrown>();
            Item.shootSpeed = 15f;
            Item.DamageType = RogueDamageClass.Instance;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (Main.LocalPlayer != null)
                Main.LocalPlayer.Calamity().drawingElephantKillerJoke = true;
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Main.LocalPlayer != null)
            {
                list.FindAndReplace("[STEALTHCOST]", stealthCostToShoot.ToPercent());
                list.FindAndReplace("[STEALTHGAIN]", stealthGainOnThrowHit.ToPercent());
            }
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool AltFunctionUse(Player player) => true;
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;
            player.Calamity().mouseWorldListener = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool shoot = player.Calamity().mouseRight;
            Projectile gun = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0, shoot ? 1f : 0);
            gun.scale = 0;
            return false;
        }
    }
}
